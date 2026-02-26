using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Business.Filters;
using StockManagement.Business.Services.Abstract;

namespace StockManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuditLog]
    [ExceptionHandler]
    public class UsersController : ControllerBase
    {
        readonly IUserSevice _userService;
        public UsersController(IUserSevice userSevice)
        {
            _userService = userSevice;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllIncludingAsync();
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-by-active-login-code")]
        public async Task<IActionResult> GetAllUsersByActiveLoginCode()
        {
            var result = await _userService.GetAllIncludingByActiveLoginCodeAsync();
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-by-inactive-login-code")]
        public async Task<IActionResult> GetAllUsersByInActiveLoginCode()
        {
            var result = await _userService.GetAllIncludingByInActiveLoginCodeAsync();
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-whole-data")]
        public async Task<IActionResult> GetAllWholeUsers()
        {
            var result = await _userService.GetAllIncludingByAllDataAsync();
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var result = await _userService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-by-user/{id}")]
        public IActionResult GetByUserId(string id)
        {
            var result = _userService.GetById(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("hard-delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _userService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteUserById(List<string> ids)
        {
            var result = await _userService.DeleteByIdAsync(ids);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,User,Supervisor")]
        [HttpPatch("set-active-login-code/{id}")]
        public async Task<IActionResult> SetActiveLoginConfirmCode(string id)
        {
            var result = await _userService.SetActiveLoginConfirmCodeAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,User,Supervisor")]
        [HttpPatch("set-inactive-login-code/{id}")]
        public async Task<IActionResult> SetInActiveLoginConfirmCode(string id)
        {
            var result = await _userService.SetInActiveLoginConfirmCodeAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("set-active/{id}")]
        public async Task<IActionResult> SetActive(string id)
        {
            var result = await _userService.SetActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("set-inactive/{id}")]
        public async Task<IActionResult> SetInActive(string id)
        {
            var result = await _userService.SetInActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SetDeleted(string id)
        {
            var result = await _userService.SetDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> SetNotDeleted(string id)
        {
            var result = await _userService.SetNotDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
