using Microsoft.EntityFrameworkCore;
using Volunteasy.Core.Data;
using Volunteasy.Core.Enums;
using Volunteasy.Core.Errors;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Application.Services;

public class BenefitProvisionService : ServiceBase, IBenefitProvisionService
{
    public BenefitProvisionService(Data data, ISession session) : base(data, session) { }

    public async Task<Benefit> RequestBenefit(BenefitAnalysisRequest analysisRequest)
    {
        if (analysisRequest.Items == null || !analysisRequest.Items.Any(x => x.Quantity > 0))
            throw new BenefitItemsCountException();

        ValidateDistributionParticipation(analysisRequest.DistributionId, Session.UserId);
        
        var benefit = CreateBenefit(new BenefitProvision
        {
            DistributionId = analysisRequest.DistributionId,
            Items = analysisRequest.Items
        }, Session.UserId);
        
        await Data.SaveChangesAsync();

        return benefit with { Position = Data.BenefitQueuePosition(analysisRequest.DistributionId, benefit.Id) };
    }
    
    public async Task<Benefit> ProvideBenefit(BenefitProvision provision)
    {
        if (provision.Items == null || !provision.Items.Any(x => x.Quantity > 0))
            throw new BenefitItemsCountException();

        if (provision.AssistedUser == null)
            throw new BenefitUnauthorizedForUserException();

        var assistedId = GetExistingUserIdOrCreateNewOne(provision.AssistedUser);

        if (provision.DistributionId != null)
            ValidateDistributionParticipation(provision.DistributionId ?? 0, assistedId);
        
        var benefit = CreateBenefit(provision, assistedId);

        await Data.SaveChangesAsync();
        return benefit with { Position = provision.DistributionId == null 
            ? null 
            : Data.BenefitQueuePosition(provision.DistributionId.Value, benefit.Id) };
    }

    private void ValidateDistributionParticipation(long distributionId, long assistedId)
    {
        var distribution = Data.Distributions
            .Include(d => d.Benefits)
            .SingleOrDefault(d => d.Id == distributionId);

        if (distribution == null)
            throw new ResourceNotFoundException(typeof(Distribution));
        
        if (distribution.Benefits?.Any(b => b.AssistedId == assistedId) ?? false)
            throw new BenefitUnauthorizedForUserException();

        if (!distribution.CanAcceptNewBenefits(DateTime.UtcNow, distribution.Benefits?.Count() ?? 0))
            throw new DistributionClosedOrFullException();
    }
    
    private long GetExistingUserIdOrCreateNewOne(AssistedUser user)
    {
        var existing = Data.Users
            .Join(Data.Memberships, u => u.Id, m => m.MemberId, (u, m) 
                => new {User = u, OrgId = m.OrganizationId})
            .Where(x => x.OrgId == Session.OrganizationId)
            .Select(x => x.User)
            .SingleOrDefault(u => u.Document == user.Document);

        if (existing != null)
            return existing.Id;
        
        var res = Data.Add(new User
        {
            Document = user.Document,
            Name = user.Name ?? "",
            Email = "",
            Address = user.Address,
            PhoneAddress = user.Phone ?? "",
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
         
        return res.Entity.Id;
    }

    private Benefit CreateBenefit(BenefitProvision analysis, long assistedId)
    {
        var resourceId = 
            GetUniqueOrganizationResourceId(Session.OrganizationId);

        var isImmediate = analysis.DistributionId == null;

        return Data.Benefits.Add(new Benefit
        {
            AssistedId = assistedId,
            DistributionId = analysis.DistributionId,
            ClaimedAt = isImmediate ? DateTime.UtcNow : null,
            Items = analysis.Items?.Select(x => new BenefitItem
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
        }).Entity;
    }

    // GetUniqueOrganizationResourceId fetches the id of the
    // unique resource the organization has at the moment
    private long GetUniqueOrganizationResourceId(long organizationId)
    {
        return Data.Resources
            .Where(r => r.OrganizationId == organizationId)
            .Select(r => r.Id)
            .SingleOrDefault();
    }
}