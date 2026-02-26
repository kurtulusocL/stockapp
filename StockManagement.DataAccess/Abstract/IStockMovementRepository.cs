using StockManagement.DataAccess.GenericRepository;
using StockManagement.Domain.Entities;

namespace StockManagement.DataAccess.Abstract
{
    public interface IStockMovementRepository : IEntityRepository<StockMovement>
    {
        Task<bool> SetActiveAsync(int id);
        Task<bool> SetInActiveAsync(int id);
        Task<bool> SetDeletedAsync(int id);
        Task<bool> SetNotDeletedAsync(int id);
    }
}
