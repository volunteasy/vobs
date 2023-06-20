using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Volunteasy.Core.Data;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Application.Services;

public record StockMovementService : IStockMovementService
{
    private readonly Data _data;

    private readonly ISession _session;

    public StockMovementService(Data data, ISession session)
    {
        _data = data;
        _session = session;
    }
    
    public async Task<StockMovement> CreateStockMovement(StockMovementEdition props)
    {
        if (props.Quantity < 0)
            props = props with { Quantity = props.Quantity * -1 };

        if (props.Quantity == 0)
            throw new ApplicationException(); // TODO: implement specific exception
        

        var add = _data.StockMovements.Add(new StockMovement
        {
            Description = props.Description,
            Date = DateTime.Now.ToUniversalTime(),
            Quantity = props.Quantity,
            ResourceId = props.ResourceId,
            Type = props.Type,
            OrganizationId = _session.OrganizationId,
        });
        
        await _data.SaveChangesAsync();
        return add.Entity;
    }

    public async Task<float> GetStockBalanceByResourceId(long resourceId)
    {
        return (float) await _data.StockMovements
            .Where(x => x.ResourceId == resourceId)
            .SumAsync(x 
                => x.Type == StockMovementType.Input ? x.Quantity : x.Quantity * -1);
    }

    public async Task<(IEnumerable<StockMovement>, string?)> ListStockMovements(StockMovementFilter filter)
    {
        return await new List<KeyValuePair<bool, Expression<Func<StockMovement, bool>>>>
            {
                new(filter.Type != null, m => m.Type == filter.Type),
                new(filter.Since != null, m => m.Date >= filter.Since),
                new(filter.Until != null, m => m.Date <= filter.Until),
            }.Where(queryFilter => queryFilter.Key)
            .Aggregate(
                _data.StockMovements.Where(m => m.ResourceId == filter.ResourceId),
                (current, queryFilter) => current.Where(queryFilter.Value))
            .Paginate(m => m.Id);
    }
}