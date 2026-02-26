
namespace StockManagement.Shared.Dtos.AuthDtos
{
    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
        public string? StatusMessage { get; set; }
    }
}
