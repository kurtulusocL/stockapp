using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Shared.Dtos.MappingDtos.AppUserDtos;

namespace StockManagement.Business.Services.Abstract
{
    public interface IUserSevice
    {
        Task<IEnumerable<AppUserGetDto>> GetAllIncludingAsync();
        Task<IEnumerable<AppUserGetDto>> GetAllIncludingByActiveLoginCodeAsync();
        Task<IEnumerable<AppUserGetDto>> GetAllIncludingByInActiveLoginCodeAsync();
        Task<IEnumerable<AppUserGetDto>> GetAllIncludingByAllDataAsync();
        Task<AppUserGetDto> GetByIdAsync(string id);
        Task<ServiceResult<bool>> DeleteAsync(string id);
        Task<ServiceResult<bool>> DeleteByIdAsync(List<string> ids);
        Task<ServiceResult<bool>> SetActiveLoginConfirmCodeAsync(string id);
        Task<ServiceResult<bool>> SetInActiveLoginConfirmCodeAsync(string id);
        Task<ServiceResult<bool>> SetActiveAsync(string id);
        Task<ServiceResult<bool>> SetInActiveAsync(string id);
        Task<ServiceResult<bool>> SetDeletedAsync(string id);
        Task<ServiceResult<bool>> SetNotDeletedAsync(string id);
        AppUserGetDto GetById(string userId);
    }
}
