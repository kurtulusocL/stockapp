using System.Linq.Expressions;
using System.Security.Claims;
using AutoMapper;
using Ganss.Xss;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using StockManagement.Business.Constants.ErrorMessages;
using StockManagement.Business.Constants.Helpers;
using StockManagement.Business.Constants.Services;
using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Business.Hubs;
using StockManagement.Business.Services.Abstract;
using StockManagement.DataAccess.Abstract;
using StockManagement.Domain.Entities;
using StockManagement.Shared.Dtos.MappingDtos.ProductDtos;

namespace StockManagement.Business.Services.Concrete
{
    public class ProductManager : IProductService
    {
        readonly IProductRepository _productRepository;
        readonly IAzureService _azureService;
        readonly IHttpContextAccessor _httpContextAccessor;
        readonly IMapper _mapper;
        readonly IHubContext<ProductHub> _hubContext;
        readonly ICacheService _cacheService;
        private readonly IHtmlSanitizer _htmlSanitizer;
        public ProductManager(IProductRepository productRepository, IAzureService azureService, IHttpContextAccessor httpContextAccessor, IMapper mapper, IHubContext<ProductHub> hubContext, ICacheService cacheService, IHtmlSanitizer htmlSanitizer)
        {
            _productRepository = productRepository;
            _azureService = azureService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _hubContext = hubContext;
            _cacheService = cacheService;
            _htmlSanitizer = htmlSanitizer;
        }

