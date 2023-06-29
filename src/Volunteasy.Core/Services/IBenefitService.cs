using Volunteasy.Core.Model;

namespace Volunteasy.Core.Services;

public interface IBenefitService
{
    public Task<Benefit> CreateBenefit(BenefitDemand demand);

    public Task<BenefitDetails> GetBenefitById(long benefitId);
    
    public Task<(IEnumerable<BenefitDetails>, string?)> 
        ListBenefits(BenefitFilter filter, long pageToken);
    
    public Task ClaimBenefit(long benefitId);
    
    public Task CancelBenefit(long benefitId);
}