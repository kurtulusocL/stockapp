using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Business.Filters;
using StockManagement.Business.Services.Abstract;
using StockManagement.Shared.Dtos.MappingDtos.WarehouseDtos;

namespace StockManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,User,Supervisor")]
    [AuditLog]
    [ExceptionHandler]
    public class WarehousesController : ControllerBase
    {
        readonly IWarehouseService _warehouseService;
        public WarehousesController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllWarehouses()
        {
            var data = await _warehouseService.GetAllIncludingAsync();
            return Ok(data);
        }

        [HttpGet("get-whole-data")]
        public async Task<IActionResult> GetAllWholeWarehouses()
        {
            var data = await _warehouseService.GetAllIncludingByAllDataAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWarehouseById(int id)
        {
            var data = await _warehouseService.GetByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> WarehouseCreate([FromBody] WarehouseCreateDto dto)
        {
            var result = await _warehouseService.CreateAsync(dto.Name, dto.Code, dto.Address, dto.TypeOfWarehouse);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpGet("get-for-edit/{id}")]
        public async Task<IActionResult> GetForEdit(int id)
        {
            var dto = await _warehouseService.GetForEditAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPut]
        public async Task<IActionResult> WarehouseUpdate([FromBody] WarehouseUpdateDto dto)
        {
            var result = await _warehouseService.UpdateAsync(dto.Name, dto.Code, dto.Address, dto.TypeOfWarehouse, dto.Id);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpDelete("hard-delete/{id}")]
        public async Task<IActionResult> DeleteWarehouseById(int id)
        {
            var result = await _warehouseService.DeleteAsync(id);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int> ids)
        {
            var result = await _warehouseService.DeleteByIdAsync(ids);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpPatch("set-active/{id}")]
        public async Task<IActionResult> SetActive(int id)
        {
            var result = await _warehouseService.SetActiveAsync(id);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpPatch("set-inactive/{id}")]
        public async Task<IActionResult> SetInactive(int id)
        {
            var result = await _warehouseService.SetInActiveAsync(id);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SetDeleted(int id)
        {
            var result = await _warehouseService.SetDeletedAsync(id);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> SetNotDeleted(int id)
        {
            var result = await _warehouseService.SetNotDeletedAsync(id);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result.Message);
        }
    }
}
