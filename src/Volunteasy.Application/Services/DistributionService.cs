using Microsoft.EntityFrameworkCore;
using Volunteasy.Core.Data;
using Volunteasy.Core.Errors;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Application.Services;

public class DistributionService : ServiceBase, IDistributionService
{
    public DistributionService(Data data, ISession session) : base(data, session) { }

    public async Task<Distribution> CreateDistribution(DistributionCreationProperties props)
    {
        var dist = await Data.Distributions.AddAsync(props.ToDistribution());
        await Data.SaveChangesAsync();

        return dist.Entity;
    }

    public async Task<Distribution> GetDistributionById(long distributionId)
    {
        var distribution = await Data.Distributions
            .WithOrganization(Session.OrganizationId)
            .SingleOrDefaultAsync(d => d.Id == distributionId);
        
        if (distribution == null)
            throw new ResourceNotFoundException(typeof(Distribution));
        
        return distribution;
    }

    public async Task<(IEnumerable<Distribution>, string?)> ListDistributions(DistributionFilter filter, long pageToken)
    {
        return await Data.Distributions
            .WithOrganization(Session.OrganizationId)
            .WithFilters(
                new(filter.Name != null, d => d.Name != null && d.Name.Contains(filter.Name ?? "")),
                new(filter.OccursAt != null, d => filter.OccursAt >= d.StartsAt && filter.OccursAt <= d.EndsAt))
            .Where(d => d.Id >= pageToken)
            .Paginate(d => d.Id);
    }

    public async Task CancelDistribution(long distributionId)
    {
        var distribution = await GetDistributionById(distributionId);
        distribution.Canceled = true;
        await Data.SaveChangesAsync();
    }

    public async Task OpenDistribution(long distributionId)
    {
        var distribution = await GetDistributionById(distributionId);
        distribution.Canceled = false;
        await Data.SaveChangesAsync();
    }

    public async Task UpdateDistribution(long distributionId, DistributionCreationProperties props)
    {
        var distribution = await GetDistributionById(distributionId);
        
        if (distribution.Canceled)
            throw new ResourceNotFoundException(typeof(Distribution));

        distribution.Name = props.Name;
        distribution.Description = props.Description;
        distribution.StartsAt = props.StartsAt;
        distribution.EndsAt = props.EndsAt;
        distribution.MaxBenefits = props.MaxBenefits;

        await Data.SaveChangesAsync();
    }
}