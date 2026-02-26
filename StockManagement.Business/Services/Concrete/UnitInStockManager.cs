using System.Linq.Expressions;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using StockManagement.Business.Constants.ErrorMessages;
using StockManagement.Business.Constants.Services;
using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Business.Hubs;
using StockManagement.Business.Services.Abstract;
using StockManagement.DataAccess.Abstract;
using StockManagement.Domain.Entities;
using StockManagement.Shared.Dtos.MappingDtos.UnitInStockStos;

namespace StockManagement.Business.Services.Concrete
{
    public class UnitInstockManager : IUnitInStockService
    {
        readonly IUnitInStockRepository _unitInStockRepository;
        readonly IAzureService _azureService;
        readonly IHttpContextAccessor _httpContextAccessor;
        readonly IMapper _mapper;
        readonly IHubContext<StockHub> _hubContext;
        readonly ICacheService _cacheService;

        public UnitInstockManager(IUnitInStockRepository unitInStockRepository, IAzureService azureService, IHttpContextAccessor httpContextAccessor, IMapper mapper, IHubContext<StockHub> hubContext, ICacheService cacheService)
        {
            _unitInStockRepository = unitInStockRepository;
            _azureService = azureService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _hubContext = hubContext;
            _cacheService = cacheService;
        }

        public async Task<ServiceResult<bool>> CreateAsync(int quantity, string code, int? productId, int warehouseId, string appUserId)
        {
            try
            {
                appUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (appUserId == null)
                    throw new ArgumentNullException(nameof(appUserId), "userId was null");

                if (productId == null)
                    throw new ArgumentNullException(nameof(productId), "productId was null");

                var model = new UnitInStockCreateDto
                {
                    Quantity = quantity,
                    Code = code,
                    ProductId = productId,
                    WarehouseId = warehouseId,
                    AppUserId = appUserId
                };

                if (model != null)
                {
                    var entity = _mapper.Map<UnitInStock>(model);
                    var result = await _unitInStockRepository.AddAsync(entity);
                    if (result)
                    {
                        await ClearStockCache(warehouseId: warehouseId, userId: appUserId, productId: productId.Value);
                        return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.AddSuccess };
                    }
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.AddError };
                }
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
                var data = await _unitInStockRepository.GetAsync(i => i.Id == id);
                if (data == null)
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.DeleteExists };

