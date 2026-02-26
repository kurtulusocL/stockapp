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
    public class ProductsController : ControllerBase
    {
        readonly IProductService _productService;
        readonly IUnitInStockService _unitInStockService;
        public ProductsController(IProductService productService, IUnitInStockService unitInStockService)
        {
            _productService = productService;
            _unitInStockService = unitInStockService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = await _productService.GetAllIncludingAsync();
            return Ok(result);
        }

        [HttpGet("get-whole-data")]
        public async Task<IActionResult> GetAllWholeProducts()
        {
            var result = await _productService.GetAllIncludingByAllDataAsync();
            return Ok(result);
        }

        [HttpGet("get-by-category/{categoryId}")]
        public async Task<IActionResult> GetAllProductsByCategory(int categoryId)
        {
            var result = await _productService.GetAllIncludingByCategoryIdAsync(categoryId);
            return Ok(result);
        }

        [HttpGet("get-by-warehouse/{warehouseId}")]
        public async Task<IActionResult> GetAllProductsByWarehouse(int warehouseId)
        {
            var result = await _productService.GetAllIncludingByWarehouseIdAsync(warehouseId);
            return Ok(result);
        }

        [HttpGet("get-by-user/{userId}")]
        public async Task<IActionResult> GetProductByUser(string userId)
        {
            var result = await _productService.GetAllIncludingByUserIdAsync(userId);
            return Ok(result);
        }

        [HttpGet("get-warning-stock")]
        public async Task<IActionResult> GetWarningStock()
        {
            var result = await _productService.GetAllIncludingByWarningStockAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("get-for-edit/{id}")]
        public async Task<IActionResult> GetForEdit(int id)
        {
            var result = await _productService.GetForEditAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> ProductCreate([FromForm] string name, [FromForm] string code, [FromForm] string description, [FromForm] decimal price,
            [FromForm] int categoryId, [FromForm] int warehouseId, [FromForm] string userId, IFormFile? image)
        {
            var result = await _productService.CreateAsync(name, code, description, price, categoryId, warehouseId, userId, image);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> ProductUpdate([FromForm] string name, [FromForm] string code, [FromForm] string description, [FromForm] decimal price,
            [FromForm] int categoryId, [FromForm] int warehouseId, [FromForm] string userId, [FromForm] int id, IFormFile? image)
        {
            var result = await _productService.UpdateAsync(name, code, description, price, categoryId, warehouseId, userId, image, id);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("add-stock")]
        public async Task<IActionResult> AddStock([FromBody] UnitInStockCreateDto dto)
        {
            var result = await _unitInStockService.CreateAsync(dto.Quantity, dto.Code, dto.ProductId, dto.WarehouseId, dto.AppUserId);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("hard-delete/{id}")]
        public async Task<IActionResult> DeleteProductById(int id)
        {
            var result = await _productService.DeleteAsync(id);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int> ids)
        {
            var result = await _productService.DeleteByIdAsync(ids);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("set-active/{id}")]
        public async Task<IActionResult> SetActive(int id)
        {
            var result = await _productService.SetActiveAsync(id);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("set-inactive/{id}")]
        public async Task<IActionResult> SetInActive(int id)
        {
            var result = await _productService.SetInActiveAsync(id);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SetDeleted(int id)
        {
            var result = await _productService.SetDeletedAsync(id);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> SetNotDeleted(int id)
        {
            var result = await _productService.SetNotDeletedAsync(id);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }
    }
}
