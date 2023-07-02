using Volunteasy.Core.Model;

namespace Volunteasy.Core.Services;

public interface IDistributionService
{
    public Task<Distribution> CreateDistribution(DistributionCreationProperties props);

    public Task<Distribution> GetDistributionById(long distributionId);
    
    public Task<(IEnumerable<Distribution>, string?)> 
        ListDistributions(DistributionFilter filter, long pageToken);

    public Task CancelDistribution(long distributionId);
    
    public Task OpenDistribution(long distributionId);

    public Task UpdateDistribution(long distributionId, DistributionCreationProperties props);
}