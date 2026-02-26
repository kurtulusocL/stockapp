using StockManagement.Shared.Dtos.AuthDtos;
using StockManagement.Shared.Dtos.AuthDtos.OAuthDtos;
using StockManagement.Shared.ViewModels.RoleVM;

namespace StockManagement.Business.Services.Abstract
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginDto login);
        Task<string?> VerifyLoginConfirmCodeAsync(LoginConfirmCodeDto model);
        Task<string?> LoginWithConfirmCodeAsync(LoginDto login);
        Task<string?> GoogleLoginAsync(GoogleLoginDto dto);
        Task<bool> RegisterAsync(RegisterDto model);
        Task<bool> ConfirmMailAsync(ConfirmCodeDto model);
        Task<bool> ChangePasswordAsync(ChangePasswordDto model, string userId);
        Task<bool> ResetPassword(ResetPasswordDto model, string code);
        Task<UpdateProfileDto> GetDataUpdateProfileAsync(string userId);
        Task<bool> UpdateProfileAsync(UpdateProfileDto model, string userId);
        Task<List<RoleAssignVM>> GetRoleAssignAsync(string id);
        Task<bool> RoleAssignAsync(List<RoleAssignVM> modelList, string id);
        Task<bool> LogoutAsync(string userId);
    }
}
