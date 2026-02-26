using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Business.Filters;
using StockManagement.Shared.Dtos.MappingDtos.UnitInStockStos;
using StockManagement.Shared.Dtos.MappingDtos.WarehouseDtos;

namespace StockManagement.WebUI.Controllers
{
    [Authorize]
    [ExceptionHandler]
    public class UnitInStockController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient GetClient() => _httpClientFactory.CreateClient("StokApiClient");
        public UnitInStockController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/unitinstocks/get-all");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<UnitInStockGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<UnitInStockGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetTablePartial()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/unitinstocks/get-all");
            if (!response.IsSuccessStatusCode)
                return PartialView("_StockTablePartial", new List<UnitInStockGetDto>());
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<UnitInStockGetDto>>();
            return PartialView("_StockTablePartial", data);
        }

        [HttpGet]
        public async Task<IActionResult> GetWholeDataPartial()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/unitinstocks/get-whole-data");
            if (!response.IsSuccessStatusCode)
                return PartialView("_StockTablePartial", new List<UnitInStockGetDto>());
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<UnitInStockGetDto>>();
            return PartialView("_StockTablePartial", data);
        }

        [HttpGet]
        public async Task<IActionResult> WholeStocks()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/unitinstocks/get-whole-data");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<UnitInStockGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<UnitInStockGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> ByProduct(int? id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/unitinstocks/get-by-product/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<UnitInStockGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<UnitInStockGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> ByWarehouse(int id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/unitinstocks/get-by-warehouse/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<UnitInStockGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<UnitInStockGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> ByUser(string id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/unitinstocks/get-by-user/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<UnitInStockGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<UnitInStockGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/unitinstocks/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            var data = await response.Content.ReadFromJsonAsync<UnitInStockGetDto>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/unitinstocks/get-for-edit/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var data = await response.Content.ReadFromJsonAsync<UnitInStockUpdateDto>();

            var warehouseResponse = await client.GetAsync("api/warehouses/get-all");
            if (warehouseResponse.IsSuccessStatusCode)
                ViewBag.Warehouses = await warehouseResponse.Content.ReadFromJsonAsync<IEnumerable<WarehouseGetDto>>();

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UnitInStockUpdateDto dto)
        {
            dto.AppUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!ModelState.IsValid)
            {
                var client2 = GetClient();
                var warehouseResponse = await client2.GetAsync("api/warehouses/get-all");
                if (warehouseResponse.IsSuccessStatusCode)
                    ViewBag.Warehouses = await warehouseResponse.Content.ReadFromJsonAsync<IEnumerable<WarehouseGetDto>>();
                return View(dto);
            }

            var client = GetClient();
            var response = await client.PutAsJsonAsync("api/unitinstocks", dto);
            if (!response.IsSuccessStatusCode)
            {
                var warehouseResponse = await client.GetAsync("api/warehouses/get-all");
                if (warehouseResponse.IsSuccessStatusCode)
                    ViewBag.Warehouses = await warehouseResponse.Content.ReadFromJsonAsync<IEnumerable<WarehouseGetDto>>();
                return View(dto);
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = GetClient();
            var response = await client.DeleteAsync($"api/unitinstocks/hard-delete/{id}");
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple(List<int> ids)
        {
            var client = GetClient();
            var response = await client.PostAsJsonAsync("api/unitinstocks/delete-multiple", ids);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetActive(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/unitinstocks/set-active/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetInActive(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/unitinstocks/set-inactive/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDeleted(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/unitinstocks/soft-delete/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetNotDeleted(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/unitinstocks/restore/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
