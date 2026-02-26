using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockManagement.API.Controllers;
using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Business.Services.Abstract;
using StockManagement.Shared.Dtos.MappingDtos.UnitInStockStos;

namespace StockManagement.Tests
{
    public class UnitInStocksControllerTests
    {
        private readonly Mock<IUnitInStockService> _mockUnitInStockService;
        private readonly UnitInStocksController _controller;

        public UnitInStocksControllerTests()
        {
            _mockUnitInStockService = new Mock<IUnitInStockService>();
            _controller = new UnitInStocksController(_mockUnitInStockService.Object);
        }

        [Fact]
        public async Task GetAllIncluding_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<UnitInStockGetDto>
        {
            new() { Id = 1, Quantity = 10 },
            new() { Id = 2, Quantity = 20 }
        };
            _mockUnitInStockService.Setup(s => s.GetAllIncludingAsync()).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllIncluding();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAllIncluding_ReturnsOk_WithEmptyList()
        {
            // Arrange
            _mockUnitInStockService.Setup(s => s.GetAllIncludingAsync()).ReturnsAsync(new List<UnitInStockGetDto>());

            // Act
            var result = await _controller.GetAllIncluding();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            (ok!.Value as List<UnitInStockGetDto>).Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllIncludingByProductId_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<UnitInStockGetDto>
        {
            new() { Id = 1, ProductId = 5, Quantity = 10 }
        };
            _mockUnitInStockService.Setup(s => s.GetAllIncludingByProductIdAsync(5)).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllIncludingByProductId(5);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAllIncludingByProductId_ReturnsOk_WithEmptyList_WhenProductNotFound()
        {
            // Arrange
            _mockUnitInStockService.Setup(s => s.GetAllIncludingByProductIdAsync(99)).ReturnsAsync(new List<UnitInStockGetDto>());

            // Act
            var result = await _controller.GetAllIncludingByProductId(99);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            (ok!.Value as List<UnitInStockGetDto>).Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllIncludingByWarehouseId_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<UnitInStockGetDto>
        {
            new() { Id = 1, WarehouseId = 3, Quantity = 15 }
        };
            _mockUnitInStockService.Setup(s => s.GetAllIncludingByWarehouseIdAsync(3)).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllIncludingByWarehouseId(3);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAllIncludingByWarehouseId_ReturnsOk_WithEmptyList_WhenWarehouseNotFound()
        {
            // Arrange
            _mockUnitInStockService.Setup(s => s.GetAllIncludingByWarehouseIdAsync(99)).ReturnsAsync(new List<UnitInStockGetDto>());

            // Act
            var result = await _controller.GetAllIncludingByWarehouseId(99);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            (ok!.Value as List<UnitInStockGetDto>).Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllIncludingByUserId_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<UnitInStockGetDto>
        {
            new() { Id = 1, AppUserId = "user-1", Quantity = 10 }
        };
            _mockUnitInStockService.Setup(s => s.GetAllIncludingByUserIdAsync("user-1")).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllIncludingByUserId("user-1");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAllIncludingByUserId_ReturnsOk_WithEmptyList_WhenUserNotFound()
        {
            // Arrange
            _mockUnitInStockService.Setup(s => s.GetAllIncludingByUserIdAsync("nonexistent")).ReturnsAsync(new List<UnitInStockGetDto>());

            // Act
            var result = await _controller.GetAllIncludingByUserId("nonexistent");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            (ok!.Value as List<UnitInStockGetDto>).Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllIncludingAllData_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<UnitInStockGetDto>
        {
            new() { Id = 1, Quantity = 10 }
        };
            _mockUnitInStockService.Setup(s => s.GetAllIncludingByAllDataAsync()).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllIncludingAllData();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenFound()
        {
            // Arrange
            var fakeDto = new UnitInStockGetDto { Id = 1, Quantity = 10 };
            _mockUnitInStockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(fakeDto);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeDto);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenNull()
        {
            // Arrange
            _mockUnitInStockService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((UnitInStockGetDto?)null);

            // Act
            var result = await _controller.GetById(99);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetForEdit_ReturnsOk_WhenFound()
        {
            // Arrange
            var fakeDto = new UnitInStockUpdateDto { Id = 1, Quantity = 10 };
            _mockUnitInStockService.Setup(s => s.GetForEditAsync(1)).ReturnsAsync(fakeDto);

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
            _mockUnitInStockService.Setup(s => s.GetForEditAsync(99)).ReturnsAsync((UnitInStockUpdateDto?)null);

            // Act
            var result = await _controller.GetForEdit(99);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void GetByIdSync_ReturnsOk_WhenFound()
        {
            // Arrange
            var fakeDto = new UnitInStockGetDto { Id = 1, Quantity = 10 };
            _mockUnitInStockService.Setup(s => s.GetById(1)).Returns(fakeDto);

            // Act
            var result = _controller.GetByIdSync(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeDto);
        }

        [Fact]
        public void GetByIdSync_ReturnsNotFound_WhenNull()
        {
            // Arrange
            _mockUnitInStockService.Setup(s => s.GetById(99)).Returns((UnitInStockGetDto?)null);

            // Act
            var result = _controller.GetByIdSync(99);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var dto = new UnitInStockUpdateDto { Id = 1, Quantity = 50, Code = "STK-01", ProductId = 2, WarehouseId = 3, AppUserId = "user-1" };
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockUnitInStockService
                .Setup(s => s.UpdateAsync(dto.Quantity, dto.Code, dto.ProductId, dto.WarehouseId, dto.AppUserId, dto.Id))
                .ReturnsAsync(successResult);

            // Act
            var result = await _controller.Update(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var dto = new UnitInStockUpdateDto { Id = 1, Quantity = 50, Code = "STK-01", ProductId = 2, WarehouseId = 3, AppUserId = "user-1" };
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "Could not be updated.." };
            _mockUnitInStockService
                .Setup(s => s.UpdateAsync(dto.Quantity, dto.Code, dto.ProductId, dto.WarehouseId, dto.AppUserId, dto.Id))
                .ReturnsAsync(failResult);

            // Act
            var result = await _controller.Update(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockUnitInStockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(successResult);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Delete_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "It couldn't be deleted." };
            _mockUnitInStockService.Setup(s => s.DeleteAsync(99)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.Delete(99);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DeleteById_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var ids = new List<int> { 1, 2, 3 };
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockUnitInStockService.Setup(s => s.DeleteByIdAsync(ids)).ReturnsAsync(successResult);

            // Act
            var result = await _controller.DeleteById(ids);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteById_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var ids = new List<int> { 99, 100 };
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "Bulk deletion failed." };
            _mockUnitInStockService.Setup(s => s.DeleteByIdAsync(ids)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.DeleteById(ids);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetActive_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockUnitInStockService.Setup(s => s.SetActiveAsync(1)).ReturnsAsync(successResult);

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
            _mockUnitInStockService.Setup(s => s.SetActiveAsync(99)).ReturnsAsync(failResult);

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
            _mockUnitInStockService.Setup(s => s.SetInActiveAsync(1)).ReturnsAsync(successResult);

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
            _mockUnitInStockService.Setup(s => s.SetInActiveAsync(99)).ReturnsAsync(failResult);

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
            _mockUnitInStockService.Setup(s => s.SetDeletedAsync(1)).ReturnsAsync(successResult);

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
            _mockUnitInStockService.Setup(s => s.SetDeletedAsync(99)).ReturnsAsync(failResult);

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
            _mockUnitInStockService.Setup(s => s.SetNotDeletedAsync(1)).ReturnsAsync(successResult);

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
            _mockUnitInStockService.Setup(s => s.SetNotDeletedAsync(99)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.SetNotDeleted(99);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
