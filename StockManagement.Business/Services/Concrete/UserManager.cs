using System.Linq.Expressions;
using AutoMapper;
using StockManagement.Business.Constants.ErrorMessages;
using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Business.Services.Abstract;
using StockManagement.DataAccess.Abstract;
using StockManagement.Domain.Entities;
using StockManagement.Shared.Dtos.MappingDtos.AppUserDtos;

namespace StockManagement.Business.Services.Concrete
{
    public class UserManager : IUserSevice
    {
        readonly IUserRepository _userRepository;
        readonly IAzureService _azureService;
        readonly IMapper _mapper;
        public UserManager(IUserRepository userRepository, IAzureService azureService, IMapper mapper)
        {
            _userRepository = userRepository;
            _azureService = azureService;
            _mapper = mapper;
        }

        public async Task<ServiceResult<bool>> DeleteAsync(string id)
        {
            try
            {
                var data = await _userRepository.GetAsync(i => i.Id == id);
                if (data == null)
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.DeleteExists };

                var result = await _userRepository.DeleteAsync(data);
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

                var result = await _userRepository.DeleteByIdsAsync(ids.Cast<object>());
                if (result == true)
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.DeleteSuccess };
                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.DeleteError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while bulk deleting entities.", ex);
            }
        }

        public async Task<IEnumerable<AppUserGetDto>> GetAllIncludingAsync()
        {
            try
            {
                var data = await _userRepository.GetAllIncludeAsync(new Expression<Func<AppUser, bool>>[]
                {
                    i=>i.IsActive==true,
                    i=>i.IsDeleted==false
                }, null, y => y.Audits, y => y.StockMovements, y => y.Products, y => y.UnitInStocks, y => y.UserSessions);
                return _mapper.Map<IEnumerable<AppUserGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<AppUser>(i => i.IsActive == true && i.IsDeleted == false, y => y.Audits, y => y.StockMovements, y => y.Products, y => y.UnitInStocks, y => y.UserSessions);
                    return _mapper.Map<IEnumerable<AppUserGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList());
                }
                catch (Exception)
                {
                    return new List<AppUserGetDto>();
                }
            }
        }

        public async Task<IEnumerable<AppUserGetDto>> GetAllIncludingByActiveLoginCodeAsync()
        {
            try
            {
                var data = await _userRepository.GetAllIncludeAsync(new Expression<Func<AppUser, bool>>[]
                {
                    i=>i.IsActive==true,
                    i=>i.IsDeleted==false,
                    i=>i.IsLoginConfirmCodeActive==true
                }, null, y => y.Audits, y => y.StockMovements, y => y.Products, y => y.UnitInStocks, y => y.UserSessions);
                return _mapper.Map<IEnumerable<AppUserGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<AppUser>(i => i.IsActive == true && i.IsDeleted == false && i.IsLoginConfirmCodeActive == true, y => y.Audits, y => y.StockMovements, y => y.Products, y => y.UnitInStocks, y => y.UserSessions);
                    return _mapper.Map<IEnumerable<AppUserGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList());
                }
                catch (Exception)
                {
                    return new List<AppUserGetDto>();
                }
            }
        }

        public async Task<IEnumerable<AppUserGetDto>> GetAllIncludingByAllDataAsync()
        {
            try
            {
                var data = await _userRepository.GetAllIncludeAsync(new Expression<Func<AppUser, bool>>[]
                {

                }, null, y => y.Audits, y => y.StockMovements, y => y.Products, y => y.UnitInStocks, y => y.UserSessions);
                return _mapper.Map<IEnumerable<AppUserGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<AppUser>(i => i.IsActive == true && i.IsDeleted == false, null);
                    return _mapper.Map<IEnumerable<AppUserGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList());
                }
                catch (Exception)
                {
                    return new List<AppUserGetDto>();
                }
            }
        }

        public async Task<IEnumerable<AppUserGetDto>> GetAllIncludingByInActiveLoginCodeAsync()
        {
            try
            {
                var data = await _userRepository.GetAllIncludeAsync(new Expression<Func<AppUser, bool>>[]
                {
                    i=>i.IsActive==true,
                    i=>i.IsDeleted==false,
                    i=>i.IsLoginConfirmCodeActive==false
                }, null, y => y.Audits, y => y.StockMovements, y => y.Products, y => y.UnitInStocks, y => y.UserSessions);
                return _mapper.Map<IEnumerable<AppUserGetDto>>(data.OrderByDescending(i => i.CreatedDate).ToList());
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<AppUser>(i => i.IsActive == true && i.IsDeleted == false && i.IsLoginConfirmCodeActive == false, y => y.Audits, y => y.StockMovements, y => y.Products, y => y.UnitInStocks, y => y.UserSessions);
                    return _mapper.Map<IEnumerable<AppUserGetDto>>(azureData.OrderByDescending(i => i.CreatedDate).ToList());
                }
                catch (Exception)
                {
                    return new List<AppUserGetDto>();
                }
            }
        }

        public AppUserGetDto GetById(string userId)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId), "userId was null");

            try
            {
                var data = _userRepository.GetInclude(i => i.Id == userId, y => y.Audits, y => y.StockMovements, y => y.Products, y => y.UnitInStocks, y => y.UserSessions);
                return _mapper.Map<AppUserGetDto>(data);
            }
            catch (Exception)
            {
                try
                {
                    var data = _azureService.GetFromAzureWithIncludes<AppUser>(i => i.Id == userId, y => y.Audits, y => y.StockMovements, y => y.Products, y => y.UnitInStocks, y => y.UserSessions);
                    return _mapper.Map<AppUserGetDto>(data);
                }
                catch (Exception ex)
                {
                    throw new Exception("An unexpected error occurred while getting the entity.", ex);
                }
            }
        }

        public async Task<AppUserGetDto> GetByIdAsync(string id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id), "id was null");

            try
            {
                var data = await _userRepository.GetIncludeAsync(i => i.Id == id, y => y.Audits, y => y.StockMovements, y => y.Products, y => y.UnitInStocks, y => y.UserSessions);
                return _mapper.Map<AppUserGetDto>(data);
            }
            catch (Exception)
            {
                try
                {
                    var data = await _azureService.GetFromAzureWithIncludesAsync<AppUser>(i => i.Id == id, y => y.Audits, y => y.StockMovements, y => y.Products, y => y.UnitInStocks, y => y.UserSessions);
                    return _mapper.Map<AppUserGetDto>(data);
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
                var entity = await _userRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsActiveNotFound };
                }

                var result = await _userRepository.SetActiveAsync(id);
                if (result)
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsActiveSuccess };
                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsActiveError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting Active the entity.", ex);
            }
        }

        public async Task<ServiceResult<bool>> SetActiveLoginConfirmCodeAsync(string id)
        {
            try
            {
                var entity = await _userRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.WarningTitle };
                }

                var result = await _userRepository.SetActiveLoginConfirmCodeAsync(id);
                if (result)
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.SuccessTitle };
                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.ErrorTitle };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting Confirm Code Active for Login the entity.", ex);
            }
        }

        public async Task<ServiceResult<bool>> SetDeletedAsync(string id)
        {
            try
            {
                var entity = await _userRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsDeleteNotFound };
                }

                var result = await _userRepository.SetDeletedAsync(id);
                if (result)
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsDeletedSuccess };
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
                var entity = await _userRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsInActiveNotFound };
                }

                var result = await _userRepository.SetInActiveAsync(id);
                if (result)
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.IsInActiveSuccess };
                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsInActiveError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting InActive the entity.", ex);
            }
        }

        public async Task<ServiceResult<bool>> SetInActiveLoginConfirmCodeAsync(string id)
        {
            try
            {
                var entity = await _userRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.WarningTitle };
                }

                var result = await _userRepository.SetInActiveLoginConfirmCodeAsync(id);
                if (result)
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.SuccessTitle };
                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.ErrorTitle };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting Confirm Code InActive for Login the entity.", ex);
            }
        }

        public async Task<ServiceResult<bool>> SetNotDeletedAsync(string id)
        {
            try
            {
                var entity = await _userRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.NotDeletedNotFound };
                }

                var result = await _userRepository.SetNotDeletedAsync(id);
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
