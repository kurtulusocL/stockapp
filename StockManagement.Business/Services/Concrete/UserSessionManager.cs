using System.Linq.Expressions;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using StockManagement.Business.Constants.ErrorMessages;
using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Business.Services.Abstract;
using StockManagement.DataAccess.Abstract;
using StockManagement.Domain.Entities;
using StockManagement.Shared.Dtos.MappingDtos.UserSessionDtos;

namespace StockManagement.Business.Services.Concrete
{
    public class UserSessionManager : IUserSessionService
    {
        readonly IUserSessionRepository _userSessionRepository;
        readonly IAzureService _azureService;
        readonly IHttpContextAccessor _httpContextAccessor;
        readonly IUserSevice _userService;
        readonly IMapper _mapper;
        public UserSessionManager(IUserSessionRepository userSessionRepository, IAzureService azureService, IHttpContextAccessor httpContextAccessor, IUserSevice userService, IMapper mapper)
        {
            _userSessionRepository = userSessionRepository;
            _azureService = azureService;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<ServiceResult<bool>> CreateAsync(string username, DateTime loginDate, string userId)
        {
            try
            {
                userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                    throw new ArgumentNullException(nameof(userId), "userId was null");

                var user = await _userService.GetByIdAsync(userId);
                if (user == null)
                    throw new ArgumentNullException(nameof(user), "user was null");

                var model = new UserSessionCreateDto
                {
                    Username = user.UserName,
                    LoginDate = loginDate,
                    IsOnline = true,
                    AppUserId = userId
                };
                var entity = _mapper.Map<UserSession>(model);
                var result = await _userSessionRepository.AddAsync(entity);
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
                var data = await _userSessionRepository.GetAsync(i => i.Id == id);
                if (data == null)
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.DeleteExists };

                var result = await _userSessionRepository.DeleteAsync(data);
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

                var result = await _userSessionRepository.DeleteByIdsAsync(ids.Cast<object>());
                if (result == true)
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.DeleteSuccess };
                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.DeleteError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while bulk deleting entities.", ex);
            }
        }

