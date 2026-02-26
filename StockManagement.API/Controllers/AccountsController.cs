using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Business.Filters;
using StockManagement.Business.Services.Abstract;
using StockManagement.Shared.Dtos.AuthDtos;
using StockManagement.Shared.Dtos.AuthDtos.OAuthDtos;
using StockManagement.Shared.ViewModels.RoleVM;

namespace StockManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuditLog]
    [ExceptionHandler]
    public class AccountsController : ControllerBase
    {
        readonly IAuthService _authService;
        public AccountsController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);
            if (token == null)
                return Unauthorized("Invalid username or password.");
            return Ok(new { token });
        }

        [AllowAnonymous]
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            var token = await _authService.GoogleLoginAsync(dto);
            if (token == null)
                return Unauthorized("Login with Google failed.");
            return Ok(new { token = token });
        }

        [AllowAnonymous]
        [HttpPost("login-with-confirm-code")]
        public async Task<IActionResult> LoginWithConfirmCode([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginWithConfirmCodeAsync(dto);
            if (result == null)
                return Unauthorized("Invalid username or password.");
            if (result == "confirm_required")
                return Ok(new LoginResponseDto { ConfirmRequired = true });
            return Ok(new LoginResponseDto { Token = result, ConfirmRequired = false });
        }

        [AllowAnonymous]
        [HttpPost("verify-login-confirm-code")]
        public async Task<IActionResult> VerifyLoginConfirmCode([FromBody] LoginConfirmCodeDto dto)
        {
            var token = await _authService.VerifyLoginConfirmCodeAsync(dto);
            if (token == null)
                return BadRequest("Code validation failed.");
            return Ok(new LoginResponseDto { Token = token });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (!result)
                return BadRequest("Registration failed.");
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("confirm-mail")]
        public async Task<IActionResult> ConfirmMail(ConfirmCodeDto dto)
        {
            var result = await _authService.ConfirmMailAsync(dto);
            if (!result)
                return BadRequest("Email verification failed.");
            return Ok(result);
        }

        [Authorize(Roles = "Admin,User,Supervisor")]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();
            var result = await _authService.ChangePasswordAsync(dto, userId);
            if (!result)
                return BadRequest("Password change attempt failed.");
            return Ok(result);
        }

        [Authorize(Roles = "Admin,User,Supervisor")]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto, [FromQuery] string code)
        {
            var result = await _authService.ResetPassword(dto, code);
            if (!result)
                return BadRequest("Password reset attempt failed.");
            return Ok(result);
        }

        [Authorize(Roles = "Admin,User,Supervisor")]
        [HttpGet("get-data-update-profile")]
        public async Task<IActionResult> GetDataUpdateProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _authService.GetDataUpdateProfileAsync(userId);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,User,Supervisor")]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();
            var result = await _authService.UpdateProfileAsync(dto, userId);
            if (!result)
                return BadRequest("Profile update failed.");
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-role-assign/{id}")]
        public async Task<IActionResult> GetRoleAssign(string id)
        {
            var result = await _authService.GetRoleAssignAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("role-assign/{id}")]
        public async Task<IActionResult> RoleAssign(List<RoleAssignVM> modelList, string id)
        {
            var result = await _authService.RoleAssignAsync(modelList, id);
            if (!result)
                return BadRequest("Role assignment failed.");
            return Ok(result);
        }

        [Authorize(Roles = "Admin,User,Supervisor")]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();
            var result = await _authService.LogoutAsync(userId);
            if (!result)
                return BadRequest("Logout failed.");
            return Ok(result);
        }
    }
}
