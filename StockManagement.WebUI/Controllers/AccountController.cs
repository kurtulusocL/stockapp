using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Business.Constants.Utilities.Responses;
using StockManagement.Business.Filters;
using StockManagement.Shared.Dtos.AuthDtos;
using StockManagement.Shared.Dtos.AuthDtos.OAuthDtos;
using StockManagement.Shared.ViewModels.RoleVM;

namespace StockManagement.WebUI.Controllers
{
    [Authorize]
    [ExceptionHandler]
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient GetClient() => _httpClientFactory.CreateClient("StokApiClient");
        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginDto()
            {
                ReturnUrl = returnUrl
            });

        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto)
        {           
            var client = GetClient();            
            var response = await client.PostAsJsonAsync("api/accounts/login-with-confirm-code", dto);
            if (!response.IsSuccessStatusCode)
            {
                return View(dto);
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            if (result.ConfirmRequired)
            {
                TempData["Email"] = dto.Email;
                return RedirectToAction("VerifyCode");
            }

            Response.Cookies.Append("JWToken", result.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(result.Token);
            var claims = jwtToken.Claims.ToList();
            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth", principal, new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1),
                IsPersistent = true
            });
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet("google-login")]
        public IActionResult GoogleLogin(string? returnUrl = null)
        {
            var redirectUrl = Url.Action("GoogleCallback", "Account", new { returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleCallback(string? returnUrl = null)
        {           
            var authenticateResult = await HttpContext.AuthenticateAsync("CookieAuth");
            if (!authenticateResult.Succeeded)
                return RedirectToAction("Login", new { error = "Login with Google failed." });

            var claims = authenticateResult.Principal?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var nameSurname = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var providerKey = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(providerKey))
                return RedirectToAction("Login", new { error = "Information could not be retrieved from the Google account." });

            var dto = new GoogleLoginDto
            {
                Email = email,
                NameSurname = nameSurname ?? email,
                ProviderKey = providerKey
            };
           
            using var client = _httpClientFactory.CreateClient("StockManagementAPI");
            var response = await client.PostAsJsonAsync("api/accounts/google-login", dto);

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Login", new { error = "Login with Google failed." });

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            string token = result?.Token ?? string.Empty;

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", new { error = "Token could not be obtained." });

            Response.Cookies.Append("JWToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddHours(8)
            });

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult VerifyCode()
        {
            var email = TempData["Email"]?.ToString();
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");
            ViewBag.Email = email;
            TempData.Keep("Email");
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(LoginConfirmCodeDto dto)
        {
            var client = GetClient();
            var response = await client.PostAsJsonAsync("api/accounts/verify-login-confirm-code", dto);
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Email = dto.Email;
                return View(dto);
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Response.Cookies.Append("JWToken", result.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(result.Token);
            var claims = jwtToken.Claims.ToList();
            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("CookieAuth", principal, new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1),
                IsPersistent = true
            });

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var client = GetClient();
            var response = await client.PostAsJsonAsync("api/accounts/register", dto);
            if (!response.IsSuccessStatusCode)
            {
                return View(dto);
            }
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ConfirmMail()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmMail(ConfirmCodeDto dto)
        {
            var client = GetClient();
            var response = await client.PostAsJsonAsync("api/accounts/confirm-mail", dto);
            if (!response.IsSuccessStatusCode)
            {
                return View(dto);
            }
            return RedirectToAction("VerifyCode");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var client = GetClient();
            var response = await client.PostAsJsonAsync("api/accounts/change-password", dto);
            if (!response.IsSuccessStatusCode)
            {
                return View(dto);
            }
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string code)
        {
            ViewBag.Code = code;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto, string code)
        {
            var client = GetClient();
            var response = await client.PostAsJsonAsync($"api/accounts/reset-password?code={code}", dto);
            if (!response.IsSuccessStatusCode)
            {
                return View(dto);
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProfile()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/accounts/get-data-update-profile");
            if (!response.IsSuccessStatusCode)
                return View(new UpdateProfileDto());

            var dto = await response.Content.ReadFromJsonAsync<UpdateProfileDto>();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
        {
            var client = GetClient();
            var response = await client.PutAsJsonAsync("api/accounts/update-profile", dto);
            if (!response.IsSuccessStatusCode)
            {
                return View(dto);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> RoleAssign(string id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/accounts/get-role-assign/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "User");
            }
            var data = await response.Content.ReadFromJsonAsync<List<RoleAssignVM>>();
            ViewBag.UserId = id;
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoleAssign(List<RoleAssignVM> modelList, string id)
        {
            var client = GetClient();
            var response = await client.PostAsJsonAsync($"api/accounts/role-assign/{id}", modelList);
            if (!response.IsSuccessStatusCode)
            {
                return View(modelList);
            }
            return RedirectToAction("Index", "User");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var client = GetClient();
            await client.PostAsync("api/accounts/logout", null);
            await HttpContext.SignOutAsync("CookieAuth");
            Response.Cookies.Delete("JWToken");
            Response.Cookies.Delete(".AspNetCore.CookieAuth");
            return RedirectToAction("Login", "Account");
        }
    }
}
