using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Shared.Dtos.MappingDtos.StockMovementDtos;

namespace StockManagement.Business.Services.Abstract
{
    public interface IStockMovementService
    {
        Task<IEnumerable<StockMovementGetDto>> GetAllIncludingAsync();
        Task<IEnumerable<StockMovementGetDto>> GetAllIncludingByUserIdAsync(string userId);
        Task<IEnumerable<StockMovementGetDto>> GetAllIncludingByProductIdAsync(int? productId);
        Task<IEnumerable<StockMovementGetDto>> GetAllIncludingRangeAsync(DateTime start, DateTime end);
        Task<IEnumerable<StockMovementGetDto>> GetAllIncludingByAllDataAsync();
        Task<StockMovementGetDto> GetByIdAsync(int? id);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<bool>> DeleteByIdAsync(List<int> ids);
        Task<ServiceResult<bool>> SetActiveAsync(int id);
        Task<ServiceResult<bool>> SetInActiveAsync(int id);
        Task<ServiceResult<bool>> SetDeletedAsync(int id);
        Task<ServiceResult<bool>> SetNotDeletedAsync(int id);
    }
}
