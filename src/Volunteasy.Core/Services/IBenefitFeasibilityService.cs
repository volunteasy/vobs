using Volunteasy.Core.Model;

namespace Volunteasy.Core.Services;

public interface IBenefitProvisionService
{
    public Task<Benefit> RequestBenefit(BenefitAnalysisRequest analysisRequest);
    
    public Task<Benefit> ProvideBenefit(BenefitProvision provision);
}