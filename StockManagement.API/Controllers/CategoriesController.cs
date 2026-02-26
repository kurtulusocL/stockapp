using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Business.Filters;
using StockManagement.Business.Services.Abstract;
using StockManagement.Shared.Dtos.MappingDtos.CategoryDtos;

namespace StockManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,User,Supervisor")]
    [AuditLog]
    [ExceptionHandler]
    public class CategoriesController : ControllerBase
    {
        readonly ICategoryService _categoryService;
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllCategories()
        {
            var data = await _categoryService.GetAllIncludingAsync();
            return Ok(data);
        }

        [HttpGet("get-whole-data")]
        public async Task<IActionResult> GetAllWholeCategories()
        {
            var data = await _categoryService.GetAllIncludingAllDataAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var data = await _categoryService.GetByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> CategoryCreate([FromBody] CategoryCreateDto entity)
        {
            var result = await _categoryService.CrateAsync(entity);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpGet("get-for-edit/{id}")]
        public async Task<IActionResult> GetForEdit(int id)
        {
            var dto = await _categoryService.GetForEditAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPut]
        public async Task<IActionResult> CategoryUpdate([FromBody] CategoryUpdateDto entity)
        {
            var result = await _categoryService.UpdateAsync(entity);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpDelete("hard-delete/{id}")]
        public async Task<IActionResult> DeleteCategory( int id)
        {
            var result = await _categoryService.DeleteAsync(id);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int> ids)
        {
            var result = await _categoryService.DeleteByIdAsync(ids);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpPatch("set-active/{id}")]
        public async Task<IActionResult> SetActive(int id)
        {
            var result = await _categoryService.SetActiveAsync(id);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpPatch("set-inactive/{id}")]
        public async Task<IActionResult> SetInactive(int id)
        {
            var result = await _categoryService.SetInActiveAsync(id);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SetDeleted(int id)
        {
            var result = await _categoryService.SetDeletedAsync(id);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> SetNotDeleted(int id)
        {
            var result = await _categoryService.SetNotDeletedAsync(id);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result.Message);
        }
    }
}
