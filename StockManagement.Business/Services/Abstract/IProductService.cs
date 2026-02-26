using Microsoft.AspNetCore.Http;
using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Shared.Dtos.MappingDtos.ProductDtos;

namespace StockManagement.Business.Services.Abstract
{
    public interface IProductService
    {
        Task<IEnumerable<ProductGetDto>> GetAllIncludingAsync();
        Task<IEnumerable<ProductGetDto>> GetAllIncludingByCategoryIdAsync(int categoryId);
        Task<IEnumerable<ProductGetDto>> GetAllIncludingByWarehouseIdAsync(int warehouseId);
        Task<IEnumerable<ProductGetDto>> GetAllIncludingByUserIdAsync(string userId);
        Task<IEnumerable<ProductGetDto>> GetAllIncludingByWarningStockAsync();
        Task<IEnumerable<ProductGetDto>> GetAllIncludingByAllDataAsync();
        Task<ProductGetDto> GetByIdAsync(int? id);
        Task<ServiceResult<bool>> CreateAsync(string name, string code, string description, decimal price, int categoryId, int warehouseId, string userId, IFormFile image);
        Task<ProductUpdateDto> GetForEditAsync(int id);
        Task<ServiceResult<bool>> UpdateAsync(string name, string code, string description, decimal price, int categoryId, int warehouseId, string userId, IFormFile image, int id);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<bool>> DeleteByIdAsync(List<int> ids);
        Task<ServiceResult<bool>> SetActiveAsync(int id);
        Task<ServiceResult<bool>> SetInActiveAsync(int id);
        Task<ServiceResult<bool>> SetDeletedAsync(int id);
        Task<ServiceResult<bool>> SetNotDeletedAsync(int id);
    }
}
