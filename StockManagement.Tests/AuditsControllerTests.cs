using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockManagement.API.Controllers;
using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Business.Services.Abstract;
using StockManagement.Shared.Dtos.MappingDtos.AuditDtos;

namespace StockManagement.Tests
{
    public class AuditsControllerTests
    {
        private readonly Mock<IAuditService> _mockAuditService;
        private readonly AuditsController _controller;

        public AuditsControllerTests()
        {
            _mockAuditService = new Mock<IAuditService>();
            _controller = new AuditsController(_mockAuditService.Object);
        }

        [Fact]
        public async Task GetAllAudits_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<AuditGetDto>
        {
            new() { Id = 1, AppUserId = "user-1" },
            new() { Id = 2, AppUserId = "user-2" }
        };
            _mockAuditService.Setup(s => s.GetAllIncludingAsync()).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllAudits();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAllAudits_ReturnsOk_WithEmptyList()
        {
            // Arrange
            _mockAuditService.Setup(s => s.GetAllIncludingAsync()).ReturnsAsync(new List<AuditGetDto>());

            // Act
            var result = await _controller.GetAllAudits();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            (ok!.Value as List<AuditGetDto>).Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllAuditsByUserId_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<AuditGetDto>
        {
            new() { Id = 1, AppUserId = "user-1" },
            new() { Id = 2, AppUserId = "user-1" }
        };
            _mockAuditService.Setup(s => s.GetAllIncludingByUserIdAsync("user-1")).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllAuditsByUserId("user-1");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAllAuditsByUserId_ReturnsOk_WithEmptyList_WhenUserNotFound()
        {
            // Arrange
            _mockAuditService.Setup(s => s.GetAllIncludingByUserIdAsync("nonexistent")).ReturnsAsync(new List<AuditGetDto>());

            // Act
            var result = await _controller.GetAllAuditsByUserId("nonexistent");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            (ok!.Value as List<AuditGetDto>).Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllAuditsByUserId_ReturnsOk_WhenIdIsNull()
        {
            // Arrange
            _mockAuditService.Setup(s => s.GetAllIncludingByUserIdAsync(null)).ReturnsAsync(new List<AuditGetDto>());

            // Act
            var result = await _controller.GetAllAuditsByUserId(null);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetAllWholeAudits_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<AuditGetDto>
        {
            new() { Id = 1, AppUserId = "user-1" }
        };
            _mockAuditService.Setup(s => s.GetAllIncludingAllDataAsync()).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllWholeAudits();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAuditById_ReturnsOk_WhenFound()
        {
            // Arrange
            var fakeDto = new AuditGetDto { Id = 1, AppUserId = "user-1" };
            _mockAuditService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(fakeDto);

            // Act
            var result = await _controller.GetAuditById(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeDto);
        }

        [Fact]
        public async Task GetAuditById_ReturnsNotFound_WhenNull()
        {
            // Arrange
            _mockAuditService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((AuditGetDto?)null);

            // Act
            var result = await _controller.GetAuditById(99);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockAuditService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(successResult);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Delete_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "It couldn't be deleted.." };
            _mockAuditService.Setup(s => s.DeleteAsync(99)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.Delete(99);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DeleteAuditById_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var ids = new List<int> { 1, 2, 3 };
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockAuditService.Setup(s => s.DeleteAllByIdAsync(ids)).ReturnsAsync(successResult);

            // Act
            var result = await _controller.DeleteAuditById(ids);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteAuditById_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var ids = new List<int> { 99, 100 };
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "Bulk deletion failed." };
            _mockAuditService.Setup(s => s.DeleteAllByIdAsync(ids)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.DeleteAuditById(ids);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetActive_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockAuditService.Setup(s => s.SetActiveAsync(1)).ReturnsAsync(successResult);

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
            _mockAuditService.Setup(s => s.SetActiveAsync(99)).ReturnsAsync(failResult);

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
            _mockAuditService.Setup(s => s.SetInActiveAsync(1)).ReturnsAsync(successResult);

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
            _mockAuditService.Setup(s => s.SetInActiveAsync(99)).ReturnsAsync(failResult);

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
            _mockAuditService.Setup(s => s.SetDeletedAsync(1)).ReturnsAsync(successResult);

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
            _mockAuditService.Setup(s => s.SetDeletedAsync(99)).ReturnsAsync(failResult);

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
            _mockAuditService.Setup(s => s.SetNotDeletedAsync(1)).ReturnsAsync(successResult);

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
            _mockAuditService.Setup(s => s.SetNotDeletedAsync(99)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.SetNotDeleted(99);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