                var result = await _unitInStockRepository.DeleteAsync(data);
                if (result == true)
                {
                    await ClearStockCache(warehouseId: data.WarehouseId, userId: data.AppUserId, productId: data.ProductId, id: id);
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.DeleteSuccess };
                }

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

                var entities = await _unitInStockRepository.GetAllAsync(x => ids.Contains(x.Id));

                if (entities == null || !entities.Any())
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.NotDeletedNotFound };

                var result = await _unitInStockRepository.DeleteByIdsAsync(ids.Cast<object>());
                if (result == true)
                {
                    foreach (var entity in entities)
                    {
                        await ClearStockCache(warehouseId: entity.WarehouseId, userId: entity.AppUserId, productId: entity.ProductId, id: entity.Id);
                    }
                    if (_hubContext != null)
                    {
                        await _hubContext.Clients.All.SendAsync("ReceiveStockUpdate");
                    }
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.DeleteSuccess };
                }
                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.DeleteError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while bulk deleting stocks.", ex);
            }
        }

        public async Task<IEnumerable<UnitInStockGetDto>> GetAllIncludingAsync()
        {
            string key = "Stocks_All";

            try
            {
                var cachedData = _cacheService.Get<IEnumerable<UnitInStockGetDto>>(key);
                if (cachedData != null) return cachedData;

                var data = await _unitInStockRepository.GetAllIncludeAsync(new Expression<Func<UnitInStock, bool>>[]
                {
                    i=>i.IsActive==true,
                    i=>i.IsDeleted==false
                }, null, y => y.Product, y => y.Warehouse, y => y.AppUser);

                var result = data.OrderByDescending(i => i.CreatedDate).ToList();
                var mappedResult = _mapper.Map<IEnumerable<UnitInStockGetDto>>(result);

                if (result.Any())
                {
                    _cacheService.Set(key, mappedResult, TimeSpan.FromMinutes(60));
                }
                return mappedResult;
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<UnitInStock>(i => i.IsActive == true && i.IsDeleted == false, y => y.Product, y => y.Warehouse, y => y.AppUser);

                    var azureResult = azureData.OrderByDescending(i => i.CreatedDate).ToList();
                    var azureMappedResult = _mapper.Map<IEnumerable<UnitInStockGetDto>>(azureResult);

                    if (azureResult.Any())
                    {
                        _cacheService.Set(key, azureMappedResult, TimeSpan.FromMinutes(60));
                    }
                    return azureMappedResult;
                }
                catch (Exception)
                {
                    return new List<UnitInStockGetDto>();
                }
            }
        }

        public async Task<IEnumerable<UnitInStockGetDto>> GetAllIncludingByAllDataAsync()
        {
            string key = "Stocks_AllData";

            if (_cacheService.Any(key))
                return _cacheService.Get<IEnumerable<UnitInStockGetDto>>(key);

            try
            {
                var data = await _unitInStockRepository.GetAllIncludeAsync(new Expression<Func<UnitInStock, bool>>[]
                {

                }, null, y => y.Product, y => y.Warehouse, y => y.AppUser);

                var result = data.OrderByDescending(i => i.CreatedDate).ToList();
                var mappedResult = _mapper.Map<IEnumerable<UnitInStockGetDto>>(result);

                if (result.Any())
                {
                    _cacheService.Set(key, mappedResult, TimeSpan.FromMinutes(60));
                }
                return mappedResult;
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<UnitInStock>(i => i.IsActive == true && i.IsDeleted == false, null);

                    var azureResult = azureData.OrderByDescending(i => i.CreatedDate).ToList();
                    var azureMappedResult = _mapper.Map<IEnumerable<UnitInStockGetDto>>(azureResult);

                    if (azureResult.Any())
                    {
                        _cacheService.Set(key, azureMappedResult, TimeSpan.FromMinutes(60));
                    }
                    return azureMappedResult;
                }
                catch (Exception)
                {
                    return new List<UnitInStockGetDto>();
                }
            }
        }

        public async Task<IEnumerable<UnitInStockGetDto>> GetAllIncludingByProductIdAsync(int? productId)
        {
            if (productId == null)
                throw new ArgumentNullException(nameof(productId), "productId was null");

            if (productId == null) throw new ArgumentNullException(nameof(productId));
            string key = $"Stocks_Prod_{productId}";

            try
            {
                var cachedData = _cacheService.Get<IEnumerable<UnitInStockGetDto>>(key);
                if (cachedData != null) return cachedData;

                var data = await _unitInStockRepository.GetAllIncludeByIdAsync(productId, "ProductId", new Expression<Func<UnitInStock, bool>>[]
                {
                    i => i.IsActive == true,
                    i => i.IsDeleted == false
                }, y => y.Product, y => y.Warehouse, y => y.AppUser);

                var result = data.OrderByDescending(i => i.CreatedDate).ToList();
                var mappedResult = _mapper.Map<IEnumerable<UnitInStockGetDto>>(result);

                if (result.Any())
                {
                    _cacheService.Set(key, mappedResult, TimeSpan.FromMinutes(60));
                }
                return mappedResult;
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<UnitInStock>(
                        i => i.IsActive == true && i.IsDeleted == false && i.ProductId == productId,
                        y => y.Product, y => y.Warehouse, y => y.AppUser);

                    var azureResult = azureData.OrderByDescending(i => i.CreatedDate).ToList();
                    var azureMappedResult = _mapper.Map<IEnumerable<UnitInStockGetDto>>(azureResult);

                    if (azureResult.Any())
                    {
                        _cacheService.Set(key, azureMappedResult, TimeSpan.FromMinutes(60));
                    }
                    return azureMappedResult;
                }
                catch (Exception)
                {
                    return new List<UnitInStockGetDto>();
                }
            }
        }

        public async Task<IEnumerable<UnitInStockGetDto>> GetAllIncludingByUserIdAsync(string userId)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId), "userId was null");

            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));
            string key = $"Stocks_User_{userId}";

            try
            {
                var cachedData = _cacheService.Get<IEnumerable<UnitInStockGetDto>>(key);
                if (cachedData != null) return cachedData;

                var data = await _unitInStockRepository.GetAllIncludeByIdAsync(userId, "AppUserId", new Expression<Func<UnitInStock, bool>>[]
                {
                    i=>i.IsActive==true,
                    i=>i.IsDeleted==false
                }, y => y.Product, y => y.Warehouse, y => y.AppUser);

                var result = data.OrderByDescending(i => i.CreatedDate).ToList();
                var mappedResult = _mapper.Map<IEnumerable<UnitInStockGetDto>>(result);

                if (result.Any())
                {
                    _cacheService.Set(key, mappedResult, TimeSpan.FromMinutes(60));
                }
                return mappedResult;
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<UnitInStock>(i => i.IsActive == true && i.IsDeleted == false && i.AppUserId == userId,
                 y => y.Product, y => y.Warehouse, y => y.AppUser);

                    var azureResult = azureData.OrderByDescending(i => i.CreatedDate).ToList();
                    var azureMappedResult = _mapper.Map<IEnumerable<UnitInStockGetDto>>(azureResult);

                    if (azureResult.Any())
                    {
                        _cacheService.Set(key, azureMappedResult, TimeSpan.FromMinutes(60));
                    }
                    return azureMappedResult;
                }
                catch (Exception)
                {
                    return new List<UnitInStockGetDto>();
                }
            }
        }

        public async Task<IEnumerable<UnitInStockGetDto>> GetAllIncludingByWarehouseIdAsync(int warehouseId)
        {
            if (warehouseId <= 0)
                return new List<UnitInStockGetDto>();

            string key = $"Stocks_Warehouse_{warehouseId}";

            try
            {
                var cachedData = _cacheService.Get<IEnumerable<UnitInStockGetDto>>(key);
                if (cachedData != null) return cachedData;

                var data = await _unitInStockRepository.GetAllIncludeByIdAsync(warehouseId, "WarehouseId", new Expression<Func<UnitInStock, bool>>[]
                {
                    i=>i.IsActive==true,
                    i=>i.IsDeleted==false
                }, y => y.Product, y => y.Warehouse, y => y.AppUser);

                var result = data.OrderByDescending(i => i.CreatedDate).ToList();
                var mappedResult = _mapper.Map<IEnumerable<UnitInStockGetDto>>(result);

                if (result.Any())
                {
                    _cacheService.Set(key, mappedResult, TimeSpan.FromMinutes(15));
                }
                return mappedResult;
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<UnitInStock>(i => i.IsActive == true && i.IsDeleted == false && i.WarehouseId == warehouseId,
                    y => y.Product, y => y.Warehouse, y => y.AppUser);

                    var azureResult = azureData.OrderByDescending(i => i.CreatedDate).ToList();
                    var azureMappedResult = _mapper.Map<IEnumerable<UnitInStockGetDto>>(azureResult);

                    if (azureResult.Any())
                    {
                        _cacheService.Set(key, azureMappedResult, TimeSpan.FromMinutes(15));
                    }
                    return azureMappedResult;
                }
                catch (Exception)
                {
                    return new List<UnitInStockGetDto>();
                }
            }
        }

        public UnitInStockGetDto GetById(int? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id), "id was null");

            string key = $"Stock_Detail_{id}";
            if (_cacheService.Any(key))
                return _cacheService.Get<UnitInStockGetDto>(key);

            try
            {
                var data = _unitInStockRepository.GetInclude(i => i.Id == id, y => y.Product, y => y.Warehouse, y => y.AppUser);
                if (data != null)
                {
                    var mappedData = _mapper.Map<UnitInStockGetDto>(data);
                    _cacheService.Set(key, mappedData, TimeSpan.FromMinutes(20));
                    return mappedData;
                }
                return null;
            }
            catch (Exception)
            {
                try
                {
                    var azureData = _azureService.GetFromAzureWithIncludes<UnitInStock>(i => i.Id == id, y => y.Product, y => y.Warehouse, y => y.AppUser);
                    if (azureData != null)
                    {
                        var mappedAzureData = _mapper.Map<UnitInStockGetDto>(azureData);
                        _cacheService.Set(key, mappedAzureData, TimeSpan.FromMinutes(20));
                        return mappedAzureData;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    throw new Exception("An unexpected error occurred while getting the entity.", ex);
                }
            }
        }

        public async Task<UnitInStockGetDto> GetByIdAsync(int? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id), "id was null");

            string key = $"Stock_Detail_{id}";
            try
            {
                var cachedData = _cacheService.Get<UnitInStockGetDto>(key);
                if (cachedData != null) return cachedData;

                var data = await _unitInStockRepository.GetIncludeAsync(i => i.Id == id, y => y.Product, y => y.Warehouse, y => y.AppUser);
                if (data != null)
                {
                    var mappedData = _mapper.Map<UnitInStockGetDto>(data);
                    _cacheService.Set(key, mappedData, TimeSpan.FromMinutes(60));
                    return mappedData;
                }
                return null;
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetFromAzureWithIncludesAsync<UnitInStock>(i => i.Id == id, y => y.Product, y => y.Warehouse, y => y.AppUser);
                    if (azureData != null)
                    {
                        var mappedAzureData = _mapper.Map<UnitInStockGetDto>(azureData);
                        _cacheService.Set(key, mappedAzureData, TimeSpan.FromMinutes(60));
                        return mappedAzureData;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    throw new Exception("An unexpected error occurred while getting the entity.", ex);
                }
            }
        }

        public async Task<ServiceResult<bool>> SetActiveAsync(int id)
        {
            try
            {
                var entity = await _unitInStockRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsActiveNotFound };
                }

                var result = await _unitInStockRepository.SetActiveAsync(id);
                if (result)
                {
                    await ClearStockCache(entity.WarehouseId, entity.AppUserId, entity.ProductId, id);
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsActiveSuccess };
                }
                return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsActiveError };
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
                var entity = await _unitInStockRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsDeleteNotFound };
                }

                var result = await _unitInStockRepository.SetDeletedAsync(id);
                if (result)
                {
                    await ClearStockCache(entity.WarehouseId, entity.AppUserId, entity.ProductId, id);
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsDeletedSuccess };
                }
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
                var entity = await _unitInStockRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsInActiveNotFound };
                }

                var result = await _unitInStockRepository.SetInActiveAsync(id);
                if (result)
                {
                    await ClearStockCache(entity.WarehouseId, entity.AppUserId, entity.ProductId, id);
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsInActiveSuccess };
                }
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
                var entity = await _unitInStockRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.NotDeletedNotFound };
                }

                var result = await _unitInStockRepository.SetNotDeletedAsync(id);
                if (result)
                {
                    await ClearStockCache(entity.WarehouseId, entity.AppUserId, entity.ProductId, id);
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.NotDeletedSuccess };
                }
                return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.NotDeleteError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting NotDeleted the entity.", ex);
            }
        }

        public async Task<UnitInStockUpdateDto> GetForEditAsync(int id)
        {
            var entity = await _unitInStockRepository.GetAsync(x => x.Id == id);
            return _mapper.Map<UnitInStockUpdateDto>(entity);
        }
        public async Task<ServiceResult<bool>> UpdateAsync(int quantity, string code, int? productId, int warehouseId, string appUserId, int id)
        {
            try
            {
                appUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (appUserId == null)
                    throw new ArgumentNullException(nameof(appUserId), "userId was null");

                if (productId == null)
                    throw new ArgumentNullException(nameof(productId), "productId was null");

                var existData = await _unitInStockRepository.GetAsync(x => x.Id == id, asNoTracking: true);
                if (existData == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.UpdateNotFound };
                }
                
                var model = new UnitInStockUpdateDto
                {
                    Quantity = quantity,
                    Code = code,
                    ProductId = productId,
                    WarehouseId = warehouseId,
                    AppUserId = appUserId,
                    Id = id,
                    UpdatedDate = DateTime.UtcNow
                };
                
                if (model != null)
                {
                    _mapper.Map(model, existData);
                    //var entity = _mapper.Map<UnitInStock>(model, existData);
                    var result = await _unitInStockRepository.UpdateAsync(existData);
                    if (result)
                    {
                        await ClearStockCache(warehouseId: warehouseId, userId: appUserId, productId: productId.Value, id: id);
                        return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.UpdateSuccess };
                    }
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.UpdateError };
                }
                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.UpdateError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while adding the entity.", ex);
            }
        }
        private async Task ClearStockCache(int? warehouseId = null, string userId = null, int? productId = null, int? id = null)
        {
            _cacheService.Remove("Stocks_All");
            _cacheService.Remove("Stocks_AllData");

            _cacheService.Remove("Products_Warning_Stock");

            if (productId.HasValue)
                _cacheService.Remove($"Stocks_Prod_{productId}");

            if (!string.IsNullOrEmpty(userId))
                _cacheService.Remove($"Stocks_User_{userId}");

            if (warehouseId.HasValue)
                _cacheService.Remove($"Stocks_Warehouse_{warehouseId}");

            if (id.HasValue)
                _cacheService.Remove($"Stock_Detail_{id}");

            if (_hubContext != null)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveStockUpdate");
            }
        }
    }
}
