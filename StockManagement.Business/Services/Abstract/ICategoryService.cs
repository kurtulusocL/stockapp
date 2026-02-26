using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Shared.Dtos.MappingDtos.CategoryDtos;

namespace StockManagement.Business.Services.Abstract
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryGetDto>> GetAllIncludingAsync();
        Task<IEnumerable<CategoryGetDto>> GetAllIncludingAllDataAsync();
        Task<CategoryGetDto> GetByIdAsync(int? id);
        Task<ServiceResult<bool>> CrateAsync(CategoryCreateDto dto);
        Task<CategoryUpdateDto> GetForEditAsync(int id);
        Task<ServiceResult<bool>> UpdateAsync(CategoryUpdateDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<bool>>  DeleteByIdAsync(List<int> ids);
        Task<ServiceResult<bool>> SetActiveAsync(int id);
        Task<ServiceResult<bool>> SetInActiveAsync(int id);
        Task<ServiceResult<bool>> SetDeletedAsync(int id);
        Task<ServiceResult<bool>> SetNotDeletedAsync(int id);
    }
}
