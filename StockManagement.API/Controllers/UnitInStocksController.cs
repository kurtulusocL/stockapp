using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Business.Filters;
using StockManagement.Business.Services.Abstract;
using StockManagement.Shared.Dtos.MappingDtos.UnitInStockStos;

namespace StockManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,User,Supervisor")]
    [AuditLog]
    [ExceptionHandler]
    public class UnitInStocksController : ControllerBase
    {
        readonly IUnitInStockService _unitInStockService;
        public UnitInStocksController(IUnitInStockService unitInStockService)
        {
            _unitInStockService = unitInStockService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllIncluding()
        {
            var result = await _unitInStockService.GetAllIncludingAsync();
            return Ok(result);
        }

        [HttpGet("get-by-product/{productId}")]
        public async Task<IActionResult> GetAllIncludingByProductId(int? productId)
        {
            var result = await _unitInStockService.GetAllIncludingByProductIdAsync(productId);
            return Ok(result);
        }

        [HttpGet("get-by-warehouse/{warehouseId}")]
        public async Task<IActionResult> GetAllIncludingByWarehouseId(int warehouseId)
        {
            var result = await _unitInStockService.GetAllIncludingByWarehouseIdAsync(warehouseId);
            return Ok(result);
        }

        [HttpGet("get-by-user/{userId}")]
        public async Task<IActionResult> GetAllIncludingByUserId(string userId)
        {
            var result = await _unitInStockService.GetAllIncludingByUserIdAsync(userId);
            return Ok(result);
        }

        [HttpGet("get-whole-data")]
        public async Task<IActionResult> GetAllIncludingAllData()
        {
            var result = await _unitInStockService.GetAllIncludingByAllDataAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _unitInStockService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("get-for-edit/{id}")]
        public async Task<IActionResult> GetForEdit(int id)
        {
            var result = await _unitInStockService.GetForEditAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("get-by-id-sync/{id}")]
        public IActionResult GetByIdSync(int? id)
        {
            var result = _unitInStockService.GetById(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UnitInStockUpdateDto dto)
        {
            var result = await _unitInStockService.UpdateAsync(dto.Quantity, dto.Code, dto.ProductId, dto.WarehouseId, dto.AppUserId, dto.Id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("hard-delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _unitInStockService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteById(List<int> ids)
        {
            var result = await _unitInStockService.DeleteByIdAsync(ids);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("set-active/{id}")]
        public async Task<IActionResult> SetActive(int id)
        {
            var result = await _unitInStockService.SetActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("set-inactive/{id}")]
        public async Task<IActionResult> SetInActive(int id)
        {
            var result = await _unitInStockService.SetInActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SetDeleted(int id)
        {
            var result = await _unitInStockService.SetDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> SetNotDeleted(int id)
        {
            var result = await _unitInStockService.SetNotDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
