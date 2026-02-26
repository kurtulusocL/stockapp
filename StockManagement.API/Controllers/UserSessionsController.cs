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
    public class UserSessionsController : ControllerBase
    {
        readonly IUserSessionService _userSessionService;
        public UserSessionsController(IUserSessionService userSessionService)
        {
            _userSessionService = userSessionService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllUserSessions()
        {
            var result = await _userSessionService.GetAllIncludingAsync();
            return Ok(result);
        }

        [HttpGet("get-by-user/{id}")]
        public async Task<IActionResult> GetAllUserSessionsByUserId(string id)
        {
            var result = await _userSessionService.GetAllIncludingByUserIdAsync(id);
            return Ok(result);
        }

        [HttpGet("get-by-login-date")]
        public async Task<IActionResult> GetAllUserSessionsByLoginDate()
        {
            var result = await _userSessionService.GetAllIncludingByLoginDateAsync();
            return Ok(result);
        }

        [HttpGet("get-online")]
        public async Task<IActionResult> GetAllUserSessionsByOnline()
        {
            var result = await _userSessionService.GetAllIncludingByOnlineAsync();
            return Ok(result);
        }

        [HttpGet("get-offline")]
        public async Task<IActionResult> GetAllUserSessionsByOffline()
        {
            var result = await _userSessionService.GetAllIncludingByOfflineAsync();
            return Ok(result);
        }

        [HttpGet("get-whole-data")]
        public async Task<IActionResult> GetAllIncludingWholeUserSessions()
        {
            var result = await _userSessionService.GetAllIncludingByAllDataAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _userSessionService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpDelete("hard-delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userSessionService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteById(List<int> ids)
        {
            var result = await _userSessionService.DeleteByIdAsync(ids);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("set-active/{id}")]
        public async Task<IActionResult> SetActive(int id)
        {
            var result = await _userSessionService.SetActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("set-inactive/{id}")]
        public async Task<IActionResult> SetInActive(int id)
        {
            var result = await _userSessionService.SetInActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SetDeleted(int id)
        {
            var result = await _userSessionService.SetDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> SetNotDeleted(int id)
        {
            var result = await _userSessionService.SetNotDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
