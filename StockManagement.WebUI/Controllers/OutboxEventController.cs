using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Business.Filters;
using StockManagement.Shared.Dtos.MappingDtos.OutboxEventDtos;

namespace StockManagement.WebUI.Controllers
{
    [Authorize]
    [ExceptionHandler]
    public class OutboxEventController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient GetClient() => _httpClientFactory.CreateClient("StokApiClient");
        public OutboxEventController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/outboxevents/get-all");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<OutboxEventGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<OutboxEventGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> WholeOutboxEvents()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/outboxevents/get-whole-data");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<OutboxEventGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<OutboxEventGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> SuccessfulEvents()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/outboxevents/get-successful");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<OutboxEventGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<OutboxEventGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> ErrorEvents()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/outboxevents/get-error");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<OutboxEventGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<OutboxEventGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/outboxevents/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            var data = await response.Content.ReadFromJsonAsync<OutboxEventGetDto>();
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = GetClient();
            var response = await client.DeleteAsync($"api/outboxevents/hard-delete/{id}");
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple(List<int> ids)
        {
            var client = GetClient();
            var response = await client.PostAsJsonAsync("api/outboxevents/delete-multiple", ids);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetActive(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/outboxevents/set-active/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetInActive(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/outboxevents/set-inactive/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/outboxevents/soft-delete/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/outboxevents/restore/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
