using System.Linq.Expressions;
using AutoMapper;
using StockManagement.Business.Constants.ErrorMessages;
using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Business.Services.Abstract;
using StockManagement.DataAccess.Abstract;
using StockManagement.Domain.Entities;
using StockManagement.Shared.Dtos.MappingDtos.CategoryDtos;

namespace StockManagement.Business.Services.Concrete
{
    public class CategoryManager : ICategoryService
    {
        readonly ICategoryRepository _categoryRepository;
        readonly IAzureService _azureService;
        readonly IMapper _mapper;
        public CategoryManager(ICategoryRepository categoryRepository, IAzureService azureService, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _azureService = azureService;
            _mapper = mapper;
        }

        public async Task<ServiceResult<bool>> CrateAsync(CategoryCreateDto dto)
        {
            try
            {
                if (dto == null)
                    throw new ArgumentNullException(nameof(dto), "dto was null");

                var entity = _mapper.Map<Category>(dto);
                var result = await _categoryRepository.AddAsync(entity);

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
                var data = await _categoryRepository.GetAsync(i => i.Id == id);
                if (data == null)
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.DeleteExists };

                var result = await _categoryRepository.DeleteAsync(data);
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

                var result = await _categoryRepository.DeleteByIdsAsync(ids.Cast<object>());
                if (result == true)
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.DeleteSuccess };
                return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.DeleteError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while bulk deleting entities.", ex);
            }
        }

        public async Task<IEnumerable<CategoryGetDto>> GetAllIncludingAllDataAsync()
        {
            try
            {
                var data = await _categoryRepository.GetAllIncludeAsync(new Expression<Func<Category, bool>>[]
                {

                }, null, y => y.Products);
                return _mapper.Map<IEnumerable<CategoryGetDto>>(data.OrderByDescending(i => i.CreatedDate));
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<Category>(null, y => y.Products);
                    return _mapper.Map<IEnumerable<CategoryGetDto>>(azureData.OrderByDescending(i => i.CreatedDate));
                }
                catch (Exception)
                {
                    return new List<CategoryGetDto>();
                }
            }
        }

        public async Task<IEnumerable<CategoryGetDto>> GetAllIncludingAsync()
        {
            try
            {
                var data = await _categoryRepository.GetAllIncludeAsync(new Expression<Func<Category, bool>>[]
                {
                    i=>i.IsActive==true,
                    i=>i.IsDeleted==false
                }, null, y => y.Products);
                return _mapper.Map<IEnumerable<CategoryGetDto>>(data.OrderByDescending(i => i.CreatedDate));
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetAllFromAzureAsync<Category>(i => i.IsActive == true && i.IsDeleted == false, y => y.Products);
                    return _mapper.Map<IEnumerable<CategoryGetDto>>(azureData.OrderByDescending(i => i.CreatedDate));
                }
                catch (Exception)
                {
                    return new List<CategoryGetDto>();
                }
            }
        }

        public async Task<CategoryGetDto> GetByIdAsync(int? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id), "id was null");

            try
            {
                var data = await _categoryRepository.GetIncludeAsync(i => i.Id == id, y => y.Products);
                return _mapper.Map<CategoryGetDto>(data);
            }
            catch (Exception)
            {
                try
                {
                    var azureData = await _azureService.GetFromAzureWithIncludesAsync<Category>(i => i.Id == id, y => y.Products);
                    return _mapper.Map<CategoryGetDto>(azureData);
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
                var entity = await _categoryRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsActiveNotFound };
                }

                var result = await _categoryRepository.SetActiveAsync(id);
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
                var entity = await _categoryRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsDeleteNotFound };
                }

                var result = await _categoryRepository.SetDeletedAsync(id);
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
                var entity = await _categoryRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.IsInActiveNotFound };
                }

                var result = await _categoryRepository.SetInActiveAsync(id);
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
                var entity = await _categoryRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.NotDeletedNotFound };
                }

                var result = await _categoryRepository.SetNotDeletedAsync(id);
                if (result)
                    return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.NotDeletedSuccess };
                return new ServiceResult<bool> { IsSuccess = true, Message = MessageConstants.NotDeleteError };
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting NotDeleted the entity.", ex);
            }
        }
        public async Task<CategoryUpdateDto> GetForEditAsync(int id)
        {
            var entity = await _categoryRepository.GetAsync(x => x.Id == id);
            return _mapper.Map<CategoryUpdateDto>(entity);
        }

        public async Task<ServiceResult<bool>> UpdateAsync(CategoryUpdateDto dto)
        {
            try
            {
                if (dto == null)
                    throw new ArgumentNullException(nameof(dto), "entity was null");

                var existData = await _categoryRepository.GetAsync(x => x.Id == dto.Id);
                if (existData == null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Message = MessageConstants.UpdateNotFound };
                }

                _mapper.Map(dto, existData);
                existData.UpdatedDate = DateTime.UtcNow;
                var result = await _categoryRepository.UpdateAsync(existData);
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
