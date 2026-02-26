
namespace StockManagement.Shared.Dtos.AuthDtos
{
    public class LoginResponseDto
    {
        public string? Token { get; set; }
        public bool ConfirmRequired { get; set; }
    }
}
