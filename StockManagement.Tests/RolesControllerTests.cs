using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockManagement.API.Controllers;
using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Business.Services.Abstract;
using StockManagement.Shared.Dtos.MappingDtos.AppRoleDtos;

namespace StockManagement.Tests
{
    public class RolesControllerTests
    {
        private readonly Mock<IRoleService> _mockRoleService;
        private readonly RolesController _controller;

        public RolesControllerTests()
        {
            _mockRoleService = new Mock<IRoleService>();
            _controller = new RolesController(_mockRoleService.Object);
        }

        [Fact]
        public async Task GetAllRoles_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<AppRoleGetDto>
        {
            new() { Id = "role-1", Name = "Admin" },
            new() { Id = "role-2", Name = "Worker" }
        };
            _mockRoleService.Setup(s => s.GetAllIncludingAsync()).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllRoles();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAllRoles_ReturnsOk_WithEmptyList()
        {
            // Arrange
            _mockRoleService.Setup(s => s.GetAllIncludingAsync()).ReturnsAsync(new List<AppRoleGetDto>());

            // Act
            var result = await _controller.GetAllRoles();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            (ok!.Value as List<AppRoleGetDto>).Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllWholeRoles_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<AppRoleGetDto>
        {
            new() { Id = "role-1", Name = "Admin" }
        };
            _mockRoleService.Setup(s => s.GetAllIncludingAllDataAsync()).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllWholeRoles();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenFound()
        {
            // Arrange
            var fakeDto = new AppRoleGetDto { Id = "role-1", Name = "Admin" };
            _mockRoleService.Setup(s => s.GetByIdAsync("role-1")).ReturnsAsync(fakeDto);

            // Act
            var result = await _controller.GetById("role-1");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeDto);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenNull()
        {
            // Arrange
            _mockRoleService.Setup(s => s.GetByIdAsync("nonexistent")).ReturnsAsync((AppRoleGetDto?)null);

            // Act
            var result = await _controller.GetById("nonexistent");

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetForEdit_ReturnsOk_WhenFound()
        {
            // Arrange
            var fakeDto = new AppRoleUpdateDto { Id = "role-1", Name = "Admin" };
            _mockRoleService.Setup(s => s.GetForEditAsync("role-1")).ReturnsAsync(fakeDto);

            // Act
            var result = await _controller.GetForEdit("role-1");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeDto);
        }

        [Fact]
        public async Task GetForEdit_ReturnsNotFound_WhenNull()
        {
            // Arrange
            _mockRoleService.Setup(s => s.GetForEditAsync("nonexistent")).ReturnsAsync((AppRoleUpdateDto?)null);

            // Act
            var result = await _controller.GetForEdit("nonexistent");

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task RoleCreate_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var dto = new AppRoleCreateDto { Name = "Supervisor" };
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockRoleService.Setup(s => s.CrateAsync(dto)).ReturnsAsync(successResult);

            // Act
            var result = await _controller.RoleCreate(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task RoleCreate_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var dto = new AppRoleCreateDto { Name = "Supervisor" };
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "The role could not be created." };
            _mockRoleService.Setup(s => s.CrateAsync(dto)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.RoleCreate(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UpdateRole_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var dto = new AppRoleUpdateDto { Id = "role-1", Name = "SuperAdmin" };
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockRoleService.Setup(s => s.UpdateAsync(dto)).ReturnsAsync(successResult);

            // Act
            var result = await _controller.UpdateRole(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task UpdateRole_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var dto = new AppRoleUpdateDto { Id = "role-1", Name = "SuperAdmin" };
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "Could not be updated.." };
            _mockRoleService.Setup(s => s.UpdateAsync(dto)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.UpdateRole(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockRoleService.Setup(s => s.DeleteAsync("role-1")).ReturnsAsync(successResult);

            // Act
            var result = await _controller.Delete("role-1");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Delete_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "It couldn't be deleted.." };
            _mockRoleService.Setup(s => s.DeleteAsync("nonexistent")).ReturnsAsync(failResult);

            // Act
            var result = await _controller.Delete("nonexistent");

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DeleteById_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var ids = new List<string> { "role-1", "role-2" };
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockRoleService.Setup(s => s.DeleteByIdAsync(ids)).ReturnsAsync(successResult);

            // Act
            var result = await _controller.DeleteById(ids);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteById_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var ids = new List<string> { "nonexistent-1", "nonexistent-2" };
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "Bulk deletion failed." };
            _mockRoleService.Setup(s => s.DeleteByIdAsync(ids)).ReturnsAsync(failResult);

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
            _mockRoleService.Setup(s => s.SetActiveAsync("role-1")).ReturnsAsync(successResult);

            // Act
            var result = await _controller.SetActive("role-1");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetActive_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "Activation failed." };
            _mockRoleService.Setup(s => s.SetActiveAsync("nonexistent")).ReturnsAsync(failResult);

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
            _mockRoleService.Setup(s => s.SetInActiveAsync("role-1")).ReturnsAsync(successResult);

            // Act
            var result = await _controller.SetInActive("role-1");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetInActive_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "It couldn't be made passive." };
            _mockRoleService.Setup(s => s.SetInActiveAsync("nonexistent")).ReturnsAsync(failResult);

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
            _mockRoleService.Setup(s => s.SetDeletedAsync("role-1")).ReturnsAsync(successResult);

            // Act
            var result = await _controller.SetDeleted("role-1");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetDeleted_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "Soft delete failed." };
            _mockRoleService.Setup(s => s.SetDeletedAsync("nonexistent")).ReturnsAsync(failResult);

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
            _mockRoleService.Setup(s => s.SetNotDeletedAsync("role-1")).ReturnsAsync(successResult);

            // Act
            var result = await _controller.SetNotDeleted("role-1");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetNotDeleted_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "Restore failed." };
            _mockRoleService.Setup(s => s.SetNotDeletedAsync("nonexistent")).ReturnsAsync(failResult);

            // Act
            var result = await _controller.SetNotDeleted("nonexistent");

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
