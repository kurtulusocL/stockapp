using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Business.Filters;
using StockManagement.Shared.Dtos.MappingDtos.AppUserDtos;

namespace StockManagement.WebUI.Controllers
{
    [Authorize]
    [ExceptionHandler]
    public class UserController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient GetClient() => _httpClientFactory.CreateClient("StokApiClient");
        public UserController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/users/get-all");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<AppUserGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<AppUserGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> WholeUsers()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/users/get-whole-data");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<AppUserGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<AppUserGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> ByActiveLoginCode()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/users/get-by-active-login-code");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<AppUserGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<AppUserGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> ByInActiveLoginCode()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/users/get-by-inactive-login-code");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<AppUserGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<AppUserGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/users/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            var data = await response.Content.ReadFromJsonAsync<AppUserGetDto>();
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var client = GetClient();
            var response = await client.DeleteAsync($"api/users/hard-delete/{id}");
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple(List<string> ids)
        {
            var client = GetClient();
            var response = await client.PostAsJsonAsync("api/users/delete-multiple", ids);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetLoginCodeActive()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var client = GetClient();
            await client.PatchAsync($"api/users/set-active-login-code/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetLoginCodeInActive()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var client = GetClient();
            await client.PatchAsync($"api/users/set-inactive-login-code/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetActive(string id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/users/set-active/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetInActive(string id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/users/set-inactive/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDeleted(string id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/users/soft-delete/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetNotDeleted(string id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/users/restore/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
