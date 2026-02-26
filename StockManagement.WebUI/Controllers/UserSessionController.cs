using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Business.Filters;
using StockManagement.Shared.Dtos.MappingDtos.UserSessionDtos;

namespace StockManagement.WebUI.Controllers
{
    [Authorize]
    [ExceptionHandler]
    public class UserSessionController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient GetClient() => _httpClientFactory.CreateClient("StokApiClient");
        public UserSessionController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/usersessions/get-all");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<UserSessionGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<UserSessionGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> WholeUserSessions()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/usersessions/get-whole-data");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<UserSessionGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<UserSessionGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> ByUser(string id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/usersessions/get-by-user/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<UserSessionGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<UserSessionGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> ByLoginDate()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/usersessions/get-by-login-date");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<UserSessionGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<UserSessionGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Online()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/usersessions/get-online");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<UserSessionGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<UserSessionGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Offline()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/usersessions/get-offline");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<UserSessionGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<UserSessionGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/usersessions/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            var data = await response.Content.ReadFromJsonAsync<UserSessionGetDto>();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var client = GetClient();
            var response = await client.DeleteAsync($"api/usersessions/hard-delete/{id}");            
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteMultiple(List<int> ids)
        {
            var client = GetClient();
            var response = await client.PostAsJsonAsync("api/usersessions/delete-multiple", ids);            
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetActive(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/usersessions/set-active/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetInActive(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/usersessions/set-inactive/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDeleted(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/usersessions/soft-delete/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetNotDeleted(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/usersessions/restore/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
