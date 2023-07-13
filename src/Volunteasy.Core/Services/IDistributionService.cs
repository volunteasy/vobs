using Volunteasy.Core.Model;

namespace Volunteasy.Core.Services;

public interface IDistributionService
{
    public Task<Distribution> CreateDistribution(DistributionCreationProperties props);

    public Task<DistributionDto> GetDistributionById(long distributionId);
    
    public Task<PaginatedList<DistributionDto>> ListDistributions(DistributionFilter filter, long pageToken);

    public Task CancelDistribution(long distributionId);
    
    public Task OpenDistribution(long distributionId);

    public Task UpdateDistribution(long distributionId, DistributionCreationProperties props);
}