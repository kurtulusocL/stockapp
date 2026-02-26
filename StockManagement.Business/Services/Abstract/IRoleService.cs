using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Shared.Dtos.MappingDtos.AppRoleDtos;

namespace StockManagement.Business.Services.Abstract
{
    public interface IRoleService
    {
        Task<IEnumerable<AppRoleGetDto>> GetAllIncludingAsync();
        Task<IEnumerable<AppRoleGetDto>> GetAllIncludingAllDataAsync();
        Task<AppRoleGetDto> GetByIdAsync(string? id);
        Task<ServiceResult<bool>> CrateAsync(AppRoleCreateDto dto);
        Task<AppRoleUpdateDto> GetForEditAsync(string id);
        Task<ServiceResult<bool>> UpdateAsync(AppRoleUpdateDto dto);
        Task<ServiceResult<bool>> DeleteAsync(string id);
        Task<ServiceResult<bool>> DeleteByIdAsync(List<string> ids);
        Task<ServiceResult<bool>> SetActiveAsync(string id);
        Task<ServiceResult<bool>> SetInActiveAsync(string id);
        Task<ServiceResult<bool>> SetDeletedAsync(string id);
        Task<ServiceResult<bool>> SetNotDeletedAsync(string id);
    }
}
