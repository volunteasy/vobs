using Volunteasy.Core.Model;

namespace Volunteasy.Core.Services;

public interface IBenefitItemService
{
    public Task AddBenefitItem(long benefitId, BenefitDemandItem item);
    
    public Task RemoveBenefitItem(long benefitId, long resourceId);
}