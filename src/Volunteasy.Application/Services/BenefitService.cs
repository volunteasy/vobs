using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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

        var assistedId = Session.CurrentRole() == MembershipRole.Assisted
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
        var benefit = Data.Benefits
            .Where(b => b.Id == benefitId)
            .Join(Data.Users, b => b.AssistedId, r => r.Id, (benefit, user) => new { Benefit = benefit, UserName = user.Name })
            .Join(Data.Distributions, bu => bu.Benefit.DistributionId, d => d.Id, (bu, dist) => new
            {
                bu.Benefit, bu.UserName, DistributionName = dist.Name, DistributionDate = dist.OccursAt
            }).Select(vl => new BenefitDetails
            {
                
            }).Include(x => x.Items, );
    }

    public async Task<(IEnumerable<BenefitDetails>, string?)> ListBenefits(BenefitFilter filter, long pageToken)
    {
        throw new NotImplementedException();
    }

    public async Task ClaimBenefit(long benefitId)
    {
        throw new NotImplementedException();
    }

    public async Task CancelBenefit(long benefitId)
    {
        throw new NotImplementedException();
    }

    public Task ValidateBenefitDemand(BenefitDemand _, long __)
    {
        return Task.CompletedTask;
    }

    public async Task ValidateBenefitDemand()
    {
        throw new NotImplementedException();
    }

    public async Task AddBenefitItem(long benefitId, BenefitDemandItem item)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveBenefitItem(long benefitId, long resourceId)
    {
        throw new NotImplementedException();
    }

    public async Task SetBenefitItemQuantity(long benefitId, long resourceId, decimal quantity)
    {
        throw new NotImplementedException();
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