        public async Task<ServiceResult<bool>> CreateAsync(string name, string code, string description, decimal price, int categoryId, int warehouseId, string userId, IFormFile image)
        {
            try
            {
                userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                    throw new ArgumentNullException(nameof(userId), "userId was null");

                ArgumentNullException.ThrowIfNull(_htmlSanitizer, nameof(_htmlSanitizer));
                string safeDescription = _htmlSanitizer.Sanitize(description ?? string.Empty);

                if (image != null && image.Length > 0)
                {
                    ServiceImageHelper.ImageValidation(image);
                    try
                    {
                        string savedFileName = await ServiceImageHelper.ProductImageResize(image);
                        var model = new ProductCreateDto
                        {
                            Name = name,
                            Code = code,
                            Description = safeDescription,
                            Price = price,
                            CategoryId = categoryId,
                            WarehouseId = warehouseId,
                            AppUserId = userId,
                            ImageUrl = savedFileName
                        };
                        if (model != null)
                        {
                            var entity = _mapper.Map<Product>(model);
                            var result = await _productRepository.AddAsync(entity);
                            if (result)
                            {
                                await ClearProductCache(categoryId: categoryId, warehouseId: warehouseId, userId: userId, id: entity.Id);
                                return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.AddSuccess };
                            }
                            return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.AddError };
                        }
                        return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.AddError };
                    }
                    catch (Exception)
                    {
                        return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.AddError };
                    }
                }
                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.AddError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while adding the entity.", ex);
            }
        }

        public async Task<ProductUpdateDto> GetForEditAsync(int id)
        {
            var entity = await _productRepository.GetAsync(x => x.Id == id);
            return _mapper.Map<ProductUpdateDto>(entity);
        }

        public async Task<ServiceResult<bool>> UpdateAsync(string name, string code, string description, decimal price, int categoryId, int warehouseId, string userId, IFormFile image, int id)
        {
            try
            {
                userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                    throw new ArgumentNullException(nameof(userId), "userId was null");

                ArgumentNullException.ThrowIfNull(_htmlSanitizer, nameof(_htmlSanitizer));
                string safeDescription = _htmlSanitizer.Sanitize(description ?? string.Empty);

                if (image != null && image.Length > 0)
                {
                    ServiceImageHelper.ImageValidation(image);
                    try
                    {
                        string savedFileName = await ServiceImageHelper.ProductImageResize(image);
                        var model = new ProductUpdateDto
                        {
                            Name = name,
                            Code = code,
                            Description = safeDescription,
                            Price = price,
                            CategoryId = categoryId,
                            WarehouseId = warehouseId,
                            AppUserId = userId,
                            ImageUrl = savedFileName,
                            Id = id,
                            UpdatedDate = DateTime.UtcNow
                        };
                        if (model != null)
                        {
                            var entity = _mapper.Map<Product>(model);
                            var result = await _productRepository.UpdateAsync(entity);
                            if (result)
                            {
                                await ClearProductCache(categoryId: categoryId, warehouseId: warehouseId, userId: userId, id: model.Id);
                                return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.UpdateSuccess };
                            }
                            return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.UpdateError };
                        }
                        return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.UpdateError };
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("An unexpected error occurred while updating the entity.", ex);
                    }
                }
                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.UpdateError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while updating the entity.", ex);
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var data = await _productRepository.GetAsync(i => i.Id == id);
                if (data == null)
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.DeleteExists };

                var result = await _productRepository.DeleteAsync(data);
                if (result == true)
                {
                    await ClearProductCache(categoryId: data.CategoryId, warehouseId: data.WarehouseId, userId: data.AppUserId, id: data.Id);
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

                var entities = await _productRepository.GetAllAsync(x => ids.Contains(x.Id));
                if (entities == null || !entities.Any())
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.NotDeletedNotFound };

                var result = await _productRepository.DeleteByIdsAsync(ids.Cast<object>());
                if (result == true)
                {
                    foreach (var entity in entities)
                    {
                        await ClearProductCache(categoryId: entity.CategoryId, warehouseId: entity.WarehouseId, userId: entity.AppUserId, id: entity.Id);
                    }
                    if (_hubContext != null)
                    {
                        await _hubContext.Clients.All.SendAsync("ReceiveProductUpdate");
                    }
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.DeleteSuccess };
                }
                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.DeleteError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while bulk deleting entities.", ex);
            }
        }

        public async Task<IEnumerable<ProductGetDto>> GetAllIncludingAsync()
        {
            string cacheKey = "Products_All";

            try
            {
                var cachedData = _cacheService.Get<IEnumerable<ProductGetDto>>(cacheKey);
                if (cachedData != null)
                {
                    return cachedData;
                }

                var data = await _productRepository.GetAllIncludeAsync(new Expression<Func<Product, bool>>[]
                {
                    i=>i.IsActive==true,
                    i=>i.IsDeleted==false
                }, null, y => y.Category, y => y.Warehouse, y => y.AppUser, y => y.StockMovements, y => y.UnitInStocks);

                var result = data.OrderByDescending(i => i.CreatedDate).ToList();
                var mappedResult = _mapper.Map<IEnumerable<ProductGetDto>>(result);

                if (result.Any())
                {
                    _cacheService.Set(cacheKey, mappedResult, TimeSpan.FromMinutes(60));
                }
                return mappedResult;
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<Product>(i => i.IsActive == true && i.IsDeleted == false, y => y.Category, y => y.Warehouse, y => y.AppUser, y => y.StockMovements, y => y.UnitInStocks);

                    var result = azureData.OrderByDescending(i => i.CreatedDate).ToList();
                    var azureMappedResult = _mapper.Map<IEnumerable<ProductGetDto>>(result);

                    if (result.Any())
                    {
                        _cacheService.Set(cacheKey, azureMappedResult, TimeSpan.FromMinutes(30));
                    }
                    return azureMappedResult;
                }
                catch (Exception)
                {
                    return new List<ProductGetDto>();
                }
            }
        }

        public async Task<IEnumerable<ProductGetDto>> GetAllIncludingByAllDataAsync()
        {
            string cacheKey = "Products_All_Raw_Data";
            try
            {
                var cachedData = _cacheService.Get<IEnumerable<ProductGetDto>>(cacheKey);
                if (cachedData != null) return cachedData;

                var data = await _productRepository.GetAllIncludeAsync(new Expression<Func<Product, bool>>[]
                {

                }, null, y => y.Category, y => y.Warehouse, y => y.AppUser, y => y.StockMovements, y => y.UnitInStocks);

                var result = data.OrderByDescending(i => i.CreatedDate).ToList();
                var mappedResult = _mapper.Map<IEnumerable<ProductGetDto>>(result);

                if (result.Any())
                {
                    _cacheService.Set(cacheKey, mappedResult, TimeSpan.FromMinutes(60));
                }
                return mappedResult;
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<Product>(null, y => y.Category, y => y.Warehouse, y => y.AppUser, y => y.StockMovements, y => y.UnitInStocks);

                    var result = azureData.OrderByDescending(i => i.CreatedDate).ToList();
                    var mappedAzureResult = _mapper.Map<IEnumerable<ProductGetDto>>(result);

                    if (result.Any())
                    {
                        _cacheService.Set(cacheKey, mappedAzureResult, TimeSpan.FromMinutes(30));
                    }
                    return mappedAzureResult;
                }
                catch (Exception)
                {
                    return new List<ProductGetDto>();
                }
            }
        }

        public async Task<IEnumerable<ProductGetDto>> GetAllIncludingByCategoryIdAsync(int categoryId)
        {
            string cacheKey = $"Products_Category_{categoryId}";
            try
            {
                var cachedData = _cacheService.Get<IEnumerable<ProductGetDto>>(cacheKey);
                if (cachedData != null) return cachedData;

                var data = await _productRepository.GetAllIncludeByIdAsync(categoryId, "CategoryId", new Expression<Func<Product, bool>>[]
                {
                    i=>i.IsActive==true,
                    i=>i.IsDeleted==false
                }, y => y.Category, y => y.Warehouse, y => y.AppUser, y => y.StockMovements, y => y.UnitInStocks);

                var result = data.OrderByDescending(i => i.CreatedDate).ToList();
                var mappedResult = _mapper.Map<IEnumerable<ProductGetDto>>(result);

                if (result.Any())
                {
                    _cacheService.Set(cacheKey, mappedResult, TimeSpan.FromMinutes(60));
                }
                return mappedResult;
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<Product>(i => i.IsActive == true && i.IsDeleted == false && i.CategoryId == categoryId, y => y.Category, y => y.Warehouse, y => y.AppUser, y => y.StockMovements, y => y.UnitInStocks);

                    var result = azureData.OrderByDescending(i => i.CreatedDate).ToList();
                    var mappedAzureResult = _mapper.Map<IEnumerable<ProductGetDto>>(result);

                    if (result.Any())
                    {
                        _cacheService.Set(cacheKey, mappedAzureResult, TimeSpan.FromMinutes(30));
                    }
                    return mappedAzureResult;
                }
                catch (Exception)
                {
                    return new List<ProductGetDto>();
                }
            }
        }

        public async Task<IEnumerable<ProductGetDto>> GetAllIncludingByUserIdAsync(string userId)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId), "userId was null");

            string cacheKey = $"Products_User_{userId}";

            try
            {
                var cachedData = _cacheService.Get<IEnumerable<ProductGetDto>>(cacheKey);
                if (cachedData != null) return cachedData;

                var data = await _productRepository.GetAllIncludeByIdAsync(userId, "AppUserId", new Expression<Func<Product, bool>>[]
                {
                    i=>i.IsActive==true,
                    i=>i.IsDeleted==false
                }, y => y.Category, y => y.Warehouse, y => y.AppUser, y => y.StockMovements, y => y.UnitInStocks);

                var result = data.OrderByDescending(i => i.CreatedDate).ToList();
                var mappedResult = _mapper.Map<IEnumerable<ProductGetDto>>(result);

                if (result.Any())
                {
                    _cacheService.Set(cacheKey, mappedResult, TimeSpan.FromMinutes(60));
                }
                return mappedResult;
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<Product>(i => i.IsActive == true && i.IsDeleted == false && i.AppUserId == userId, y => y.Category, y => y.Warehouse, y => y.AppUser, y => y.StockMovements, y => y.UnitInStocks);

                    var result = azureData.OrderByDescending(i => i.CreatedDate).ToList();
                    var mappedAzureResult = _mapper.Map<IEnumerable<ProductGetDto>>(result);

                    if (result.Any())
                    {
                        _cacheService.Set(cacheKey, mappedAzureResult, TimeSpan.FromMinutes(60));
                    }
                    return mappedAzureResult;
                }
                catch (Exception)
                {
                    return new List<ProductGetDto>();
                }
            }
        }

        public async Task<IEnumerable<ProductGetDto>> GetAllIncludingByWarehouseIdAsync(int warehouseId)
        {
            string cacheKey = $"Products_Warehouse_{warehouseId}";

            try
            {
                var cachedData = _cacheService.Get<IEnumerable<ProductGetDto>>(cacheKey);
                if (cachedData != null) return cachedData;

                var data = await _productRepository.GetAllIncludeByIdAsync(warehouseId, "WarehouseId", new Expression<Func<Product, bool>>[]
                {
                    i=>i.IsActive==true,
                    i=>i.IsDeleted==false
                }, y => y.Category, y => y.Warehouse, y => y.AppUser, y => y.StockMovements, y => y.UnitInStocks);

                var result = data.OrderByDescending(i => i.CreatedDate).ToList();
                var mappedResult = _mapper.Map<IEnumerable<ProductGetDto>>(result);

                if (result.Any())
                {
                    _cacheService.Set(cacheKey, mappedResult, TimeSpan.FromMinutes(60));
                }
                return mappedResult;
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<Product>(i => i.IsActive == true && i.IsDeleted == false && i.WarehouseId == warehouseId, y => y.Category, y => y.Warehouse, y => y.AppUser, y => y.StockMovements, y => y.UnitInStocks);

                    var result = azureData.OrderByDescending(i => i.CreatedDate).ToList();
                    var mappedAzureResult = _mapper.Map<IEnumerable<ProductGetDto>>(result);

                    if (result.Any())
                    {
                        _cacheService.Set(cacheKey, mappedAzureResult, TimeSpan.FromMinutes(60));
                    }
                    return mappedAzureResult;
                }
                catch (Exception)
                {
                    return new List<ProductGetDto>();
                }
            }
        }

        public async Task<IEnumerable<ProductGetDto>> GetAllIncludingByWarningStockAsync()
        {
            string cacheKey = "Products_Warning_Stock";

            try
            {
                var cachedData = _cacheService.Get<IEnumerable<ProductGetDto>>(cacheKey);
                if (cachedData != null) return cachedData;

                var data = await _productRepository.GetAllIncludeAsync(new Expression<Func<Product, bool>>[]
                {
                    i => i.IsActive == true,
                    i => i.IsDeleted == false
                }, null, y => y.Category, y => y.Warehouse, y => y.AppUser, y => y.StockMovements, y => y.UnitInStocks);

                var result = data.Where(p => p.UnitInStocks != null && p.UnitInStocks.Sum(s => s.Quantity) < 15).OrderByDescending(i => i.CreatedDate).ToList();
                var mappedResult = _mapper.Map<IEnumerable<ProductGetDto>>(result);

                if (result.Any())
                {
                    _cacheService.Set(cacheKey, mappedResult, TimeSpan.FromMinutes(30));
                }
                return mappedResult;
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<Product>(i => i.IsActive == true && i.IsDeleted == false,
                        y => y.Category, y => y.Warehouse, y => y.AppUser, y => y.StockMovements, y => y.UnitInStocks);

                    var result = azureData.Where(p => p.UnitInStocks != null && p.UnitInStocks.Sum(s => s.Quantity) < 15).OrderByDescending(i => i.CreatedDate).ToList();
                    var mappedAzureResult = _mapper.Map<IEnumerable<ProductGetDto>>(result);

                    if (result.Any())
                    {
                        _cacheService.Set(cacheKey, mappedAzureResult, TimeSpan.FromMinutes(30));
                    }
                    return mappedAzureResult;
                }
                catch (Exception)
                {
                    return new List<ProductGetDto>();
                }
            }
        }

        public async Task<ProductGetDto> GetByIdAsync(int? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id), "id was null");

            string cacheKey = $"Product_Detail_{id}";
            try
            {
                var cachedProduct = _cacheService.Get<ProductGetDto>(cacheKey);
                if (cachedProduct != null) return cachedProduct;

                var data = await _productRepository.GetIncludeAsync(i => i.Id == id, y => y.Category, y => y.Warehouse, y => y.AppUser, y => y.StockMovements, y => y.UnitInStocks);
                if (data != null)
                {
                    var mappedData = _mapper.Map<ProductGetDto>(data);
                    _cacheService.Set(cacheKey, mappedData, TimeSpan.FromMinutes(60));
                    return mappedData;
                }
                return null;
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetFromAzureWithIncludesAsync<Product>(i => i.Id == id, y => y.Category, y => y.Warehouse, y => y.AppUser, y => y.StockMovements, y => y.UnitInStocks);
                    if (azureData != null)
                    {
                        var mappedAzureData = _mapper.Map<ProductGetDto>(azureData);
                        _cacheService.Set(cacheKey, mappedAzureData, TimeSpan.FromMinutes(60));
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
                var entity = await _productRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsActiveNotFound };
                }

                var result = await _productRepository.SetActiveAsync(id);
                if (result)
                {
                    await ClearProductCache(categoryId: entity.CategoryId, warehouseId: entity.WarehouseId, userId: entity.AppUserId, id: id);
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsActiveSuccess };
                }
                return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsActiveError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting InActive the entity.", ex);
            }
        }

        public async Task<ServiceResult<bool>> SetDeletedAsync(int id)
        {
            try
            {
                var entity = await _productRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsDeleteNotFound };
                }

                var result = await _productRepository.SetDeletedAsync(id);
                if (result)
                {
                    await ClearProductCache(categoryId: entity.CategoryId, warehouseId: entity.WarehouseId, userId: entity.AppUserId, id: id);
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsDeletedSuccess };
                }
                return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsDeleteError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting InActive the entity.", ex);
            }
        }

        public async Task<ServiceResult<bool>> SetInActiveAsync(int id)
        {
            try
            {
                var entity = await _productRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsInActiveNotFound };
                }

                var result = await _productRepository.SetInActiveAsync(id);
                if (result)
                {
                    await ClearProductCache(categoryId: entity.CategoryId, warehouseId: entity.WarehouseId, userId: entity.AppUserId, id: id);
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
                var entity = await _productRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.NotDeletedNotFound };
                }

                var result = await _productRepository.SetNotDeletedAsync(id);
                if (result)
                {
                    await ClearProductCache(categoryId: entity.CategoryId, warehouseId: entity.WarehouseId, userId: entity.AppUserId, id: id);
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.NotDeletedSuccess };
                }
                return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.NotDeleteError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting NotDeleted the entity.", ex);
            }
        }

        private async Task ClearProductCache(int? categoryId = null, int? warehouseId = null, string userId = null, int? id = null)
        {
            _cacheService.Remove("Products_All");
            _cacheService.Remove("Products_All_Raw_Data");
            _cacheService.Remove("Products_Warning_Stock");

            if (categoryId.HasValue)
                _cacheService.Remove($"Products_Category_{categoryId}");

            if (warehouseId.HasValue)
                _cacheService.Remove($"Products_Warehouse_{warehouseId}");

            if (!string.IsNullOrEmpty(userId))
                _cacheService.Remove($"Products_User_{userId}");

            if (id.HasValue)
                _cacheService.Remove($"Product_Detail_{id}");

            if (_hubContext != null)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveProductUpdate");
            }
        }
    }
}
