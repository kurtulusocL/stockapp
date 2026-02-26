using System.Linq.Expressions;
using AutoMapper;
using StockManagement.Business.Constants.ErrorMessages;
using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Business.Services.Abstract;
using StockManagement.DataAccess.Abstract;
using StockManagement.Domain.Entities;
using StockManagement.Shared.Dtos.MappingDtos.AuditDtos;

namespace StockManagement.Business.Services.Concrete
{
    public class AuditManager : IAuditService
    {
        readonly IAuditRepository _auditRepository;
        readonly IAzureService _azureService;
        readonly IMapper _mapper;
        public AuditManager(IAuditRepository auditRepository, IAzureService azureService, IMapper mapper)
        {
            _auditRepository = auditRepository;
            _azureService = azureService;
            _mapper = mapper;
        }

        public async Task<ServiceResult<bool>> DeleteAllByIdAsync(List<int> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                    throw new ArgumentNullException(nameof(ids), "id list was null or empty");

                var result = await _auditRepository.DeleteByIdsAsync(ids.Cast<object>());
                if (result)
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

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var data = await _auditRepository.GetAsync(i => i.Id == id);
                if (data == null)
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.DeleteExists };

                var result = await _auditRepository.DeleteAsync(data);
                if (result == true)
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.DeleteSuccess };

                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.DeleteError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while deleting the entity.", ex);
            }
        }

        public async Task<IEnumerable<AuditGetDto>> GetAllIncludingAllDataAsync()
        {
            try
            {
                var data = await _auditRepository.GetAllIncludeAsync(new Expression<Func<Audit, bool>>[]
                {

                }, null, y => y.AppUser);
                return _mapper.Map<IEnumerable<AuditGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<Audit>(null, y => y.AppUser);
                    return _mapper.Map<IEnumerable<AuditGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList());
                }
                catch (Exception)
                {
                    return new List<AuditGetDto>();
                }
            }
        }

        public async Task<IEnumerable<AuditGetDto>> GetAllIncludingAsync()
        {
            try
            {
                var data = await _auditRepository.GetAllIncludeAsync(new Expression<Func<Audit, bool>>[]
                {
                     i=>i.IsActive==true,
                     i=>i.IsDeleted==false
                }, null, y => y.AppUser);
                return _mapper.Map<IEnumerable<AuditGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<Audit>(i => i.IsActive == true && i.IsDeleted == false, y => y.AppUser);
                    return _mapper.Map<IEnumerable<AuditGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList());
                }
                catch (Exception)
                {
                    return new List<AuditGetDto>();
                }
            }
        }

        public async Task<IEnumerable<AuditGetDto>> GetAllIncludingByUserIdAsync(string? userId)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId), "userId was null");

            try
            {
                var data = await _auditRepository.GetAllIncludeByIdAsync(userId, "AppUserId", new Expression<Func<Audit, bool>>[]
                {
                     i=>i.IsActive==true,
                     i=>i.IsDeleted==false
                }, y => y.AppUser);
                return _mapper.Map<IEnumerable<AuditGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<Audit>(i => i.AppUserId == userId && i.IsActive == true && i.IsDeleted == false, y => y.AppUser);
                    return _mapper.Map<IEnumerable<AuditGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList());
                }
                catch (Exception)
                {
                    return new List<AuditGetDto>();
                }
            }
        }

        public async Task<AuditGetDto> GetByIdAsync(int? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id), "id was null");

            try
            {
                var data = await _auditRepository.GetIncludeAsync(i => i.Id == id, y => y.AppUser);
                return _mapper.Map<AuditGetDto>(data);
            }
            catch (Exception)
            {
                try
                {
                    var data = await _azureService.GetFromAzureWithIncludesAsync<Audit>(i => i.Id == id, y => y.AppUser);
                    return _mapper.Map<AuditGetDto>(data);
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
                var entity = await _auditRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsActiveNotFound };
                }

                var result = await _auditRepository.SetActiveAsync(id);
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
                var entity = await _auditRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsDeleteNotFound };
                }

                var result = await _auditRepository.SetDeletedAsync(id);
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
                var entity = await _auditRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsInActiveNotFound };
                }

                var result = await _auditRepository.SetInActiveAsync(id);
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
                var entity = await _auditRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.NotDeletedNotFound };
                }

                var result = await _auditRepository.SetNotDeletedAsync(id);
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
