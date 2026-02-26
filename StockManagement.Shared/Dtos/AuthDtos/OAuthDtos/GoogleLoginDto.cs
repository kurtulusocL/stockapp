
namespace StockManagement.Shared.Dtos.AuthDtos.OAuthDtos
{
    public class GoogleLoginDto
    {
        public string Email { get; set; }
        public string? NameSurname { get; set; }
        public string? Title{ get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProviderKey { get; set; }
    }
}
