using Volunteasy.Core.Model;

namespace Volunteasy.Core.Services;

public interface IBenefitProvisionService
{
    public Task<Benefit> RequestBenefit(long distributionId, DistributionBenefitAnalysisRequest analysisRequest);
    
    public Task<Benefit> ProvideBenefit(BenefitProvision provision);
}