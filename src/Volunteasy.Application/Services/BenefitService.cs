using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Volunteasy.Core.Data;
using Volunteasy.Core.Enums;
using Volunteasy.Core.Errors;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Application.Services;

public class BenefitService : ServiceBase, IBenefitService
{
    private readonly IUserService _users;

    private readonly IMembershipService _members;

    public BenefitService(Data data, ISession session, IUserService users, IMembershipService members) : base(data, session)
    {
        _users = users;
        _members = members;
    }

    public async Task<Benefit> CreateBenefit(BenefitDemand demand)
    {
        if (demand.Items == null || !demand.Items.Any(x => x.Quantity > 0))
            throw new BenefitItemsCountException();

        var assistedId = Session.CurrentRole == MembershipRole.Assisted
            ? Session.UserId
            : await GetUserIdFromDemand(demand);
        
        var isImmediate = demand.DistributionId == null;

        return (await Data.Benefits.AddAsync(new Benefit
        {
            AssistedId = assistedId,
            DistributionId = demand.DistributionId,
            ClaimedAt = isImmediate ? DateTime.UtcNow : null,
            Items = demand.Items.Select(x => new BenefitItem
            {
                Quantity = x.Quantity,
                ResourceId = x.ResourceId,
                StockMovement = new StockMovement
                {
                    Date = DateTime.UtcNow,
                    Quantity = x.Quantity,
                    ResourceId = x.ResourceId,
                    Type = isImmediate ? StockMovementType.Output : StockMovementType.Reserved,
                }
            })
        })).Entity;
    }

    public async Task<BenefitDetails> GetBenefitById(long benefitId)
    {
        var benefit = await GetBenefitDetails(Data.Benefits
                .Where(b => b.Id == benefitId))
            .Include(x => x.Items)
            .SingleOrDefaultAsync();

        if (benefit == null)
            throw new BenefitNotFoundException();
        
        return benefit;
    }

    public async Task<(IEnumerable<BenefitDetails>, string?)> ListBenefits(BenefitFilter filter, long pageToken)
    {
        var query = new List<KeyValuePair<bool, Expression<Func<Benefit, bool>>>>
            {
                new(filter.DistributionId != null, b => b.DistributionId == filter.DistributionId),
                new(filter.ClaimedSince != null, b => b.ClaimedAt >= filter.ClaimedSince),
                new(filter.ClaimedUntil != null, b => b.ClaimedAt <= filter.ClaimedUntil),
            }.Where(queryFilter => queryFilter.Key)
            .Aggregate(Data.Benefits.AsQueryable(),
                (current, queryFilter) => current.Where(queryFilter.Value))
            .Where(b => b.Id >= pageToken);
            
            return await GetBenefitDetails(query).Paginate(b => b.Id);
    }

    public async Task ClaimBenefit(long benefitId)
    {
        var benefit = await Data.Benefits.SingleOrDefaultAsync(b => b.Id == benefitId);
        if (benefit == null)
            throw new BenefitNotFoundException();

        benefit.ClaimedAt = DateTime.UtcNow;
        await Data.SaveChangesAsync();
    }

    public async Task CancelBenefit(long benefitId)
    {
        var benefit = await Data.Benefits.SingleOrDefaultAsync(b => b.Id == benefitId);
        if (benefit == null)
            throw new BenefitNotFoundException();

        benefit.RevokedReason = RevokedBenefitReason.Canceled;
        await Data.SaveChangesAsync();
    }

    private IQueryable<BenefitDetails> GetBenefitDetails(IQueryable<Benefit> q)
    {
        return q.Join(Data.Users, b => b.AssistedId, r => r.Id,
                (benefit, user) => new { Benefit = benefit, UserName = user.Name })
            .Join(Data.Distributions, bu => bu.Benefit.DistributionId, d => d.Id, (bu, dist) => new
            {
                bu.Benefit, bu.UserName, DistributionName = dist.Name, DistributionDate = dist.StartsAt
            }).Select(vl => new BenefitDetails
            {
                DistributionId = vl.Benefit.DistributionId,
                DistributionDate = vl.DistributionDate,
                DistributionName = vl.DistributionName,

                UserId = vl.Benefit.AssistedId,
                UserName = vl.UserName,

                ClaimedAt = vl.Benefit.ClaimedAt,
                ItemsCount = Data.BenefitItems.Count(bi =>
                    bi.BenefitId == vl.Benefit.Id),
                Id = vl.Benefit.Id,
            });
    }

    private async Task<long> GetUserIdFromDemand(BenefitDemand demand)
    {
        if (demand.UserId is not null)
        {
            if (!Data.Memberships.Any(m =>
                    m.Role == MembershipRole.Assisted
                    && m.MemberId == demand.UserId
                    && m.OrganizationId == Session.OrganizationId))
                throw new BenefitUnauthorizedForUserException();

            return demand.UserId.Value;
        }
        
        if (demand.NewShallowUser is null)
            throw new BenefitUnauthorizedForUserException();

        var newUser = await _users.CreateUser(demand.NewShallowUser, shallow: true);

        await _members.EnrollOrganization(Session.OrganizationId, newUser.Id, MembershipRole.Assisted);
        return newUser.Id;
    }
}