        public async Task<IEnumerable<UserSessionGetDto>> GetAllIncludingAsync()
        {
            try
            {
                var data = await _userSessionRepository.GetAllIncludeAsync(new Expression<Func<UserSession, bool>>[]
                {
                    i=>i.IsActive==true,
                    i=>i.IsDeleted==false
                }, null, y => y.AppUser);
                return _mapper.Map<IEnumerable<UserSessionGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<UserSession>(i => i.IsActive == true && i.IsDeleted == false, y => y.AppUser);
                    return _mapper.Map<IEnumerable<UserSessionGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList());
                }
                catch (Exception)
                {
                    return new List<UserSessionGetDto>();
                }
            }
        }

        public async Task<IEnumerable<UserSessionGetDto>> GetAllIncludingByAllDataAsync()
        {
            try
            {
                var data = await _userSessionRepository.GetAllIncludeAsync(new Expression<Func<UserSession, bool>>[]
                {

                }, null, y => y.AppUser);
                return _mapper.Map<IEnumerable<UserSessionGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<UserSession>(null, y => y.AppUser);
                    return _mapper.Map<IEnumerable<UserSessionGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList());
                }
                catch (Exception)
                {
                    return new List<UserSessionGetDto>();
                }
            }
        }

        public async Task<IEnumerable<UserSessionGetDto>> GetAllIncludingByLoginDateAsync()
        {
            try
            {
                var data = await _userSessionRepository.GetAllIncludeAsync(new Expression<Func<UserSession, bool>>[]
                {
                    i=>i.IsActive==true,
                    i=>i.IsDeleted==false
                }, null, y => y.AppUser);
                return _mapper.Map<IEnumerable<UserSessionGetDto>>(data.OrderByDescending(i => i.LoginDate).ToList());
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<UserSession>(i => i.IsActive == true && i.IsDeleted == false, y => y.AppUser);
                    return _mapper.Map<IEnumerable<UserSessionGetDto>>(azureData.OrderByDescending(i => i.LoginDate).ToList());
                }
                catch (Exception)
                {
                    return new List<UserSessionGetDto>();
                }
            }
        }

        public async Task<IEnumerable<UserSessionGetDto>> GetAllIncludingByOfflineAsync()
        {
            try
            {
                var data = await _userSessionRepository.GetAllIncludeAsync(new Expression<Func<UserSession, bool>>[]
                {
                    i=>i.IsActive==true,
                    i=>i.IsDeleted==false,
                    i=>i.IsOnline==false
                }, null, y => y.AppUser);
                return _mapper.Map<IEnumerable<UserSessionGetDto>>(data.OrderByDescending(i => i.LoginDate).ToList());
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<UserSession>(i => i.IsActive == true && i.IsDeleted == false && i.IsOnline == false, y => y.AppUser);
                    return _mapper.Map<IEnumerable<UserSessionGetDto>>(azureData.OrderByDescending(i => i.LoginDate).ToList());
                }
                catch (Exception)
                {
                    return new List<UserSessionGetDto>();
                }
            }
        }

        public async Task<IEnumerable<UserSessionGetDto>> GetAllIncludingByOnlineAsync()
        {
            try
            {
                var data = await _userSessionRepository.GetAllIncludeAsync(new Expression<Func<UserSession, bool>>[]
                {
                    i=>i.IsActive==true,
                    i=>i.IsDeleted==false,
                    i=>i.IsOnline==true
                }, null, y => y.AppUser);
                return _mapper.Map<IEnumerable<UserSessionGetDto>>(data.OrderByDescending(i => i.LoginDate).ToList());
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<UserSession>(i => i.IsActive == true && i.IsDeleted == false && i.IsOnline == true, y => y.AppUser);
                    return _mapper.Map<IEnumerable<UserSessionGetDto>>(azureData.OrderByDescending(i => i.LoginDate).ToList());
                }
                catch (Exception)
                {
                    return new List<UserSessionGetDto>();
                }
            }
        }

        public async Task<IEnumerable<UserSessionGetDto>> GetAllIncludingByUserIdAsync(string userId)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId), "userId was null");

            try
            {
                var data = await _userSessionRepository.GetAllIncludeByIdAsync(userId, "AppUserId", new Expression<Func<UserSession, bool>>[]
                {
                     i=>i.IsActive==true,
                    i=>i.IsDeleted==false
                }, y => y.AppUser);
                return _mapper.Map<IEnumerable<UserSessionGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<UserSession>(i => i.AppUserId == userId && i.IsActive == true && i.IsDeleted == false, y => y.AppUser);
                    return _mapper.Map<IEnumerable<UserSessionGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList());
                }
                catch (Exception)
                {
                    return new List<UserSessionGetDto>();
                }
            }
        }

        public async Task<UserSessionGetDto> GetByIdAsync(int? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id), "id was null");

            try
            {
                var data = await _userSessionRepository.GetIncludeAsync(i => i.Id == id, y => y.AppUserId);
                return _mapper.Map<UserSessionGetDto>(data);
            }
            catch (Exception)
            {
                try
                {
                    var data = await _azureService.GetFromAzureWithIncludesAsync<UserSession>(i => i.Id == id, y => y.AppUser);
                    return _mapper.Map<UserSessionGetDto>(data);
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
                var entity = await _userSessionRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsActiveNotFound };
                }

                var result = await _userSessionRepository.SetActiveAsync(id);
                if (result)
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsActiveSuccess };
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
                var entity = await _userSessionRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsDeleteNotFound };
                }

                var result = await _userSessionRepository.SetDeletedAsync(id);
                if (result)
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsDeletedSuccess };
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
                var entity = await _userSessionRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsInActiveNotFound };
                }

                var result = await _userSessionRepository.SetInActiveAsync(id);
                if (result)
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsInActiveSuccess };
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
                var entity = await _userSessionRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.NotDeletedNotFound };
                }

                var result = await _userSessionRepository.SetNotDeletedAsync(id);
                if (result)
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.NotDeletedSuccess };
                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.NotDeleteError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting NotDeleted the entity.", ex);
            }
        }
    }
}
