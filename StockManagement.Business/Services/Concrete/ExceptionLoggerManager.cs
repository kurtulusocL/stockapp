using AutoMapper;
using StockManagement.Business.Constants.ErrorMessages;
using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Business.Services.Abstract;
using StockManagement.DataAccess.Abstract;
using StockManagement.Domain.Entities;
using StockManagement.Shared.Dtos.MappingDtos.ExceptionLoggerDtos;

namespace StockManagement.Business.Services.Concrete
{
    public class ExceptionLoggerManager : IExceptionLoggerService
    {
        readonly IExceptionLoggerRepository _exceptionLoggerRepository;
        readonly IAzureService _azureService;
        readonly IMapper _mapper;
        public ExceptionLoggerManager(IExceptionLoggerRepository exceptionLoggerRepository, IAzureService azureService, IMapper mapper)
        {
            _exceptionLoggerRepository = exceptionLoggerRepository;
            _azureService = azureService;
            _mapper = mapper;
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var data = await _exceptionLoggerRepository.GetAsync(i => i.Id == id);
                if (data == null)
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.DeleteExists };

                var result = await _exceptionLoggerRepository.DeleteAsync(data);
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

                var result = await _exceptionLoggerRepository.DeleteByIdsAsync(ids.Cast<object>());
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

        public async Task<IEnumerable<ExceptionLoggerGetDto>> GetAllAllDataAsync()
        {
            try
            {
                var data = await _exceptionLoggerRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<ExceptionLoggerGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<ExceptionLogger>(null);
                    return _mapper.Map<IEnumerable<ExceptionLoggerGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList());
                }
                catch (Exception)
                {
                    return new List<ExceptionLoggerGetDto>();
                }
            }
        }

        public async Task<IEnumerable<ExceptionLoggerGetDto>> GetAllAsync()
        {
            try
            {
                var data = await _exceptionLoggerRepository.GetAllAsync(i => i.IsActive == true && i.IsDeleted == false);
                return _mapper.Map<IEnumerable<ExceptionLoggerGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<ExceptionLogger>(i => i.IsActive == true && i.IsDeleted == false);
                    return _mapper.Map<IEnumerable<ExceptionLoggerGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList());
                }
                catch (Exception)
                {
                    return new List<ExceptionLoggerGetDto>();
                }
            }
        }

        public async Task<ExceptionLoggerGetDto> GetByIdAsync(int? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id), "id was null");
            try
            {
                var data = await _exceptionLoggerRepository.GetAsync(i => i.Id == id);
                return _mapper.Map<ExceptionLoggerGetDto>(data);
            }
            catch (Exception)
            {
                try
                {
                    var data = await _azureService.GetFromAzureAsync<ExceptionLogger>(id.Value);
                    return _mapper.Map<ExceptionLoggerGetDto>(data);
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
                var entity = await _exceptionLoggerRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsDeleteNotFound };
                }

                var result = await _exceptionLoggerRepository.SetActiveAsync(id);
                if (result)
                {
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsDeletedSuccess };
                }
                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsDeleteError };
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
                var entity = await _exceptionLoggerRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsDeleteNotFound };
                }

                var result = await _exceptionLoggerRepository.SetDeletedAsync(id);
                if (result)
                {
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsDeletedSuccess };
                }
                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsDeleteError };
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
                var entity = await _exceptionLoggerRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsInActiveNotFound };
                }

                var result = await _exceptionLoggerRepository.SetInActiveAsync(id);
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
                var entity = await _exceptionLoggerRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.NotDeletedNotFound };
                }

                var result = await _exceptionLoggerRepository.SetNotDeletedAsync(id);
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
