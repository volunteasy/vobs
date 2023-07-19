using Microsoft.EntityFrameworkCore;
using Volunteasy.Core.Data;
using Volunteasy.Core.Errors;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Application.Services;

public class DistributionService : ServiceBase, IDistributionService
{
    public DistributionService(Data data, IVolunteasyContext session) : base(data, session) { }

    public async Task<Distribution> CreateDistribution(DistributionCreationProperties props)
    {
        var dist = await Data.Distributions.AddAsync(props.ToDistribution());
        await Data.SaveChangesAsync();

        return dist.Entity;
    }

    public async Task<DistributionDto> GetDistributionById(long distributionId)
    {
        var distribution = await Data.DistributionDetails(Session.UserId)
            .WithOrganization(Session.OrganizationId)
            .SingleOrDefaultAsync(d => d.Id == distributionId);
        
        if (distribution == null)
            throw new ResourceNotFoundException(typeof(Distribution));

        if (distribution.Benefit != null)
            distribution = distribution with
            {
                Benefit = distribution.Benefit with
                {
                    Position = Data.BenefitQueuePosition(distributionId, distribution.Benefit.BenefitId) ?? 0,
                }
            };

        return distribution;
    }

    public async Task<PaginatedList<DistributionDto>> ListDistributions(DistributionFilter filter, long pageToken)
    {
        var res = await  Data.DistributionDetails(Session.UserId)
            .WithOrganization(Session.OrganizationId)
            .OrderByDescending(d => d.EndsAt)
            .WithFilters(
                new(filter.Name != null, d => d.Name != null && d.Name.Contains(filter.Name ?? "")),
                new(filter.OccursAt != null, d => filter.OccursAt >= d.StartsAt && filter.OccursAt <= d.EndsAt))
            .Where(d => d.Id >= pageToken)
            .PaginateList(d => d.Id); 

        return res with
        {
            List = res.List.Select(d => d with
            {
                Benefit = d.Benefit == null
                    ? null
                    : d.Benefit with
                    {
                        Position = Data.BenefitQueuePosition(d.Id, d.Benefit.BenefitId) ?? 0,
                    }
            })
        };
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