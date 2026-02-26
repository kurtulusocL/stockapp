using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Shared.Dtos.MappingDtos.ExceptionLoggerDtos;

namespace StockManagement.Business.Services.Abstract
{
    public interface IExceptionLoggerService
    {
        Task<IEnumerable<ExceptionLoggerGetDto>> GetAllAsync();
        Task<IEnumerable<ExceptionLoggerGetDto>> GetAllAllDataAsync();
        Task<ExceptionLoggerGetDto> GetByIdAsync(int? id);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<bool>> DeleteByIdAsync(List<int> ids);
        Task<ServiceResult<bool>> SetActiveAsync(int id);
        Task<ServiceResult<bool>> SetInActiveAsync(int id);
        Task<ServiceResult<bool>> SetDeletedAsync(int id);
        Task<ServiceResult<bool>> SetNotDeletedAsync(int id);
    }
}
