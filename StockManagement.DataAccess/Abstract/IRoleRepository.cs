using StockManagement.DataAccess.GenericRepository;
using StockManagement.Domain.Entities;

namespace StockManagement.DataAccess.Abstract
{
    public interface IRoleRepository : IEntityRepository<AppRole>
    {
        Task<bool> SetActiveAsync(string id);
        Task<bool> SetInActiveAsync(string id);
        Task<bool> SetDeletedAsync(string id);
        Task<bool> SetNotDeletedAsync(string id);
    }
}
