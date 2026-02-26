using System.Linq.Expressions;
using AutoMapper;
using StockManagement.Business.Constants.ErrorMessages;
using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Business.Services.Abstract;
using StockManagement.DataAccess.Abstract;
using StockManagement.Domain.Entities;
using StockManagement.Shared.Dtos.MappingDtos.StockMovementDtos;

namespace StockManagement.Business.Services.Concrete
{
    public class StockMovementManager : IStockMovementService
    {
        readonly IStockMovementRepository _stockMovementRepository;
        readonly IAzureService _azureService;
        readonly IMapper _mapper;
        public StockMovementManager(IStockMovementRepository stockMovementRepository, IAzureService azureService, IMapper mapper)
        {
            _stockMovementRepository = stockMovementRepository;
            _azureService = azureService;
            _mapper = mapper;
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var data = await _stockMovementRepository.GetAsync(i => i.Id == id);
                if (data == null)
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.DeleteExists };

                var result = await _stockMovementRepository.DeleteAsync(data);
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

                var result = await _stockMovementRepository.DeleteByIdsAsync(ids.Cast<object>());
                if (result == true)
                {
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.DeleteSuccess };
                }
                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.DeleteError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while bulk deleting entities.", ex);
            }
        }

        public async Task<IEnumerable<StockMovementGetDto>> GetAllIncludingAsync()
        {
            try
            {
                var data = await _stockMovementRepository.GetAllIncludeAsync(new Expression<Func<StockMovement, bool>>[]
                {
                    i=>i.IsActive==true,
                    i=>i.IsDeleted==false
                }, null, y => y.Product, y => y.AppUser);
                return _mapper.Map<IEnumerable<StockMovementGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<StockMovement>(i => i.IsActive == true && i.IsDeleted == false, y => y.Product, y => y.AppUser);
                    return _mapper.Map<IEnumerable<StockMovementGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList());
                }
                catch (Exception)
                {
                    return new List<StockMovementGetDto>();
                }
            }
        }

        public async Task<IEnumerable<StockMovementGetDto>> GetAllIncludingByAllDataAsync()
        {
            try
            {
                var data = await _stockMovementRepository.GetAllIncludeAsync(new Expression<Func<StockMovement, bool>>[]
                {

                }, null, y => y.Product, y => y.AppUser);
                return _mapper.Map<IEnumerable<StockMovementGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<StockMovement>(null, y => y.Product, y => y.AppUser);
                    return _mapper.Map<IEnumerable<StockMovementGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList());
                }
                catch (Exception)
                {
                    return new List<StockMovementGetDto>();
                }
            }
        }

        public async Task<IEnumerable<StockMovementGetDto>> GetAllIncludingByProductIdAsync(int? productId)
        {
            if (productId == null)
                throw new ArgumentNullException(nameof(productId), "productId was null");

            try
            {
                var data = await _stockMovementRepository.GetAllIncludeByIdAsync(productId, "ProductId", new Expression<Func<StockMovement, bool>>[]
                {
                    i=>i.IsActive==true,
                    i=>i.IsDeleted==false
                }, y => y.Product, y => y.AppUser);
                return _mapper.Map<IEnumerable<StockMovementGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<StockMovement>(
                        i => i.ProductId == productId && i.IsActive == true && i.IsDeleted == false,
                        y => y.AppUser, y => y.Product
                    );
                    return _mapper.Map<IEnumerable<StockMovementGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList());
                }
                catch (Exception)
                {
                    return new List<StockMovementGetDto>();
                }
            }
        }

        public async Task<IEnumerable<StockMovementGetDto>> GetAllIncludingByUserIdAsync(string userId)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId), "userId was null");

            try
            {
                var data = await _stockMovementRepository.GetAllIncludeByIdAsync(userId, "AppUserId", new Expression<Func<StockMovement, bool>>[]
                {
                    i=>i.IsActive==true,
                    i=>i.IsDeleted==false
                }, y => y.Product, y => y.AppUser);
                return _mapper.Map<IEnumerable<StockMovementGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<StockMovement>(
                        i => i.AppUserId == userId && i.IsActive == true && i.IsDeleted == false,
                        y => y.AppUser, y => y.Product
                    );
                    return _mapper.Map<IEnumerable<StockMovementGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList());
                }
                catch (Exception)
                {
                    return new List<StockMovementGetDto>();
                }
            }
        }

        public async Task<IEnumerable<StockMovementGetDto>> GetAllIncludingRangeAsync(DateTime start, DateTime end)
        {
            try
            {
                var data = await _stockMovementRepository.GetAllIncludeAsync(new Expression<Func<StockMovement, bool>>[]
                {
                    i=>i.CreatedDate >= start && i.CreatedDate <= end,
                }, null, y => y.AppUser, y => y.Product);
                return _mapper.Map<IEnumerable<StockMovementGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
            }
            catch (Exception)
            {
                var azureData = await _azureService.GetAllFromAzureAsync<StockMovement>(x => x.CreatedDate >= start && x.CreatedDate <= end, y => y.Product, y => y.AppUser);
                return _mapper.Map<IEnumerable<StockMovementGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList());
            }
        }

        public async Task<StockMovementGetDto> GetByIdAsync(int? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id), "id was null");

            try
            {
                var data = await _stockMovementRepository.GetIncludeAsync(i => i.Id == id, y => y.AppUser, y => y.Product);
                return _mapper.Map<StockMovementGetDto>(data);
            }
            catch (Exception)
            {
                try
                {
                    var data = await _azureService.GetFromAzureWithIncludesAsync<StockMovement>(i => i.Id == id, y => y.AppUser, y => y.Product);
                    return _mapper.Map<StockMovementGetDto>(data);
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
                var entity = await _stockMovementRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsActiveNotFound };
                }

                var result = await _stockMovementRepository.SetActiveAsync(id);
                if (result)
                {
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsActiveSuccess };
                }
                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsActiveError };
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
                var entity = await _stockMovementRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsDeleteNotFound };
                }

                var result = await _stockMovementRepository.SetDeletedAsync(id);
                if (result)
                {
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsDeletedSuccess };
                }
                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsDeleteError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while Deleted Active the entity.", ex);
            }
        }

        public async Task<ServiceResult<bool>> SetInActiveAsync(int id)
        {
            try
            {
                var entity = await _stockMovementRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsInActiveNotFound };
                }

                var result = await _stockMovementRepository.SetInActiveAsync(id);
                if (result)
                {
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsInActiveSuccess };
                }
                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsInActiveError };
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
                var entity = await _stockMovementRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.NotDeletedNotFound };
                }

                var result = await _stockMovementRepository.SetNotDeletedAsync(id);
                if (result)
                {
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.NotDeletedSuccess };
                }
                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.NotDeleteError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting NotDeleted the entity.", ex);
            }
        }
    }
}
