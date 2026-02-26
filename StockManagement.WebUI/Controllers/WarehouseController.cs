using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Business.Filters;
using StockManagement.Shared.Dtos.MappingDtos.WarehouseDtos;

namespace StockManagement.WebUI.Controllers
{
    [Authorize]
    [ExceptionHandler]
    public class WarehouseController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient GetClient() => _httpClientFactory.CreateClient("StokApiClient");
        public WarehouseController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = GetClient();
            var response = await client.GetFromJsonAsync<List<WarehouseGetDto>>("api/warehouses/get-all");
            return View(response);
        }

        [HttpGet]
        public async Task<IActionResult> WholeWarehouse()
        {
            var client = GetClient();
            var response = await client.GetFromJsonAsync<List<WarehouseGetDto>>("api/warehouses/get-whole-data");
            return View(response);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/warehouses/{id}");
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index");
            var category = await response.Content.ReadFromJsonAsync<WarehouseGetDto>();
            return View(category);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WarehouseCreateDto warehouse)
        {
            var client = GetClient();
            var response = await client.PostAsJsonAsync("api/warehouses", warehouse);
            if (response.IsSuccessStatusCode)
                return Redirect(Request.Headers["Referer"].ToString());
            return View(warehouse);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = GetClient();
            var dto = await client.GetFromJsonAsync<WarehouseUpdateDto>($"api/warehouses/get-for-edit/{id}");
            if (dto == null) return NotFound();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(WarehouseUpdateDto warehouse)
        {
            var client = GetClient();
            var response = await client.PutAsJsonAsync("api/warehouses", warehouse);
            if (response.IsSuccessStatusCode)
                return Redirect(Request.Headers["Referer"].ToString());          
            return View(warehouse);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = GetClient();
            await client.DeleteAsync($"api/warehouses/hard-delete/{id}");
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple(List<int> ids)
        {
            var client = GetClient();
            await client.PostAsJsonAsync("api/warehouses/delete-multiple", ids);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetActive(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/warehouses/set-active/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetInactive(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/warehouses/set-inactive/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDeleted(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/warehouses/soft-delete/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetNotDeleted(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/warehouses/restore/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
