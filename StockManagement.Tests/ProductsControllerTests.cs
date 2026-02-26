using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockManagement.API.Controllers;
using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Business.Services.Abstract;
using StockManagement.Shared.Dtos.MappingDtos.ProductDtos;
using StockManagement.Shared.Dtos.MappingDtos.UnitInStockStos;

namespace StockManagement.Tests
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly Mock<IUnitInStockService> _mockUnitInStockService;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _mockUnitInStockService = new Mock<IUnitInStockService>();
            _controller = new ProductsController(_mockProductService.Object, _mockUnitInStockService.Object);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<ProductGetDto>
        {
            new() { Id = 1, Name = "Product A", Code = "PRD-01" },
            new() { Id = 2, Name = "Product B", Code = "PRD-02" }
        };
            _mockProductService.Setup(s => s.GetAllIncludingAsync()).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllProducts();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsOk_WithEmptyList()
        {
            // Arrange
            _mockProductService.Setup(s => s.GetAllIncludingAsync()).ReturnsAsync(new List<ProductGetDto>());

            // Act
            var result = await _controller.GetAllProducts();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            (ok!.Value as List<ProductGetDto>).Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllWholeProducts_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<ProductGetDto>
        {
            new() { Id = 1, Name = "Product A", Code = "PRD-01" }
        };
            _mockProductService.Setup(s => s.GetAllIncludingByAllDataAsync()).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllWholeProducts();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAllProductsByCategory_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<ProductGetDto>
        {
            new() { Id = 1, Name = "Product A", CategoryId = 3 }
        };
            _mockProductService.Setup(s => s.GetAllIncludingByCategoryIdAsync(3)).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllProductsByCategory(3);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAllProductsByCategory_ReturnsOk_WithEmptyList_WhenCategoryNotFound()
        {
            // Arrange
            _mockProductService.Setup(s => s.GetAllIncludingByCategoryIdAsync(99)).ReturnsAsync(new List<ProductGetDto>());

            // Act
            var result = await _controller.GetAllProductsByCategory(99);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            (ok!.Value as List<ProductGetDto>).Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllProductsByWarehouse_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<ProductGetDto>
        {
            new() { Id = 1, Name = "Product A" }
        };
            _mockProductService.Setup(s => s.GetAllIncludingByWarehouseIdAsync(2)).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllProductsByWarehouse(2);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAllProductsByWarehouse_ReturnsOk_WithEmptyList_WhenWarehouseNotFound()
        {
            // Arrange
            _mockProductService.Setup(s => s.GetAllIncludingByWarehouseIdAsync(99)).ReturnsAsync(new List<ProductGetDto>());

            // Act
            var result = await _controller.GetAllProductsByWarehouse(99);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            (ok!.Value as List<ProductGetDto>).Should().BeEmpty();
        }

        [Fact]
        public async Task GetProductByUser_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<ProductGetDto>
        {
            new() { Id = 1, Name = "Product A", AppUserId = "user-1" }
        };
            _mockProductService.Setup(s => s.GetAllIncludingByUserIdAsync("user-1")).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetProductByUser("user-1");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetProductByUser_ReturnsOk_WithEmptyList_WhenUserNotFound()
        {
            // Arrange
            _mockProductService.Setup(s => s.GetAllIncludingByUserIdAsync("nonexistent")).ReturnsAsync(new List<ProductGetDto>());

            // Act
            var result = await _controller.GetProductByUser("nonexistent");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            (ok!.Value as List<ProductGetDto>).Should().BeEmpty();
        }

        [Fact]
        public async Task GetWarningStock_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<ProductGetDto>
        {
            new() { Id = 1, Name = "Critical Product" }
        };
            _mockProductService.Setup(s => s.GetAllIncludingByWarningStockAsync()).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetWarningStock();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetWarningStock_ReturnsOk_WithEmptyList_WhenNoWarningStock()
        {
            // Arrange
            _mockProductService.Setup(s => s.GetAllIncludingByWarningStockAsync()).ReturnsAsync(new List<ProductGetDto>());

            // Act
            var result = await _controller.GetWarningStock();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            (ok!.Value as List<ProductGetDto>).Should().BeEmpty();
        }

        [Fact]
        public async Task GetProductById_ReturnsOk_WhenFound()
        {
            // Arrange
            var fakeDto = new ProductGetDto { Id = 1, Name = "Product A", Code = "PRD-01" };
            _mockProductService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(fakeDto);

            // Act
            var result = await _controller.GetProductById(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeDto);
        }

        [Fact]
        public async Task GetProductById_ReturnsNotFound_WhenNull()
        {
            // Arrange
            _mockProductService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((ProductGetDto?)null);

            // Act
            var result = await _controller.GetProductById(99);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetForEdit_ReturnsOk_WhenFound()
        {
            // Arrange
            var fakeDto = new ProductUpdateDto { Id = 1, Name = "Product A", Code = "PRD-01" };
            _mockProductService.Setup(s => s.GetForEditAsync(1)).ReturnsAsync(fakeDto);

            // Act
            var result = await _controller.GetForEdit(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeDto);
        }

        [Fact]
        public async Task GetForEdit_ReturnsNotFound_WhenNull()
        {
            // Arrange
            _mockProductService.Setup(s => s.GetForEditAsync(99)).ReturnsAsync((ProductUpdateDto?)null);

            // Act
            var result = await _controller.GetForEdit(99);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task ProductCreate_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockProductService
                .Setup(s => s.CreateAsync("Product A", "PRD-01", "Description", 100m, 1, 2, "user-1", null))
                .ReturnsAsync(successResult);

            // Act
            var result = await _controller.ProductCreate("Product A", "PRD-01", "Description", 100m, 1, 2, "user-1", null);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task ProductCreate_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "Product could not be created.." };
            _mockProductService
                .Setup(s => s.CreateAsync("Product A", "PRD-01", "Description", 100m, 1, 2, "user-1", null))
                .ReturnsAsync(failResult);

            // Act
            var result = await _controller.ProductCreate("Product A", "PRD-01", "Description", 100m, 1, 2, "user-1", null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ProductUpdate_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockProductService
                .Setup(s => s.UpdateAsync("Product A", "PRD-01", "Description", 100m, 1, 2, "user-1", null, 1))
                .ReturnsAsync(successResult);

            // Act
            var result = await _controller.ProductUpdate("Product A", "PRD-01", "Description", 100m, 1, 2, "user-1", 1, null);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task ProductUpdate_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "Could not be updated.." };
            _mockProductService
                .Setup(s => s.UpdateAsync("Product A", "PRD-01", "Description", 100m, 1, 2, "user-1", null, 1))
                .ReturnsAsync(failResult);

            // Act
            var result = await _controller.ProductUpdate("Product A", "PRD-01", "Description", 100m, 1, 2, "user-1", 1, null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task AddStock_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var dto = new UnitInStockCreateDto { Quantity = 50, Code = "STK-01", ProductId = 1, WarehouseId = 2, AppUserId = "user-1" };
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockUnitInStockService
                .Setup(s => s.CreateAsync(dto.Quantity, dto.Code, dto.ProductId, dto.WarehouseId, dto.AppUserId))
                .ReturnsAsync(successResult);

            // Act
            var result = await _controller.AddStock(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task AddStock_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var dto = new UnitInStockCreateDto { Quantity = 50, Code = "STK-01", ProductId = 1, WarehouseId = 2, AppUserId = "user-1" };
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "Stock could not be added." };
            _mockUnitInStockService
                .Setup(s => s.CreateAsync(dto.Quantity, dto.Code, dto.ProductId, dto.WarehouseId, dto.AppUserId))
                .ReturnsAsync(failResult);

            // Act
            var result = await _controller.AddStock(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DeleteProductById_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockProductService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(successResult);

            // Act
            var result = await _controller.DeleteProductById(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteProductById_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "It couldn't be deleted.." };
            _mockProductService.Setup(s => s.DeleteAsync(99)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.DeleteProductById(99);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DeleteMultiple_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var ids = new List<int> { 1, 2, 3 };
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockProductService.Setup(s => s.DeleteByIdAsync(ids)).ReturnsAsync(successResult);

            // Act
            var result = await _controller.DeleteMultiple(ids);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteMultiple_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var ids = new List<int> { 99, 100 };
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "Bulk deletion failed." };
            _mockProductService.Setup(s => s.DeleteByIdAsync(ids)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.DeleteMultiple(ids);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetActive_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockProductService.Setup(s => s.SetActiveAsync(1)).ReturnsAsync(successResult);

            // Act
            var result = await _controller.SetActive(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetActive_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "Activation failed." };
            _mockProductService.Setup(s => s.SetActiveAsync(99)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.SetActive(99);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetInActive_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockProductService.Setup(s => s.SetInActiveAsync(1)).ReturnsAsync(successResult);

            // Act
            var result = await _controller.SetInActive(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetInActive_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "It couldn't be made passive." };
            _mockProductService.Setup(s => s.SetInActiveAsync(99)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.SetInActive(99);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetDeleted_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockProductService.Setup(s => s.SetDeletedAsync(1)).ReturnsAsync(successResult);

            // Act
            var result = await _controller.SetDeleted(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetDeleted_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "Soft delete failed." };
            _mockProductService.Setup(s => s.SetDeletedAsync(99)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.SetDeleted(99);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetNotDeleted_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockProductService.Setup(s => s.SetNotDeletedAsync(1)).ReturnsAsync(successResult);

            // Act
            var result = await _controller.SetNotDeleted(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetNotDeleted_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "Restore failed." };
            _mockProductService.Setup(s => s.SetNotDeletedAsync(99)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.SetNotDeleted(99);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
