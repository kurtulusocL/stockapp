using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Business.Filters;
using StockManagement.Shared.Dtos.MappingDtos.AuditDtos;

namespace StockManagement.WebUI.Controllers
{
    [Authorize]
    [ExceptionHandler]
    public class AuditController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient GetClient() => _httpClientFactory.CreateClient("StokApiClient");
        public AuditController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/audits/get-all");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<AuditGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<AuditGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> WholeAudits()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/audits/get-whole-data");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<AuditGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<AuditGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> ByUser(string? id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/audits/get-by-user/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<AuditGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<AuditGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/audits/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            var data = await response.Content.ReadFromJsonAsync<AuditGetDto>();
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = GetClient();
            var response = await client.DeleteAsync($"api/audits/hard-delete/{id}");
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple(List<int> ids)
        {
            var client = GetClient();
            var response = await client.PostAsJsonAsync("api/audits/delete-multiple", ids);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetActive(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/audits/set-active/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetInActive(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/audits/set-inactive/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDeleted(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/audits/soft-delete/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetNotDeleted(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/audits/restore/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
