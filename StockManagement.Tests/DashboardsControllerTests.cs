using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockManagement.API.Controllers;
using StockManagement.Business.Services.Abstract;
using StockManagement.Shared.Dtos.MappingDtos.AppUserDtos;

namespace StockManagement.Tests
{
    public class DashboardsControllerTests
    {
        private readonly Mock<IUserSevice> _mockUserService;
        private readonly DashboardsController _controller;

        public DashboardsControllerTests()
        {
            _mockUserService = new Mock<IUserSevice>();
            _controller = new DashboardsController(_mockUserService.Object);
        }

        private void SetUser(string userId)
        {
            var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId)
        };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        private void SetUserWithNoClaims()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };
        }

        [Fact]
        public async Task GetMe_ReturnsOk_WhenUserFound()
        {
            // Arrange
            SetUser("user-1");
            var fakeDto = new AppUserGetDto { Id = "user-1", UserName = "kurtulus" };
            _mockUserService.Setup(s => s.GetByIdAsync("user-1")).ReturnsAsync(fakeDto);

            // Act
            var result = await _controller.GetMe();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeDto);
        }

        [Fact]
        public async Task GetMe_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            SetUser("user-1");
            _mockUserService.Setup(s => s.GetByIdAsync("user-1")).ReturnsAsync((AppUserGetDto?)null);

            // Act
            var result = await _controller.GetMe();

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetMe_ReturnsUnauthorized_WhenUserIdIsNull()
        {
            // Arrange
            SetUserWithNoClaims();

            // Act
            var result = await _controller.GetMe();

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
        }
    }
}
