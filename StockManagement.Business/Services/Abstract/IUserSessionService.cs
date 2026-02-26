using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Shared.Dtos.MappingDtos.UserSessionDtos;

namespace StockManagement.Business.Services.Abstract
{
    public interface IUserSessionService
    {
        Task<IEnumerable<UserSessionGetDto>> GetAllIncludingAsync();
        Task<IEnumerable<UserSessionGetDto>> GetAllIncludingByUserIdAsync(string userId);
        Task<IEnumerable<UserSessionGetDto>> GetAllIncludingByLoginDateAsync();
        Task<IEnumerable<UserSessionGetDto>> GetAllIncludingByOnlineAsync();
        Task<IEnumerable<UserSessionGetDto>> GetAllIncludingByOfflineAsync();
        Task<IEnumerable<UserSessionGetDto>> GetAllIncludingByAllDataAsync();
        Task<UserSessionGetDto> GetByIdAsync(int? id);
        Task<ServiceResult<bool>> CreateAsync(string username, DateTime loginDate, string userId);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<bool>> DeleteByIdAsync(List<int> ids);
        Task<ServiceResult<bool>> SetActiveAsync(int id);
        Task<ServiceResult<bool>> SetInActiveAsync(int id);
        Task<ServiceResult<bool>> SetDeletedAsync(int id);
        Task<ServiceResult<bool>> SetNotDeletedAsync(int id);
    }
}
