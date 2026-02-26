using System.Linq.Expressions;
using AutoMapper;
using Ganss.Xss;
using StockManagement.Business.Constants.ErrorMessages;
using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Business.Services.Abstract;
using StockManagement.DataAccess.Abstract;
using StockManagement.Domain.Entities;
using StockManagement.Shared.Dtos.MappingDtos.WarehouseDtos;

namespace StockManagement.Business.Services.Concrete
{
    public class WarehouseManager : IWarehouseService
    {
        readonly IWarehouseRepository _warehouseRepository;
        readonly IAzureService _azureService;
        readonly IMapper _mapper;
        private readonly IHtmlSanitizer _htmlSanitizer;
        public WarehouseManager(IWarehouseRepository warehouseRepository, IAzureService azureService, IMapper mapper, IHtmlSanitizer htmlSanitizer)
        {
            _warehouseRepository = warehouseRepository;
            _azureService = azureService;
            _mapper = mapper;
            _htmlSanitizer = htmlSanitizer;
        }

        public async Task<ServiceResult<bool>> CreateAsync(string name, string code, string address, string typeOfWarehouse)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(_htmlSanitizer, nameof(_htmlSanitizer));
                string safeAddress = _htmlSanitizer.Sanitize(address ?? string.Empty);

                var model = new WarehouseCreateDto
                {
                    Name = name,
                    Code = code,
                    Address = safeAddress,
                    TypeOfWarehouse = typeOfWarehouse
                };
                var entity = _mapper.Map<Warehouse>(model);
                var result = await _warehouseRepository.AddAsync(entity);
                if (result == true)
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.AddSuccess };

                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.AddError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while adding the entity.", ex);
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var data = await _warehouseRepository.GetAsync(i => i.Id == id);
                if (data == null)
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.DeleteExists };

                var result = await _warehouseRepository.DeleteAsync(data);
                if (result == true)
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.DeleteSuccess };

                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.DeleteError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while deleting the entity.", ex);
            }
        }

        public async Task<ServiceResult<bool>> DeleteByIdAsync(List<int> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                    throw new ArgumentNullException(nameof(ids), "id list was null or empty");

                var result = await _warehouseRepository.DeleteByIdsAsync(ids.Cast<object>());
                if (result == true)
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.DeleteSuccess };
                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.DeleteError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while bulk deleting entities.", ex);
            }
        }

        public async Task<IEnumerable<WarehouseGetDto>> GetAllIncludingAsync()
        {
            try
            {
                var data = await _warehouseRepository.GetAllIncludeAsync(new Expression<Func<Warehouse, bool>>[]
                {
                    i=>i.IsActive==true,
                    i=>i.IsDeleted==false
                }, null, y => y.Products, y => y.UnitInStocks);
                return _mapper.Map<IEnumerable<WarehouseGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<Warehouse>(i => i.IsActive == true && i.IsDeleted == false, y => y.Products, y => y.UnitInStocks);
                    return _mapper.Map<IEnumerable<WarehouseGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList());
                }
                catch (Exception)
                {
                    return new List<WarehouseGetDto>();
                }
            }
        }

        public async Task<WarehouseGetDto> GetByIdAsync(int? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id), "id was null");

            try
            {
                var data = await _warehouseRepository.GetIncludeAsync(i => i.Id == id, y => y.Products, y => y.UnitInStocks);
                return _mapper.Map<WarehouseGetDto>(data);
            }
            catch (Exception)
            {
                try
                {
                    var data = await _azureService.GetFromAzureWithIncludesAsync<Warehouse>(i => i.Id == id, y => y.UnitInStocks, y => y.Products);
                    return _mapper.Map<WarehouseGetDto>(data);
                }
                catch (Exception ex)
                {
                    throw new Exception("An unexpected error occurred while getting the entity.", ex);
                }
            }
        }

        public async Task<IEnumerable<WarehouseGetDto>> GetAllIncludingByAllDataAsync()
        {
            try
            {
                var data = await _warehouseRepository.GetAllIncludeAsync(new Expression<Func<Warehouse, bool>>[]
                {

                }, null, y => y.Products, y => y.UnitInStocks);
                return _mapper.Map<IEnumerable<WarehouseGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<Warehouse>(i => i.IsActive == true && i.IsDeleted == false, null);
                    return _mapper.Map<IEnumerable<WarehouseGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList());
                }
                catch (Exception)
                {
                    return new List<WarehouseGetDto>();
                }
            }
        }

        public async Task<ServiceResult<bool>> SetActiveAsync(int id)
        {
            try
            {
                var entity = await _warehouseRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsActiveNotFound };
                }

                var result = await _warehouseRepository.SetActiveAsync(id);
                if (result)
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsActiveSuccess };
                return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsInActiveError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting Active the entity.", ex);
            }
        }

        public async Task<ServiceResult<bool>> SetDeletedAsync(int id)
        {
            try
            {
                var entity = await _warehouseRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsDeleteNotFound };
                }

                var result = await _warehouseRepository.SetDeletedAsync(id);
                if (result)
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsDeletedSuccess };
                return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsDeleteError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting Deleted the entity.", ex);
            }
        }

        public async Task<ServiceResult<bool>> SetInActiveAsync(int id)
        {
            try
            {
                var entity = await _warehouseRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsInActiveNotFound };
                }

                var result = await _warehouseRepository.SetInActiveAsync(id);
                if (result)
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsInActiveSuccess };
                return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsInActiveError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting InActive the entity.", ex);
            }
        }

        public async Task<ServiceResult<bool>> SetNotDeletedAsync(int id)
        {
            try
            {
                var entity = await _warehouseRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.NotDeletedNotFound };
                }

                var result = await _warehouseRepository.SetNotDeletedAsync(id);
                if (result)
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.NotDeletedSuccess };
                return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.NotDeleteError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting NotDeleted the entity.", ex);
            }
        }

        public async Task<WarehouseUpdateDto> GetForEditAsync(int id)
        {
            var entity = await _warehouseRepository.GetAsync(x => x.Id == id);
            return _mapper.Map<WarehouseUpdateDto>(entity);
        }

        public async Task<ServiceResult<bool>> UpdateAsync(string name, string code, string address, string typeOfWarehouse, int id)
        {
            try
            {
                if (id < 0)
                    throw new ArgumentNullException(nameof(id), "id was not available");

                var existData = await _warehouseRepository.GetAsync(x => x.Id == id);
                if (existData == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.UpdateNotFound };
                }

                ArgumentNullException.ThrowIfNull(_htmlSanitizer, nameof(_htmlSanitizer));
                string sadeAddress = _htmlSanitizer.Sanitize(address ?? string.Empty);

                var model = new WarehouseUpdateDto
                {
                    Name = name,
                    Code = code,
                    Address = sadeAddress,
                    TypeOfWarehouse = typeOfWarehouse,
                    UpdatedDate = DateTime.UtcNow,
                    Id = id
                };
                _mapper.Map(model, existData);

                var result = await _warehouseRepository.UpdateAsync(existData);
                if (result == true)
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.UpdateSuccess };

                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.UpdateError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while updating the entity.", ex);
            }
        }
    }
}
