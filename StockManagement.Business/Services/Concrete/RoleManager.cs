using AutoMapper;
using StockManagement.Business.Constants.ErrorMessages;
using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Business.Services.Abstract;
using StockManagement.DataAccess.Abstract;
using StockManagement.Domain.Entities;
using StockManagement.Shared.Dtos.MappingDtos.AppRoleDtos;

namespace StockManagement.Business.Services.Concrete
{
    public class RoleManager : IRoleService
    {
        readonly IRoleRepository _roleRepository;
        readonly IAzureService _azureService;
        readonly IMapper _mapper;
        public RoleManager(IRoleRepository roleRepository, IAzureService azureService, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _azureService = azureService;
            _mapper = mapper;
        }

        public async Task<ServiceResult<bool>> CrateAsync(AppRoleCreateDto dto)
        {
            try
            {
                if (dto == null)
                    throw new ArgumentNullException(nameof(dto), "entity was null");

                var entity = _mapper.Map<AppRole>(dto);
                var result = await _roleRepository.AddAsync(entity);
                if (result == true)
                {
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.AddSuccess };
                }
                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.AddError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while adding the entity.", ex);
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(string id)
        {
            try
            {
                var data = await _roleRepository.GetAsync(i => i.Id == id);
                if (data == null)
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.DeleteExists };

                var result = await _roleRepository.DeleteAsync(data);
                if (result == true)
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.DeleteSuccess };

                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.DeleteError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while deleting the entity.", ex);
            }
        }

        public async Task<ServiceResult<bool>> DeleteByIdAsync(List<string> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                    throw new ArgumentNullException(nameof(ids), "id list was null or empty");

                var result = await _roleRepository.DeleteByIdsAsync(ids.Cast<object>());
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

        public async Task<IEnumerable<AppRoleGetDto>> GetAllIncludingAllDataAsync()
        {
            try
            {
                var data = await _roleRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<AppRoleGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<AppRole>(null);
                    return _mapper.Map<IEnumerable<AppRoleGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList());
                }
                catch (Exception)
                {
                    return new List<AppRoleGetDto>();
                }
            }
        }

        public async Task<IEnumerable<AppRoleGetDto>> GetAllIncludingAsync()
        {
            try
            {
                var data = await _roleRepository.GetAllAsync(i => i.IsActive == true && i.IsDeleted == false);
                return _mapper.Map<IEnumerable<AppRoleGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<AppRole>(i => i.IsActive == true && i.IsDeleted == false);
                    return _mapper.Map<IEnumerable<AppRoleGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList());
                }
                catch (Exception)
                {
                    return new List<AppRoleGetDto>();
                }
            }
        }

        public async Task<AppRoleGetDto> GetByIdAsync(string? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id), "id was null");

            try
            {
                var data = await _roleRepository.GetAsync(i => i.Id == id);
                return _mapper.Map<AppRoleGetDto>(data);
            }
            catch (Exception)
            {
                try
                {
                    var data = await _azureService.GetFromAzureAsync<AppRole>(id);
                    return _mapper.Map<AppRoleGetDto>(data);
                }
                catch (Exception ex)
                {
                    throw new Exception("An unexpected error occurred while getting the entity.", ex);
                }
            }
        }

        public async Task<ServiceResult<bool>> SetActiveAsync(string id)
        {
            try
            {
                var entity = await _roleRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsActiveNotFound };
                }

                var result = await _roleRepository.SetActiveAsync(id);
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

        public async Task<ServiceResult<bool>> SetDeletedAsync(string id)
        {
            try
            {
                var entity = await _roleRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsDeleteNotFound };
                }

                var result = await _roleRepository.SetDeletedAsync(id);
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

        public async Task<ServiceResult<bool>> SetInActiveAsync(string id)
        {
            try
            {
                var entity = await _roleRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsInActiveNotFound };
                }

                var result = await _roleRepository.SetInActiveAsync(id);
                if (result)
                {
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsInActiveSuccess };
                }
                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsInActiveError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting Deleted the entity.", ex);
            }
        }

        public async Task<ServiceResult<bool>> SetNotDeletedAsync(string id)
        {
            try
            {
                var entity = await _roleRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.NotDeletedNotFound };
                }

                var result = await _roleRepository.SetNotDeletedAsync(id);
                if (result)
                {
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.NotDeletedSuccess };
                }
                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.NotDeleteError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting Not Deleted the entity.", ex);
            }
        }

        public async Task<AppRoleUpdateDto> GetForEditAsync(string id)
        {
            var entity = await _roleRepository.GetAsync(x => x.Id == id);
            return _mapper.Map<AppRoleUpdateDto>(entity);
        }
        public async Task<ServiceResult<bool>> UpdateAsync(AppRoleUpdateDto dto)
        {
            try
            {
                if (dto == null)
                    throw new ArgumentNullException(nameof(dto), "entity was null");

                var existData = await _roleRepository.GetAsync(x => x.Id == dto.Id);
                if (existData == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.UpdateNotFound };
                }

                _mapper.Map(dto, existData);
                existData.UpdatedDate = DateTime.UtcNow;
                var result = await _roleRepository.UpdateAsync(existData);
                if (result == true)
                {
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.UpdateSuccess };
                }
                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.UpdateError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while updating the entity.", ex);
            }
        }
    }
}
