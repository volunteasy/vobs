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

        var resourceId = await 
            Data.Resources
                .Where(r => r.OrganizationId == Session.OrganizationId)
                .Select(r => r.Id)
                .SingleOrDefaultAsync();

        if (demand.Items == null || !demand.Items.Any(x => x.Quantity > 0))
            throw new BenefitItemsCountException();


        var assistedId = Session.CurrentRole == MembershipRole.Assisted
            ? Session.UserId
            : await GetUserIdFromDemand(demand);
        
        var isImmediate = demand.DistributionId == null;

        var res = await Data.Benefits.AddAsync(new Benefit
        {
            AssistedId = assistedId,
            DistributionId = demand.DistributionId,
            ClaimedAt = isImmediate ? DateTime.UtcNow : null,
            Items = demand.Items.Select(x => new BenefitItem
            {
                Quantity = x.Quantity,
                ResourceId = resourceId,
                StockMovement = new StockMovement
                {
                    Date = DateTime.UtcNow,
                    Quantity = x.Quantity,
                    ResourceId = resourceId,
                    Type = isImmediate ? StockMovementType.Output : StockMovementType.Reserved,
                }
            }).ToList()
        });

        await Data.SaveChangesAsync();
        return res.Entity;
    }

    public async Task<BenefitDetails> GetBenefitById(long benefitId)
    {
        var benefit = await GetBenefitDetails(
                Data.Benefits.Where(b => b.Id == benefitId), true)
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

    private IQueryable<BenefitDetails> GetBenefitDetails(IQueryable<Benefit> q, bool includeItems = false)
    {
        if (includeItems)
            q = q.Include(b => b.Items);
        return q
            .Include(b => b.Distribution)
            .Join(Data.Users, b => b.AssistedId, r => r.Id,
                (benefit, user) => new { Benefit = benefit, UserName = user.Name })
            .Select(vl => new BenefitDetails
            {
                DistributionId = vl.Benefit.DistributionId,
                DistributionDate = vl.Benefit.Distribution == null ? null : vl.Benefit.Distribution.StartsAt,
                DistributionName = vl.Benefit.Distribution == null ? null : vl.Benefit.Distribution.Name,

                UserId = vl.Benefit.AssistedId,
                UserName = vl.UserName,

                Items = !includeItems ? null : Data.BenefitItems
                    .Include(i => i.Resource)
                    .Where(b => b.BenefitId == vl.Benefit.Id)
                    .Select(i => new BenefitItemDetails
                    {
                        Quantity = i.Quantity, 
                        ResourceId = i.ResourceId, 
                        ResourceName = i.Resource == null ? null : i.Resource.Name,
                        StockMovementId = i.StockMovementId,
                        
                    })
                    .ToList(),
                ClaimedAt = vl.Benefit.ClaimedAt,
                ItemsCount = vl.Benefit.Items != null ? vl.Benefit.Items.Count : Data.BenefitItems.Count(bi =>
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

        var user = Data.Add(new User
        {
            Document = demand.NewShallowUser.Document,
            Name = demand.NewShallowUser.Name,
            Email = demand.NewShallowUser.Email,
            Address = demand.NewShallowUser.Address,
            Memberships = new List<Membership>
            {
                new()
                {
                    Role = MembershipRole.Assisted,
                    Status = MembershipStatus.Approved,
                    MemberSince = DateTime.Now.ToUniversalTime(),
                    OrganizationId = Session.OrganizationId,
                }
            }
        });
         
        return user.Entity.Id;
    }
}