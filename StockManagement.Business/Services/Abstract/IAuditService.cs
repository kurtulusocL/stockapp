using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Shared.Dtos.MappingDtos.AuditDtos;

namespace StockManagement.Business.Services.Abstract
{
    public interface IAuditService
    {
        Task<IEnumerable<AuditGetDto>> GetAllIncludingAsync();
        Task<IEnumerable<AuditGetDto>> GetAllIncludingByUserIdAsync(string? userId);
        Task<IEnumerable<AuditGetDto>> GetAllIncludingAllDataAsync();
        Task<AuditGetDto> GetByIdAsync(int? id);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<bool>> DeleteAllByIdAsync(List<int> ids);
        Task<ServiceResult<bool>> SetActiveAsync(int id);
        Task<ServiceResult<bool>> SetInActiveAsync(int id);
        Task<ServiceResult<bool>> SetDeletedAsync(int id);
        Task<ServiceResult<bool>> SetNotDeletedAsync(int id);
    }
}
