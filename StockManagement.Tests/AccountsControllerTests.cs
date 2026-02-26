using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockManagement.API.Controllers;
using StockManagement.Business.Services.Abstract;
using StockManagement.Shared.Dtos.AuthDtos;
using StockManagement.Shared.ViewModels.RoleVM;

namespace StockManagement.Tests
{
    public class AccountsControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AccountsController _controller;

        public AccountsControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AccountsController(_mockAuthService.Object);
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
        public async Task Login_ReturnsOk_WhenTokenReturned()
        {
            // Arrange
            var dto = new LoginDto { Email = "kurtulus@gmail.com", Password = "Pass123!" };
            _mockAuthService.Setup(s => s.LoginAsync(dto)).ReturnsAsync("jwt-token-123");

            // Act
            var result = await _controller.Login(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenTokenIsNull()
        {
            // Arrange
            var dto = new LoginDto { Email = "kurtulus@gmail.com", Password = "WrongPass" };
            _mockAuthService.Setup(s => s.LoginAsync(dto)).ReturnsAsync((string?)null);

            // Act
            var result = await _controller.Login(dto);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task LoginWithConfirmCode_ReturnsOk_WithToken_WhenSuccess()
        {
            // Arrange
            var dto = new LoginDto { Email = "kurtulus@gmail.com", Password = "Pass123!" };
            _mockAuthService.Setup(s => s.LoginWithConfirmCodeAsync(dto)).ReturnsAsync("jwt-token-123");

            // Act
            var result = await _controller.LoginWithConfirmCode(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            var response = ok!.Value as LoginResponseDto;
            response!.ConfirmRequired.Should().BeFalse();
            response.Token.Should().Be("jwt-token-123");
        }

        [Fact]
        public async Task LoginWithConfirmCode_ReturnsOk_WithConfirmRequired_WhenConfirmNeeded()
        {
            // Arrange
            var dto = new LoginDto { Email = "kurtulus@gmail.com", Password = "Pass123!" };
            _mockAuthService.Setup(s => s.LoginWithConfirmCodeAsync(dto)).ReturnsAsync("confirm_required");

            // Act
            var result = await _controller.LoginWithConfirmCode(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            var response = ok!.Value as LoginResponseDto;
            response!.ConfirmRequired.Should().BeTrue();
        }

        [Fact]
        public async Task LoginWithConfirmCode_ReturnsUnauthorized_WhenResultIsNull()
        {
            // Arrange
            var dto = new LoginDto { Email = "kurtulus@gmail.com", Password = "WrongPass" };
            _mockAuthService.Setup(s => s.LoginWithConfirmCodeAsync(dto)).ReturnsAsync((string?)null);

            // Act
            var result = await _controller.LoginWithConfirmCode(dto);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task VerifyLoginConfirmCode_ReturnsOk_WhenTokenReturned()
        {
            // Arrange
            var dto = new LoginConfirmCodeDto { Email = "kurtulus@gmail.com", LoginConfirmCode = 123456 };
            _mockAuthService.Setup(s => s.VerifyLoginConfirmCodeAsync(dto)).ReturnsAsync("jwt-token-123");

            // Act
            var result = await _controller.VerifyLoginConfirmCode(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            var response = ok!.Value as LoginResponseDto;
            response!.Token.Should().Be("jwt-token-123");
        }

        [Fact]
        public async Task VerifyLoginConfirmCode_ReturnsBadRequest_WhenTokenIsNull()
        {
            // Arrange
            var dto = new LoginConfirmCodeDto { Email = "kurtulus@gmail.com", LoginConfirmCode = 000000 };
            _mockAuthService.Setup(s => s.VerifyLoginConfirmCodeAsync(dto)).ReturnsAsync((string?)null);

            // Act
            var result = await _controller.VerifyLoginConfirmCode(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var dto = new RegisterDto { NameSurname = "Kurtulus Ocal", Email = "new@test.com", Password = "Pass123!" };
            _mockAuthService.Setup(s => s.RegisterAsync(dto)).ReturnsAsync(true);

            // Act
            var result = await _controller.Register(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var dto = new RegisterDto { NameSurname = "Kurtulus Ocal", Email = "new@test.com", Password = "Pass123!" };
            _mockAuthService.Setup(s => s.RegisterAsync(dto)).ReturnsAsync(false);

            // Act
            var result = await _controller.Register(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ConfirmMail_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var dto = new ConfirmCodeDto { Email = "kurtulus@test.com", ConfirmCode = 123456 };
            _mockAuthService.Setup(s => s.ConfirmMailAsync(dto)).ReturnsAsync(true);

            // Act
            var result = await _controller.ConfirmMail(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task ConfirmMail_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var dto = new ConfirmCodeDto { Email = "kurtulus@test.com", ConfirmCode = 123987 };
            _mockAuthService.Setup(s => s.ConfirmMailAsync(dto)).ReturnsAsync(false);

            // Act
            var result = await _controller.ConfirmMail(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ChangePassword_ReturnsOk_WhenSuccess()
        {
            // Arrange
            SetUser("user-1");
            var dto = new ChangePasswordDto { CurrentPassword = "OldPass123!", NewPassword = "NewPass123!" };
            _mockAuthService.Setup(s => s.ChangePasswordAsync(dto, "user-1")).ReturnsAsync(true);

            // Act
            var result = await _controller.ChangePassword(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task ChangePassword_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            SetUser("user-1");
            var dto = new ChangePasswordDto { CurrentPassword = "WrongOld!", NewPassword = "NewPass123!" };
            _mockAuthService.Setup(s => s.ChangePasswordAsync(dto, "user-1")).ReturnsAsync(false);

            // Act
            var result = await _controller.ChangePassword(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ChangePassword_ReturnsUnauthorized_WhenUserIdIsNull()
        {
            // Arrange
            SetUserWithNoClaims();
            var dto = new ChangePasswordDto { CurrentPassword = "OldPass123!", NewPassword = "NewPass123!" };

            // Act
            var result = await _controller.ChangePassword(dto);

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task ResetPassword_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var dto = new ResetPasswordDto { NewPassword = "NewPass123!" };
            _mockAuthService.Setup(s => s.ResetPassword(dto, "reset-code-abc")).ReturnsAsync(true);

            // Act
            var result = await _controller.ResetPassword(dto, "reset-code-abc");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task ResetPassword_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var dto = new ResetPasswordDto { NewPassword = "NewPass123!" };
            _mockAuthService.Setup(s => s.ResetPassword(dto, "invalid-code")).ReturnsAsync(false);

            // Act
            var result = await _controller.ResetPassword(dto, "invalid-code");

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetDataUpdateProfile_ReturnsOk_WhenUserFound()
        {
            // Arrange
            SetUser("user-1");
            var fakeDto = new UpdateProfileDto { Email = "Kurtulus@gmail.com" };
            _mockAuthService.Setup(s => s.GetDataUpdateProfileAsync("user-1")).ReturnsAsync(fakeDto);

            // Act
            var result = await _controller.GetDataUpdateProfile();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeDto);
        }

        [Fact]
        public async Task GetDataUpdateProfile_ReturnsUnauthorized_WhenUserIdIsNull()
        {
            // Arrange
            SetUserWithNoClaims();

            // Act
            var result = await _controller.GetDataUpdateProfile();

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task UpdateProfile_ReturnsOk_WhenSuccess()
        {
            // Arrange
            SetUser("user-1");
            var dto = new UpdateProfileDto { Email = "Kurtulus@gmail.com" };
            _mockAuthService.Setup(s => s.UpdateProfileAsync(dto, "user-1")).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateProfile(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task UpdateProfile_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            SetUser("user-1");
            var dto = new UpdateProfileDto { Email = "Kurtulus@gmail.com" };
            _mockAuthService.Setup(s => s.UpdateProfileAsync(dto, "user-1")).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateProfile(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UpdateProfile_ReturnsUnauthorized_WhenUserIdIsNull()
        {
            // Arrange
            SetUserWithNoClaims();
            var dto = new UpdateProfileDto { Email = "Kurtulus@gmail.com" };

            // Act
            var result = await _controller.UpdateProfile(dto);

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task GetRoleAssign_ReturnsOk_WhenFound()
        {
            // Arrange
            var fakeData = new List<RoleAssignVM>
        {
            new() { RoleId = "role-1", RoleName = "Admin", HasAssign = true }
        };
            _mockAuthService.Setup(s => s.GetRoleAssignAsync("user-1")).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetRoleAssign("user-1");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetRoleAssign_ReturnsNotFound_WhenNull()
        {
            // Arrange
            _mockAuthService.Setup(s => s.GetRoleAssignAsync("nonexistent")).ReturnsAsync((List<RoleAssignVM>?)null);

            // Act
            var result = await _controller.GetRoleAssign("nonexistent");

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task RoleAssign_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var modelList = new List<RoleAssignVM>
        {
            new() { RoleId = "role-1", RoleName = "Admin", HasAssign = true }
        };
            _mockAuthService.Setup(s => s.RoleAssignAsync(modelList, "user-1")).ReturnsAsync(true);

            // Act
            var result = await _controller.RoleAssign(modelList, "user-1");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task RoleAssign_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var modelList = new List<RoleAssignVM>
        {
            new() { RoleId = "role-1", RoleName = "Admin", HasAssign = false }
        };
            _mockAuthService.Setup(s => s.RoleAssignAsync(modelList, "user-1")).ReturnsAsync(false);

            // Act
            var result = await _controller.RoleAssign(modelList, "user-1");

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Logout_ReturnsOk_WhenSuccess()
        {
            // Arrange
            SetUser("user-1");
            _mockAuthService.Setup(s => s.LogoutAsync("user-1")).ReturnsAsync(true);

            // Act
            var result = await _controller.Logout();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Logout_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            SetUser("user-1");
            _mockAuthService.Setup(s => s.LogoutAsync("user-1")).ReturnsAsync(false);

            // Act
            var result = await _controller.Logout();

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Logout_ReturnsUnauthorized_WhenUserIdIsNull()
        {
            // Arrange
            SetUserWithNoClaims();

            // Act
            var result = await _controller.Logout();

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
        }
    }
}
