using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Shared.Dtos.MappingDtos.WarehouseDtos;

namespace StockManagement.Business.Services.Abstract
{
    public interface IWarehouseService
    {
        Task<IEnumerable<WarehouseGetDto>> GetAllIncludingAsync();
        Task<IEnumerable<WarehouseGetDto>> GetAllIncludingByAllDataAsync();
        Task<WarehouseGetDto> GetByIdAsync(int? id);
        Task<ServiceResult<bool>> CreateAsync(string name, string code, string address, string typeOfWarehouse);
        Task<WarehouseUpdateDto> GetForEditAsync(int id);
        Task<ServiceResult<bool>> UpdateAsync(string name, string code, string address, string typeOfWarehouse, int id);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<bool>> DeleteByIdAsync(List<int> ids);
        Task<ServiceResult<bool>> SetActiveAsync(int id);
        Task<ServiceResult<bool>> SetInActiveAsync(int id);
        Task<ServiceResult<bool>> SetDeletedAsync(int id);
        Task<ServiceResult<bool>> SetNotDeletedAsync(int id);
    }
}
