using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Shared.Dtos.MappingDtos.UnitInStockStos;

namespace StockManagement.Business.Services.Abstract
{
    public interface IUnitInStockService
    {
        Task<IEnumerable<UnitInStockGetDto>> GetAllIncludingAsync(); 
        Task<IEnumerable<UnitInStockGetDto>> GetAllIncludingByProductIdAsync(int? productId);
        Task<IEnumerable<UnitInStockGetDto>> GetAllIncludingByWarehouseIdAsync(int warehouseId);
        Task<IEnumerable<UnitInStockGetDto>> GetAllIncludingByUserIdAsync(string userId);
        Task<IEnumerable<UnitInStockGetDto>> GetAllIncludingByAllDataAsync();
        Task<UnitInStockGetDto> GetByIdAsync(int? id);
        Task<ServiceResult<bool>> CreateAsync(int quantity, string code, int? productId, int warehouseId, string appUserId);
        Task<UnitInStockUpdateDto> GetForEditAsync(int id);
        Task<ServiceResult<bool>> UpdateAsync(int quantity, string code, int? productId, int warehouseId, string appUserId, int id);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<bool>> DeleteByIdAsync(List<int> ids);
        Task<ServiceResult<bool>> SetActiveAsync(int id);
        Task<ServiceResult<bool>> SetInActiveAsync(int id);
        Task<ServiceResult<bool>> SetDeletedAsync(int id);
        Task<ServiceResult<bool>> SetNotDeletedAsync(int id);
        UnitInStockGetDto GetById(int? id);
    }
}
