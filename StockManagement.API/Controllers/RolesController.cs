using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Business.Filters;
using StockManagement.Business.Services.Abstract;
using StockManagement.Shared.Dtos.MappingDtos.AppRoleDtos;

namespace StockManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [AuditLog]
    [ExceptionHandler]
    public class RolesController : ControllerBase
    {
        readonly IRoleService _roleService;
        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _roleService.GetAllIncludingAsync();
            return Ok(result);
        }

        [HttpGet("get-whole-data")]
        public async Task<IActionResult> GetAllWholeRoles()
        {
            var result = await _roleService.GetAllIncludingAllDataAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _roleService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> RoleCreate(AppRoleCreateDto dto)
        {
            var result = await _roleService.CrateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("get-for-edit/{id}")]
        public async Task<IActionResult> GetForEdit(string id)
        {
            var result = await _roleService.GetForEditAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRole(AppRoleUpdateDto dto)
        {
            var result = await _roleService.UpdateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("hard-delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _roleService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteById(List<string> ids)
        {
            var result = await _roleService.DeleteByIdAsync(ids);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("set-active/{id}")]
        public async Task<IActionResult> SetActive(string id)
        {
            var result = await _roleService.SetActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("set-inactive/{id}")]
        public async Task<IActionResult> SetInActive(string id)
        {
            var result = await _roleService.SetInActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SetDeleted(string id)
        {
            var result = await _roleService.SetDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> SetNotDeleted(string id)
        {
            var result = await _roleService.SetNotDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
