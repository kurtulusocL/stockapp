using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Business.Filters;
using StockManagement.Shared.Dtos.MappingDtos.AppRoleDtos;

namespace StockManagement.WebUI.Controllers
{
    [Authorize]
    [ExceptionHandler]
    public class RoleController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient GetClient() => _httpClientFactory.CreateClient("StokApiClient");
        public RoleController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/roles/get-all");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<AppRoleGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<AppRoleGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> WholeRoles()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/roles/get-whole-data");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<AppRoleGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<AppRoleGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/roles/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            var data = await response.Content.ReadFromJsonAsync<AppRoleGetDto>();
            return View(data);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppRoleCreateDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var client = GetClient();
            var response = await client.PostAsJsonAsync("api/roles", dto);
            if (!response.IsSuccessStatusCode)
            {
                return View(dto);
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/roles/get-for-edit/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var data = await response.Content.ReadFromJsonAsync<AppRoleUpdateDto>();
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AppRoleUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var client = GetClient();
            var response = await client.PutAsJsonAsync("api/roles", dto);
            if (!response.IsSuccessStatusCode)
            {
                return View(dto);
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var client = GetClient();
            var response = await client.DeleteAsync($"api/roles/hard-delete/{id}");
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple(List<string> ids)
        {
            var client = GetClient();
            var response = await client.PostAsJsonAsync("api/roles/delete-multiple", ids);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetActive(string id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/roles/set-active/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetInActive(string id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/roles/set-inactive/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDeleted(string id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/roles/soft-delete/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetNotDeleted(string id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/roles/restore/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
