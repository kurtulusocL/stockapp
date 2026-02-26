using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Business.Filters;
using StockManagement.Business.Services.Abstract;

namespace StockManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,User,Supervisor")]
    [AuditLog]
    [ExceptionHandler]
    public class DashboardsController : ControllerBase
    {
        readonly IUserSevice _userService;
        public DashboardsController(IUserSevice userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var user = await _userService.GetByIdAsync(userId);
            if (user == null) return NotFound();

            return Ok(user);
        }
    }
}
