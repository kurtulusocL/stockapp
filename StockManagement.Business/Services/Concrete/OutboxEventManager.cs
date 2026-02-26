using AutoMapper;
using StockManagement.Business.Constants.ErrorMessages;
using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Business.Services.Abstract;
using StockManagement.DataAccess.Abstract;
using StockManagement.Domain.Entities;
using StockManagement.Shared.Dtos.MappingDtos.OutboxEventDtos;

namespace StockManagement.Business.Services.Concrete
{
    public class OutboxEventManager : IOutboxEventService
    {
        readonly IOutboxEventRepository _outboxEventRepository;
        readonly IAzureService _azureService;
        readonly IMapper _mapper;
        public OutboxEventManager(IOutboxEventRepository outboxEventRepository, IAzureService azureService, IMapper mapper)
        {
            _outboxEventRepository = outboxEventRepository;
            _azureService = azureService;
            _mapper = mapper;
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var data = await _outboxEventRepository.GetAsync(i => i.Id == id);
                if (data == null)
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.DeleteExists };

                var result = await _outboxEventRepository.DeleteAsync(data);
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

                var result = await _outboxEventRepository.DeleteByIdsAsync(ids.Cast<object>());
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

        public async Task<IEnumerable<OutboxEventGetDto>> GetAllAllDataAsync()
        {
            try
            {
                var data = await _outboxEventRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<OutboxEventGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<OutboxEvent>(null);
                    return _mapper.Map<IEnumerable<OutboxEventGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList());
                }
                catch (Exception)
                {
                    return new List<OutboxEventGetDto>();
                }
            }
        }

        public async Task<IEnumerable<OutboxEventGetDto>> GetAllAsync()
        {
            try
            {
                var data = await _outboxEventRepository.GetAllAsync(i => i.IsActive == true && i.IsDeleted == false);
                return _mapper.Map<IEnumerable<OutboxEventGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<OutboxEvent>(i => i.IsActive == true && i.IsDeleted == false);
                    return _mapper.Map<IEnumerable<OutboxEventGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList());
                }
                catch (Exception)
                {
                    return new List<OutboxEventGetDto>();
                }
            }
        }

        public async Task<IEnumerable<OutboxEventGetDto>> GetAllByErrorProcessAsync()
        {
            try
            {
                var data = await _outboxEventRepository.GetAllAsync(i => i.IsActive == true && i.IsDeleted == false && i.IsProcessed == false);
                return _mapper.Map<IEnumerable<OutboxEventGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<OutboxEvent>(i => i.IsActive == true && i.IsDeleted == false && i.IsProcessed == false);
                    return _mapper.Map<IEnumerable<OutboxEventGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList());
                }
                catch (Exception)
                {
                    return new List<OutboxEventGetDto>();
                }
            }
        }

        public async Task<IEnumerable<OutboxEventGetDto>> GetAllBySuccessfullProcessAsync()
        {
            try
            {
                var data = await _outboxEventRepository.GetAllAsync(i => i.IsActive == true && i.IsDeleted == false && i.IsProcessed == true);
                return _mapper.Map<IEnumerable<OutboxEventGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<OutboxEvent>(i => i.IsActive == true && i.IsDeleted == false && i.IsProcessed == true);
                    return _mapper.Map<IEnumerable<OutboxEventGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList());
                }
                catch (Exception)
                {
                    return new List<OutboxEventGetDto>();
                }
            }
        }

        public async Task<OutboxEventGetDto> GetByIdAsync(int? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id), "id was null");

            try
            {
                var data= await _outboxEventRepository.GetAsync(i => i.Id == id);
                return _mapper.Map<OutboxEventGetDto>(data);
            }
            catch (Exception)
            {
                try
                {
                    var data= await _azureService.GetFromAzureAsync<OutboxEvent>(id.Value);
                    return _mapper.Map<OutboxEventGetDto>(data);
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
                var entity = await _outboxEventRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsActiveNotFound };
                }

                var result = await _outboxEventRepository.SetActiveAsync(id);
                if (result)
                {
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsActiveSuccess };
                }
                return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsActiveError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting Deleted the entity.", ex);
            }
        }

        public async Task<ServiceResult<bool>> SetDeletedAsync(int id)
        {
            try
            {
                var entity = await _outboxEventRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsDeleteNotFound };
                }

                var result = await _outboxEventRepository.SetDeletedAsync(id);
                if (result)
                {
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
                var entity = await _outboxEventRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsInActiveNotFound };
                }

                var result = await _outboxEventRepository.SetInActiveAsync(id);
                if (result)
                {
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
                var entity = await _outboxEventRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.NotDeletedNotFound };
                }

                var result = await _outboxEventRepository.SetNotDeletedAsync(id);
                if (result)
                {
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.NotDeletedSuccess };
                }
                return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.NotDeleteError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting NotDeleted the entity.", ex);
            }
        }
    }
}
