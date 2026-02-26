using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockManagement.API.Controllers;
using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Business.Services.Abstract;
using StockManagement.Shared.Dtos.MappingDtos.StockMovementDtos;

namespace StockManagement.Tests
{
    public class StockMovementsControllerTests
    {
        private readonly Mock<IStockMovementService> _mockStockMovementService;
        private readonly StockMovementsController _controller;

        public StockMovementsControllerTests()
        {
            _mockStockMovementService = new Mock<IStockMovementService>();
            _controller = new StockMovementsController(_mockStockMovementService.Object);
        }

        [Fact]
        public async Task GetAllStockMovements_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<StockMovementGetDto>
        {
            new() { Id = 1, AppUserId = "user-1" },
            new() { Id = 2, AppUserId = "user-2" }
        };
            _mockStockMovementService.Setup(s => s.GetAllIncludingAsync()).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllStockMovements();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAllStockMovements_ReturnsOk_WithEmptyList()
        {
            // Arrange
            _mockStockMovementService.Setup(s => s.GetAllIncludingAsync()).ReturnsAsync(new List<StockMovementGetDto>());

            // Act
            var result = await _controller.GetAllStockMovements();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            (ok!.Value as List<StockMovementGetDto>).Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllStockMovementsByUserId_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<StockMovementGetDto>
        {
            new() { Id = 1, AppUserId = "user-1" }
        };
            _mockStockMovementService.Setup(s => s.GetAllIncludingByUserIdAsync("user-1")).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllStockMovementsByUserId("user-1");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAllStockMovementsByUserId_ReturnsOk_WithEmptyList_WhenUserNotFound()
        {
            // Arrange
            _mockStockMovementService.Setup(s => s.GetAllIncludingByUserIdAsync("nonexistent")).ReturnsAsync(new List<StockMovementGetDto>());

            // Act
            var result = await _controller.GetAllStockMovementsByUserId("nonexistent");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            (ok!.Value as List<StockMovementGetDto>).Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllStockMovementsByProductId_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<StockMovementGetDto>
        {
            new() { Id = 1, ProductId = 5 }
        };
            _mockStockMovementService.Setup(s => s.GetAllIncludingByProductIdAsync(5)).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllStockMovementsByProductId(5);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAllStockMovementsByProductId_ReturnsOk_WithEmptyList_WhenProductNotFound()
        {
            // Arrange
            _mockStockMovementService.Setup(s => s.GetAllIncludingByProductIdAsync(99)).ReturnsAsync(new List<StockMovementGetDto>());

            // Act
            var result = await _controller.GetAllStockMovementsByProductId(99);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            (ok!.Value as List<StockMovementGetDto>).Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllStockMovementsRange_ReturnsOk_WithList()
        {
            // Arrange
            var start = new DateTime(2026, 1, 1);
            var end = new DateTime(2026, 2, 1);
            var fakeData = new List<StockMovementGetDto>
        {
            new() { Id = 1, AppUserId = "user-1" },
            new() { Id = 2, AppUserId = "user-2" }
        };
            _mockStockMovementService.Setup(s => s.GetAllIncludingRangeAsync(start, end)).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllStockMovementsRange(start, end);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAllStockMovementsRange_ReturnsOk_WithEmptyList_WhenNoDataInRange()
        {
            // Arrange
            var start = new DateTime(2020, 1, 1);
            var end = new DateTime(2020, 1, 2);
            _mockStockMovementService.Setup(s => s.GetAllIncludingRangeAsync(start, end)).ReturnsAsync(new List<StockMovementGetDto>());

            // Act
            var result = await _controller.GetAllStockMovementsRange(start, end);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            (ok!.Value as List<StockMovementGetDto>).Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllWholeStockMovements_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<StockMovementGetDto>
        {
            new() { Id = 1, AppUserId = "user-1" }
        };
            _mockStockMovementService.Setup(s => s.GetAllIncludingByAllDataAsync()).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllWholeStockMovements();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetStockMovementById_ReturnsOk_WhenFound()
        {
            // Arrange
            var fakeDto = new StockMovementGetDto { Id = 1, AppUserId = "user-1" };
            _mockStockMovementService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(fakeDto);

            // Act
            var result = await _controller.GetStockMovementById(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeDto);
        }

        [Fact]
        public async Task GetStockMovementById_ReturnsNotFound_WhenNull()
        {
            // Arrange
            _mockStockMovementService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((StockMovementGetDto?)null);

            // Act
            var result = await _controller.GetStockMovementById(99);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockStockMovementService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(successResult);

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
            _mockStockMovementService.Setup(s => s.DeleteAsync(99)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.Delete(99);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DeleteStockMovementById_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var ids = new List<int> { 1, 2, 3 };
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockStockMovementService.Setup(s => s.DeleteByIdAsync(ids)).ReturnsAsync(successResult);

            // Act
            var result = await _controller.DeleteStockMovementById(ids);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteStockMovementById_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var ids = new List<int> { 99, 100 };
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "Bulk deletion failed." };
            _mockStockMovementService.Setup(s => s.DeleteByIdAsync(ids)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.DeleteStockMovementById(ids);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetActive_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockStockMovementService.Setup(s => s.SetActiveAsync(1)).ReturnsAsync(successResult);

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
            _mockStockMovementService.Setup(s => s.SetActiveAsync(99)).ReturnsAsync(failResult);

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
            _mockStockMovementService.Setup(s => s.SetInActiveAsync(1)).ReturnsAsync(successResult);

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
            _mockStockMovementService.Setup(s => s.SetInActiveAsync(99)).ReturnsAsync(failResult);

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
            _mockStockMovementService.Setup(s => s.SetDeletedAsync(1)).ReturnsAsync(successResult);

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
            _mockStockMovementService.Setup(s => s.SetDeletedAsync(99)).ReturnsAsync(failResult);

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
            _mockStockMovementService.Setup(s => s.SetNotDeletedAsync(1)).ReturnsAsync(successResult);

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
            _mockStockMovementService.Setup(s => s.SetNotDeletedAsync(99)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.SetNotDeleted(99);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
