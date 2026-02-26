using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Business.Filters;
using StockManagement.Shared.Dtos.MappingDtos.StockMovementDtos;

namespace StockManagement.WebUI.Controllers
{
    [Authorize]
    [ExceptionHandler]
    public class StockMovementController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient GetClient() => _httpClientFactory.CreateClient("StokApiClient");
        public StockMovementController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/stockmovements/get-all");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<StockMovementGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<StockMovementGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> WholeStockMovements()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/stockmovements/get-whole-data");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<StockMovementGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<StockMovementGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> ByUser(string userId)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/stockmovements/get-by-user/{userId}");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<StockMovementGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<StockMovementGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> ByProduct(int? productId)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/stockmovements/get-by-product/{productId}");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<StockMovementGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<StockMovementGetDto>>();
            return View(data);
        }

        [HttpGet]
        public IActionResult ByRange()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ByRange(DateTime start, DateTime end)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/stockmovements/get-by-range?start={start:yyyy-MM-ddTHH:mm:ss}&end={end:yyyy-MM-ddTHH:mm:ss}");
            if (!response.IsSuccessStatusCode)
            {
                return View();
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<StockMovementGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/stockmovements/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            var data = await response.Content.ReadFromJsonAsync<StockMovementGetDto>();
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = GetClient();
            var response = await client.DeleteAsync($"api/stockmovements/hard-delete/{id}");
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple(List<int> ids)
        {
            var client = GetClient();
            var response = await client.PostAsJsonAsync("api/stockmovements/delete-multiple", ids);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetActive(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/stockmovements/set-active/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetInActive(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/stockmovements/set-inactive/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDeleted(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/stockmovements/soft-delete/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetNotDeleted(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/stockmovements/restore/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
