using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using Volunteasy.Core.Data;    
using Volunteasy.Core.Errors;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Application.Services;

public class BenefitItemService : ServiceBase, IBenefitItemService
{
    public BenefitItemService(Data data, ISession session) : base(data, session) {}

    public async Task AddBenefitItem(long benefitId, BenefitDemandItem item)
    {
        try
        {
            await Data.BenefitItems.AddAsync(new BenefitItem
            {
                BenefitId = benefitId,
                Quantity = item.Quantity,
                ResourceId = item.ResourceId,
                StockMovement = new StockMovement
                {
                    Date = DateTime.UtcNow,
                    Quantity = item.Quantity,
                    ResourceId = item.ResourceId,
                    Type = StockMovementType.Reserved,
                    OrganizationId = Session.OrganizationId
                }
            });

            await Data.SaveChangesAsync();
        }
        catch (ReferenceConstraintException)
        {
            throw new BenefitNotFoundException();
        }
        catch (UniqueConstraintException)
        {
            throw new BenefitItemAlreadySetException();
        }
    }

    public async Task RemoveBenefitItem(long benefitId, long resourceId)
    {
        var item = await Data.BenefitItems.Include(b => b.StockMovement)
            .SingleOrDefaultAsync(b => b.BenefitId == benefitId && b.ResourceId == resourceId);

        if (item == null)
            throw new BenefitNotFoundException();

        Data.BenefitItems.Remove(item);
        await Data.SaveChangesAsync();
    }
}