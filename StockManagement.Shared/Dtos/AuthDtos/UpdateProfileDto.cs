
namespace StockManagement.Shared.Dtos.AuthDtos
{
    public class UpdateProfileDto
    {
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsLoginConfirmCodeActive { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime UpdatedDate { get; set; }
    }
}
