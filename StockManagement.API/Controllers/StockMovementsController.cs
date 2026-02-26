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
    public class StockMovementsController : ControllerBase
    {
        readonly IStockMovementService _stockMovementService;
        public StockMovementsController(IStockMovementService stockMovementService)
        {
            _stockMovementService = stockMovementService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllStockMovements()
        {
            var result = await _stockMovementService.GetAllIncludingAsync();
            return Ok(result);
        }

        [HttpGet("get-by-user/{userId}")]
        public async Task<IActionResult> GetAllStockMovementsByUserId(string userId)
        {
            var result = await _stockMovementService.GetAllIncludingByUserIdAsync(userId);
            return Ok(result);
        }

        [HttpGet("get-by-product/{productId}")]
        public async Task<IActionResult> GetAllStockMovementsByProductId(int? productId)
        {
            var result = await _stockMovementService.GetAllIncludingByProductIdAsync(productId);
            return Ok(result);
        }

        [HttpGet("get-by-range")]
        public async Task<IActionResult> GetAllStockMovementsRange([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var result = await _stockMovementService.GetAllIncludingRangeAsync(start, end);
            return Ok(result);
        }

        [HttpGet("get-whole-data")]
        public async Task<IActionResult> GetAllWholeStockMovements()
        {
            var result = await _stockMovementService.GetAllIncludingByAllDataAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStockMovementById(int id)
        {
            var result = await _stockMovementService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpDelete("hard-delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _stockMovementService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteStockMovementById(List<int> ids)
        {
            var result = await _stockMovementService.DeleteByIdAsync(ids);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("set-active/{id}")]
        public async Task<IActionResult> SetActive(int id)
        {
            var result = await _stockMovementService.SetActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("set-inactive/{id}")]
        public async Task<IActionResult> SetInActive(int id)
        {
            var result = await _stockMovementService.SetInActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SetDeleted(int id)
        {
            var result = await _stockMovementService.SetDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> SetNotDeleted(int id)
        {
            var result = await _stockMovementService.SetNotDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
