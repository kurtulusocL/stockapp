using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockManagement.API.Controllers;
using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Business.Services.Abstract;
using StockManagement.Shared.Dtos.MappingDtos.UserSessionDtos;

namespace StockManagement.Tests
{
    public class UserSessionsControllerTests
    {
        private readonly Mock<IUserSessionService> _mockUserSessionService;
        private readonly UserSessionsController _controller;

        public UserSessionsControllerTests()
        {
            _mockUserSessionService = new Mock<IUserSessionService>();
            _controller = new UserSessionsController(_mockUserSessionService.Object);
        }

        [Fact]
        public async Task GetAllUserSessions_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<UserSessionGetDto>
        {
            new() { Id = 1, AppUserId = "user-1" },
            new() { Id = 2, AppUserId = "user-2" }
        };
            _mockUserSessionService.Setup(s => s.GetAllIncludingAsync()).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllUserSessions();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAllUserSessions_ReturnsOk_WithEmptyList()
        {
            // Arrange
            _mockUserSessionService.Setup(s => s.GetAllIncludingAsync()).ReturnsAsync(new List<UserSessionGetDto>());

            // Act
            var result = await _controller.GetAllUserSessions();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            (ok!.Value as List<UserSessionGetDto>).Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllUserSessionsByUserId_ReturnsOk_WithList()
        {
            // Arrange
            var userId = "user-1";
            var fakeData = new List<UserSessionGetDto>
        {
            new() { Id = 1, AppUserId = userId },
            new() { Id = 2, AppUserId = userId }
        };
            _mockUserSessionService.Setup(s => s.GetAllIncludingByUserIdAsync(userId)).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllUserSessionsByUserId(userId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAllUserSessionsByUserId_ReturnsOk_WithEmptyList_WhenUserNotFound()
        {
            // Arrange
            _mockUserSessionService.Setup(s => s.GetAllIncludingByUserIdAsync("nonexistent")).ReturnsAsync(new List<UserSessionGetDto>());

            // Act
            var result = await _controller.GetAllUserSessionsByUserId("nonexistent");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            (ok!.Value as List<UserSessionGetDto>).Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllUserSessionsByLoginDate_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<UserSessionGetDto>
        {
            new() { Id = 1, AppUserId = "user-1" }
        };
            _mockUserSessionService.Setup(s => s.GetAllIncludingByLoginDateAsync()).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllUserSessionsByLoginDate();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAllUserSessionsByOnline_ReturnsOk_WithOnlineSessions()
        {
            // Arrange
            var fakeData = new List<UserSessionGetDto>
        {
            new() { Id = 1, AppUserId = "user-1" },
            new() { Id = 2, AppUserId = "user-2" }
        };
            _mockUserSessionService.Setup(s => s.GetAllIncludingByOnlineAsync()).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllUserSessionsByOnline();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAllUserSessionsByOnline_ReturnsOk_WithEmptyList_WhenNoneOnline()
        {
            // Arrange
            _mockUserSessionService.Setup(s => s.GetAllIncludingByOnlineAsync()).ReturnsAsync(new List<UserSessionGetDto>());

            // Act
            var result = await _controller.GetAllUserSessionsByOnline();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            (ok!.Value as List<UserSessionGetDto>).Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllUserSessionsByOffline_ReturnsOk_WithOfflineSessions()
        {
            // Arrange
            var fakeData = new List<UserSessionGetDto>
        {
            new() { Id = 1, AppUserId = "user-3" }
        };
            _mockUserSessionService.Setup(s => s.GetAllIncludingByOfflineAsync()).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllUserSessionsByOffline();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAllUserSessionsByOffline_ReturnsOk_WithEmptyList_WhenNoneOffline()
        {
            // Arrange
            _mockUserSessionService.Setup(s => s.GetAllIncludingByOfflineAsync()).ReturnsAsync(new List<UserSessionGetDto>());

            // Act
            var result = await _controller.GetAllUserSessionsByOffline();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            (ok!.Value as List<UserSessionGetDto>).Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllIncludingWholeUserSessions_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<UserSessionGetDto>
        {
            new() { Id = 1, AppUserId = "user-1" }
        };
            _mockUserSessionService.Setup(s => s.GetAllIncludingByAllDataAsync()).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllIncludingWholeUserSessions();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenFound()
        {
            // Arrange
            var fakeDto = new UserSessionGetDto { Id = 1, AppUserId = "user-1" };
            _mockUserSessionService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(fakeDto);

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
            _mockUserSessionService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((UserSessionGetDto?)null);

            // Act
            var result = await _controller.GetById(99);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockUserSessionService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(successResult);

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
            _mockUserSessionService.Setup(s => s.DeleteAsync(99)).ReturnsAsync(failResult);

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
            _mockUserSessionService.Setup(s => s.DeleteByIdAsync(ids)).ReturnsAsync(successResult);

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
            _mockUserSessionService.Setup(s => s.DeleteByIdAsync(ids)).ReturnsAsync(failResult);

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
            _mockUserSessionService.Setup(s => s.SetActiveAsync(1)).ReturnsAsync(successResult);

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
            _mockUserSessionService.Setup(s => s.SetActiveAsync(99)).ReturnsAsync(failResult);

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
            _mockUserSessionService.Setup(s => s.SetInActiveAsync(1)).ReturnsAsync(successResult);

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
            _mockUserSessionService.Setup(s => s.SetInActiveAsync(99)).ReturnsAsync(failResult);

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
            _mockUserSessionService.Setup(s => s.SetDeletedAsync(1)).ReturnsAsync(successResult);

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
            _mockUserSessionService.Setup(s => s.SetDeletedAsync(99)).ReturnsAsync(failResult);

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
            _mockUserSessionService.Setup(s => s.SetNotDeletedAsync(1)).ReturnsAsync(successResult);

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
            _mockUserSessionService.Setup(s => s.SetNotDeletedAsync(99)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.SetNotDeleted(99);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
