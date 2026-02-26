using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Business.Filters;
using StockManagement.Business.Services.Abstract;

namespace StockManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [AuditLog]
    [ExceptionHandler]
    public class AuditsController : ControllerBase
    {
        readonly IAuditService _auditService;
        public AuditsController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllAudits()
        {
            var result = await _auditService.GetAllIncludingAsync();
            return Ok(result);
        }

        [HttpGet("get-by-user/{id}")]
        public async Task<IActionResult> GetAllAuditsByUserId(string? id)
        {
            var result = await _auditService.GetAllIncludingByUserIdAsync(id);
            return Ok(result);
        }

        [HttpGet("get-whole-data")]
        public async Task<IActionResult> GetAllWholeAudits()
        {
            var result = await _auditService.GetAllIncludingAllDataAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuditById(int id)
        {
            var result = await _auditService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpDelete("hard-delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _auditService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteAuditById(List<int> ids)
        {
            var result = await _auditService.DeleteAllByIdAsync(ids);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("set-active/{id}")]
        public async Task<IActionResult> SetActive(int id)
        {
            var result = await _auditService.SetActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("set-inactive/{id}")]
        public async Task<IActionResult> SetInActive(int id)
        {
            var result = await _auditService.SetInActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SetDeleted(int id)
        {
            var result = await _auditService.SetDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> SetNotDeleted(int id)
        {
            var result = await _auditService.SetNotDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
