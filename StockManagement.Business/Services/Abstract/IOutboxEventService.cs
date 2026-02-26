using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Shared.Dtos.MappingDtos.OutboxEventDtos;

namespace StockManagement.Business.Services.Abstract
{
    public interface IOutboxEventService
    {
        Task<IEnumerable<OutboxEventGetDto>> GetAllAsync();
        Task<IEnumerable<OutboxEventGetDto>> GetAllBySuccessfullProcessAsync();
        Task<IEnumerable<OutboxEventGetDto>> GetAllByErrorProcessAsync();
        Task<IEnumerable<OutboxEventGetDto>> GetAllAllDataAsync();
        Task<OutboxEventGetDto> GetByIdAsync(int? id);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<bool>> DeleteByIdAsync(List<int> ids);
        Task<ServiceResult<bool>> SetActiveAsync(int id);
        Task<ServiceResult<bool>> SetInActiveAsync(int id);
        Task<ServiceResult<bool>> SetDeletedAsync(int id);
        Task<ServiceResult<bool>> SetNotDeletedAsync(int id);
    }
}
