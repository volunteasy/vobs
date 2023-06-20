using Volunteasy.Core.Model;

namespace Volunteasy.Core.Services;

public interface IStockMovementService
{
    Task<StockMovement> CreateStockMovement(StockMovementEdition props);
    Task<float> GetStockBalanceByResourceId(long resourceId);
    Task<(IEnumerable<StockMovement>, string?)> ListStockMovements(StockMovementFilter type);
}