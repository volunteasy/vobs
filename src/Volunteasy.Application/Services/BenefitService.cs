using Microsoft.EntityFrameworkCore;
using Volunteasy.Core.Data;
using Volunteasy.Core.Errors;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Application.Services;

public class BenefitService : ServiceBase, IBenefitService
{
    public BenefitService(Data data, ISession session) : base(data, session) { }

    public async Task<BenefitDetails> GetBenefitById(long benefitId)
    {
        var benefit = await GetBenefitDetails(
                Data.Benefits
                    .WithOrganization(Session.OrganizationId)
                    .Where(b => b.Id == benefitId), true)
            .SingleOrDefaultAsync();

        if (benefit == null)
            throw new BenefitNotFoundException();

        return benefit with
        {
            Position = benefit.DistributionId == null ? null : Data
                .BenefitQueuePosition(benefit.DistributionId.Value, benefit.Id)
        };
    }

    public async Task<(IEnumerable<BenefitDetails>, string?)> ListBenefits(BenefitFilter filter, long pageToken)
    {

        var query = Data.Benefits.WithFilters(
            new(filter.ClaimedSince != null, b => b.ClaimedAt >= filter.ClaimedSince),
            new(filter.ClaimedUntil != null, b => b.ClaimedAt <= filter.ClaimedUntil),
            new (filter.NotClaimedOnly, b => b.ClaimedAt == null)
        );

        // Only filter by organizationId if filter does not have an assistedId set
        if (filter.AssistedId == null)
            query = query.WithOrganization(Session.OrganizationId);
        
        // Only paginate if filter does not have a distributionId set
        if (filter.DistributionId == null)
            query = query.WithPageToken(pageToken);

        var comm = query
            .Include(b => b.Items)
            .Include(b => b.Distribution)
            .Join(Data.Users, b => b.AssistedId, r => r.Id,
                (benefit, user) => new { Benefit = benefit, UserName = user.Name })
            .Select(vl => new BenefitDetails
            {
                DistributionId = vl.Benefit.DistributionId,
                DistributionDate = vl.Benefit.Distribution == null ? null : vl.Benefit.Distribution.StartsAt,
                DistributionName = vl.Benefit.Distribution == null ? null : vl.Benefit.Distribution.Name,

                UserId = vl.Benefit.AssistedId,
                UserName = vl.UserName,

                ClaimedAt = vl.Benefit.ClaimedAt,
                ItemsCount = Data.BenefitItems.Count(bi => bi.BenefitId == vl.Benefit.Id),
                Id = vl.Benefit.Id
            })
            .OrderBy(b => b.Id);

        IEnumerable<BenefitDetails>? list;
        string? nextPage = null;

        if (filter.DistributionId != null)
        {
            list = await comm.ToListAsync();
        }
        else
        {
            (list, nextPage) = await comm.Paginate(x => x.Id);
        }

        list = list.Select(b => b with
        {
            Position = b.DistributionId == null
                ? null
                : Data
                    .BenefitQueuePosition(b.DistributionId.Value, b.Id)
        }).OrderBy(b => b.Position);

        return (list, nextPage);

    }

    public async Task ClaimBenefit(long benefitId)
    {
        var benefit = await Data.Benefits.SingleOrDefaultAsync(b => b.Id == benefitId);
        if (benefit == null)
            throw new BenefitNotFoundException();

        benefit.ClaimedAt = DateTime.UtcNow;
        await Data.SaveChangesAsync();
    }

    public async Task CancelBenefit(long benefitId)
    {
        var benefit = await Data.Benefits.SingleOrDefaultAsync(b => b.Id == benefitId);
        if (benefit == null)
            throw new BenefitNotFoundException();

        benefit.RevokedReason = RevokedBenefitReason.Canceled;
        await Data.SaveChangesAsync();
    }

    private IQueryable<BenefitDetails> GetBenefitDetails(IQueryable<Benefit> q, bool includeItems = false)
    {
        if (includeItems)
            q = q.Include(b => b.Items);
        return q
            .Include(b => b.Distribution)
            .Join(Data.Users, b => b.AssistedId, r => r.Id,
                (benefit, user) => new { Benefit = benefit, UserName = user.Name })
            .Select(vl => new BenefitDetails
            {
                DistributionId = vl.Benefit.DistributionId,
                DistributionDate = vl.Benefit.Distribution == null ? null : vl.Benefit.Distribution.StartsAt,
                DistributionName = vl.Benefit.Distribution == null ? null : vl.Benefit.Distribution.Name,

                UserId = vl.Benefit.AssistedId,
                UserName = vl.UserName,

                Items = !includeItems ? null : Data.BenefitItems
                    .Include(i => i.Resource)
                    .Where(b => b.BenefitId == vl.Benefit.Id)
                    .Select(i => new BenefitItemDetails
                    {
                        Quantity = i.Quantity, 
                        ResourceId = i.ResourceId, 
                        ResourceName = i.Resource == null ? null : i.Resource.Name,
                        StockMovementId = i.StockMovementId,
                        
                    })
                    .ToList(),
                ClaimedAt = vl.Benefit.ClaimedAt,
                ItemsCount = vl.Benefit.Items != null ? vl.Benefit.Items.Count : Data.BenefitItems.Count(bi =>
                    bi.BenefitId == vl.Benefit.Id),
                Id = vl.Benefit.Id,
            }).OrderBy(b => b.Id);
    }
}