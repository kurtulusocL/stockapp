using Microsoft.AspNetCore.Authentication;

namespace StockManagement.Business.Constants.Options.ApiKeyOptions
{
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "ApiKey";
        public const string HeaderName = "X-Api-Key";
    }
}
