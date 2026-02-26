
namespace StockManagement.Shared.Dtos.AuthDtos
{
    public class LoginConfirmCodeDto
    {
        public string Email { get; set; }
        public int LoginConfirmCode { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
