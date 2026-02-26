
namespace StockManagement.Business.Constants.Helpers
{
    public static class PhoneNumberValidationHelpers
    {
        public static bool IsNumeric(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            return value.All(char.IsDigit);
        }
        public static bool IsValidPhone(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            return value.Length >= 10 && value.Length <= 15 && value.All(char.IsDigit);
        }
    }
}
