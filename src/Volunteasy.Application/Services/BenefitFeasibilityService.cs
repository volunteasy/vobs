using Microsoft.EntityFrameworkCore;
using Volunteasy.Core.Data;
using Volunteasy.Core.Errors;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Application.Services;

public class BenefitProvisionService : ServiceBase, IBenefitProvisionService
{
    public BenefitProvisionService(Data data, IVolunteasyContext session) : base(data, session) { }

    public async Task<Benefit> RequestBenefit(long distributionId, DistributionBenefitAnalysisRequest analysisRequest)
    {
        if (analysisRequest.Items == null || !analysisRequest.Items.Any(x => x.Quantity > 0))
            throw new BenefitItemsCountException();

        ValidateDistributionParticipation(distributionId, Session.UserId);
        
        var benefit = CreateBenefit(new BenefitProvision
        {
            DistributionId = distributionId,
            Items = analysisRequest.Items
        }, Session.UserId);
        
        await Data.SaveChangesAsync();

        return benefit with { Position = Data.BenefitQueuePosition(distributionId, benefit.Id) };
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

    private void ValidateDistributionParticipation(long distributionId, long beneficiaryId)
    {
        // If beneficiary is not active, block benefit
        if (Data.Beneficiaries.SingleOrDefault(b => b.Id == beneficiaryId)?.Active ?? true)
            throw new BenefitUnauthorizedForUserException();
        
        var distribution = Data.Distributions
            .Include(d => d.Benefits)
            .SingleOrDefault(d => d.Id == distributionId);

        if (distribution == null)
            throw new ResourceNotFoundException(typeof(Distribution));
        
        distribution.ValidateNewBeneficiary(beneficiaryId);

        // Gets user benefits (including not claimed ones)
        var userBenefits = Data.Benefits
            .Include(b => b.Distribution)
            .Where(b => b.AssistedId == beneficiaryId)
            .OrderByDescending(b => b.ClaimedAt);

        // Checks if user has any benefits claimed in the last 45 days
        var benefit = userBenefits
            .FirstOrDefault(b => b.RecentlyClaimed(DateTime.UtcNow, 45));
        
        if (benefit != null)
            throw new BeneficiaryHasRecentClaimException(benefit, 45);

        benefit = userBenefits
            .FirstOrDefault(b => b.IsInAnOpenDistribution(DateTime.UtcNow));
        
        if (benefit != null)
            throw new BeneficiaryHasOpenDistributionException(benefit);
    }
    
    private long GetExistingUserIdOrCreateNewOne(BeneficiaryCreation user)
    {
        var existing = Data.Beneficiaries
            .WithOrganization(Session.OrganizationId)
            .SingleOrDefault(u => u.Document == user.Document);

        if (existing != null)
        {
            if (!existing.Active)
                throw new BenefitUnauthorizedForUserException();
            
            return existing.Id;
        }

        var beneficiary = user.ToBeneficiary();
        beneficiary.OrganizationId = Session.OrganizationId;
        beneficiary.Active = true;

        var res = Data.Beneficiaries.Add(beneficiary);
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