
namespace StockManagement.Shared.Dtos.AuthDtos
{
    public class RegisterDto
    {
        public string NameSurname { get; set; }
        public string PhoneNumber { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
