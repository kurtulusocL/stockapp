using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockManagement.API.Controllers;
using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Business.Services.Abstract;
using StockManagement.Shared.Dtos.MappingDtos.AppUserDtos;

namespace StockManagement.Tests
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserSevice> _mockUserService;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockUserService = new Mock<IUserSevice>();
            _controller = new UsersController(_mockUserService.Object);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<AppUserGetDto>
        {
            new() { Id = "user-1", UserName = "kurtulus" },
            new() { Id = "user-2", UserName = "ocal" }
        };
            _mockUserService.Setup(s => s.GetAllIncludingAsync()).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOk_WithEmptyList()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetAllIncludingAsync()).ReturnsAsync(new List<AppUserGetDto>());

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            (ok!.Value as List<AppUserGetDto>).Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllUsersByActiveLoginCode_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<AppUserGetDto>
        {
            new() { Id = "user-1", UserName = "kurtulus" }
        };
            _mockUserService.Setup(s => s.GetAllIncludingByActiveLoginCodeAsync()).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllUsersByActiveLoginCode();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAllUsersByActiveLoginCode_ReturnsOk_WithEmptyList()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetAllIncludingByActiveLoginCodeAsync()).ReturnsAsync(new List<AppUserGetDto>());

            // Act
            var result = await _controller.GetAllUsersByActiveLoginCode();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            (ok!.Value as List<AppUserGetDto>).Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllUsersByInActiveLoginCode_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<AppUserGetDto>
        {
            new() { Id = "user-2", UserName = "ocal" }
        };
            _mockUserService.Setup(s => s.GetAllIncludingByInActiveLoginCodeAsync()).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllUsersByInActiveLoginCode();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAllUsersByInActiveLoginCode_ReturnsOk_WithEmptyList()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetAllIncludingByInActiveLoginCodeAsync()).ReturnsAsync(new List<AppUserGetDto>());

            // Act
            var result = await _controller.GetAllUsersByInActiveLoginCode();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            (ok!.Value as List<AppUserGetDto>).Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllWholeUsers_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<AppUserGetDto>
        {
            new() { Id = "user-1", UserName = "kurtulus" }
        };
            _mockUserService.Setup(s => s.GetAllIncludingByAllDataAsync()).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllWholeUsers();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        // ──────────────────────────────────────────────
        // GET: {id} - async
        // ──────────────────────────────────────────────

        [Fact]
        public async Task GetUserById_ReturnsOk_WhenFound()
        {
            // Arrange
            var fakeDto = new AppUserGetDto { Id = "user-1", UserName = "kurtulus" };
            _mockUserService.Setup(s => s.GetByIdAsync("user-1")).ReturnsAsync(fakeDto);

            // Act
            var result = await _controller.GetUserById("user-1");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeDto);
        }

        [Fact]
        public async Task GetUserById_ReturnsNotFound_WhenNull()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetByIdAsync("nonexistent")).ReturnsAsync((AppUserGetDto?)null);

            // Act
            var result = await _controller.GetUserById("nonexistent");

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void GetByUserId_ReturnsOk_WhenFound()
        {
            // Arrange
            var fakeDto = new AppUserGetDto { Id = "user-1", UserName = "kurtulus" };
            _mockUserService.Setup(s => s.GetById("user-1")).Returns(fakeDto);

            // Act
            var result = _controller.GetByUserId("user-1");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeDto);
        }

        [Fact]
        public void GetByUserId_ReturnsNotFound_WhenNull()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetById("nonexistent")).Returns((AppUserGetDto?)null);

            // Act
            var result = _controller.GetByUserId("nonexistent");

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockUserService.Setup(s => s.DeleteAsync("user-1")).ReturnsAsync(successResult);

            // Act
            var result = await _controller.Delete("user-1");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Delete_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "It couldn't be deleted.." };
            _mockUserService.Setup(s => s.DeleteAsync("nonexistent")).ReturnsAsync(failResult);

            // Act
            var result = await _controller.Delete("nonexistent");

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DeleteUserById_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var ids = new List<string> { "user-1", "user-2" };
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockUserService.Setup(s => s.DeleteByIdAsync(ids)).ReturnsAsync(successResult);

            // Act
            var result = await _controller.DeleteUserById(ids);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteUserById_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var ids = new List<string> { "nonexistent-1", "nonexistent-2" };
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "Bulk deletion failed." };
            _mockUserService.Setup(s => s.DeleteByIdAsync(ids)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.DeleteUserById(ids);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetActiveLoginConfirmCode_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockUserService.Setup(s => s.SetActiveLoginConfirmCodeAsync("user-1")).ReturnsAsync(successResult);

            // Act
            var result = await _controller.SetActiveLoginConfirmCode("user-1");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetActiveLoginConfirmCode_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "Could not be activated." };
            _mockUserService.Setup(s => s.SetActiveLoginConfirmCodeAsync("nonexistent")).ReturnsAsync(failResult);

            // Act
            var result = await _controller.SetActiveLoginConfirmCode("nonexistent");

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetInActiveLoginConfirmCode_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockUserService.Setup(s => s.SetInActiveLoginConfirmCodeAsync("user-1")).ReturnsAsync(successResult);

            // Act
            var result = await _controller.SetInActiveLoginConfirmCode("user-1");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetInActiveLoginConfirmCode_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "It couldn't be made passive." };
            _mockUserService.Setup(s => s.SetInActiveLoginConfirmCodeAsync("nonexistent")).ReturnsAsync(failResult);

            // Act
            var result = await _controller.SetInActiveLoginConfirmCode("nonexistent");

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetActive_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockUserService.Setup(s => s.SetActiveAsync("user-1")).ReturnsAsync(successResult);

            // Act
            var result = await _controller.SetActive("user-1");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetActive_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "Activation failed." };
            _mockUserService.Setup(s => s.SetActiveAsync("nonexistent")).ReturnsAsync(failResult);

            // Act
            var result = await _controller.SetActive("nonexistent");

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetInActive_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockUserService.Setup(s => s.SetInActiveAsync("user-1")).ReturnsAsync(successResult);

            // Act
            var result = await _controller.SetInActive("user-1");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetInActive_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "It couldn't be made passive." };
            _mockUserService.Setup(s => s.SetInActiveAsync("nonexistent")).ReturnsAsync(failResult);

            // Act
            var result = await _controller.SetInActive("nonexistent");

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetDeleted_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockUserService.Setup(s => s.SetDeletedAsync("user-1")).ReturnsAsync(successResult);

            // Act
            var result = await _controller.SetDeleted("user-1");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetDeleted_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "Soft delete failed." };
            _mockUserService.Setup(s => s.SetDeletedAsync("nonexistent")).ReturnsAsync(failResult);

            // Act
            var result = await _controller.SetDeleted("nonexistent");

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetNotDeleted_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockUserService.Setup(s => s.SetNotDeletedAsync("user-1")).ReturnsAsync(successResult);

            // Act
            var result = await _controller.SetNotDeleted("user-1");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetNotDeleted_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "Restore failed." };
            _mockUserService.Setup(s => s.SetNotDeletedAsync("nonexistent")).ReturnsAsync(failResult);

            // Act
            var result = await _controller.SetNotDeleted("nonexistent");

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
