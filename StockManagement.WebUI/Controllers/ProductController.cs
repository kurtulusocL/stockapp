using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Business.Filters;
using StockManagement.Shared.Dtos.MappingDtos.CategoryDtos;
using StockManagement.Shared.Dtos.MappingDtos.ProductDtos;
using StockManagement.Shared.Dtos.MappingDtos.UnitInStockStos;
using StockManagement.Shared.Dtos.MappingDtos.WarehouseDtos;

namespace StockManagement.WebUI.Controllers
{
    [Authorize]
    [ExceptionHandler]
    public class ProductController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient GetClient() => _httpClientFactory.CreateClient("StokApiClient");
        public ProductController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/products/get-all");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<ProductGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<ProductGetDto>>();
            return View(data);
        }

        public async Task<IActionResult> WholeProducts()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/products/get-whole-data");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<ProductGetDto>());
            }
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<ProductGetDto>>();
            return View(data);
        }

        public async Task<IActionResult> ByCategory(int id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/products/get-by-category/{id}");
            if (!response.IsSuccessStatusCode)
                return Redirect(Request.Headers["Referer"].ToString());

            var data = await response.Content.ReadFromJsonAsync<IEnumerable<ProductGetDto>>();
            return View(data);
        }

        public async Task<IActionResult> ByWarehouse(int id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/products/get-by-warehouse/{id}");
            if (!response.IsSuccessStatusCode)
                return Redirect(Request.Headers["Referer"].ToString());

            var data = await response.Content.ReadFromJsonAsync<IEnumerable<ProductGetDto>>();
            return View(data);
        }

        public async Task<IActionResult> ByUser(string id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/products/get-by-user/{id}");
            if (!response.IsSuccessStatusCode)
                return Redirect(Request.Headers["Referer"].ToString());

            var data = await response.Content.ReadFromJsonAsync<IEnumerable<ProductGetDto>>();
            return View(data);
        }

        public async Task<IActionResult> WarningStock()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/products/get-warning-stock");
            if (!response.IsSuccessStatusCode)
                return Redirect(Request.Headers["Referer"].ToString());

            var data = await response.Content.ReadFromJsonAsync<IEnumerable<ProductGetDto>>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/products/{id}");
            if (!response.IsSuccessStatusCode)
                return Redirect(Request.Headers["Referer"].ToString());

            var data = await response.Content.ReadFromJsonAsync<ProductGetDto>();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name, string code, string description, decimal price, int categoryId, int warehouseId, string userId, IFormFile? image)
        {
            userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var client = GetClient();

            var formContent = new MultipartFormDataContent();
            formContent.Add(new StringContent(name ?? ""), "name");
            formContent.Add(new StringContent(code ?? ""), "code");
            formContent.Add(new StringContent(description ?? ""), "description");
            formContent.Add(new StringContent(price.ToString()), "price");
            formContent.Add(new StringContent(categoryId.ToString()), "categoryId");
            formContent.Add(new StringContent(warehouseId.ToString()), "warehouseId");
            formContent.Add(new StringContent(userId ?? ""), "userId");

            if (image != null)
            {
                var streamContent = new StreamContent(image.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);
                formContent.Add(streamContent, "image", image.FileName);
            }

            var response = await client.PostAsync("api/products", formContent);
            if (!response.IsSuccessStatusCode)
            {
                await PopulateDropdowns();
                return View();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/products/get-for-edit/{id}");
            if (!response.IsSuccessStatusCode)
                return Redirect(Request.Headers["Referer"].ToString());

            var data = await response.Content.ReadFromJsonAsync<ProductUpdateDto>();
            await PopulateDropdowns();
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductUpdateDto dto, IFormFile? image)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var client = GetClient();

            var formContent = new MultipartFormDataContent();
            formContent.Add(new StringContent(dto.Name ?? ""), "name");
            formContent.Add(new StringContent(dto.Code ?? ""), "code");
            formContent.Add(new StringContent(dto.Description ?? ""), "description");
            formContent.Add(new StringContent(dto.Price.ToString()), "price");
            formContent.Add(new StringContent(dto.CategoryId.ToString()), "categoryId");
            formContent.Add(new StringContent(dto.WarehouseId.ToString()), "warehouseId");
            formContent.Add(new StringContent(userId ?? ""), "userId");
            formContent.Add(new StringContent(dto.Id.ToString()), "id");

            if (image != null)
            {
                var streamContent = new StreamContent(image.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);
                formContent.Add(streamContent, "image", image.FileName);
            }
            var response = await client.PutAsync("api/products", formContent);
            if (!response.IsSuccessStatusCode)
            {
                await PopulateDropdowns();
                return View(dto);
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
        public async Task<IActionResult> AddStock(int id)
        {
            var client = GetClient();

            var productResponse = await client.GetAsync($"api/products/{id}");
            if (!productResponse.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            var product = await productResponse.Content.ReadFromJsonAsync<ProductGetDto>();
            ViewBag.Product = product;

            var warehouseResponse = await client.GetAsync("api/warehouses/get-all");
            if (warehouseResponse.IsSuccessStatusCode)
            {
                var warehouses = await warehouseResponse.Content.ReadFromJsonAsync<IEnumerable<WarehouseGetDto>>();
                ViewBag.Warehouses = warehouses?.ToList() ?? new List<WarehouseGetDto>();
            }

            var model = new UnitInStockCreateDto { ProductId = id };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStock(int quantity, string code, int? productId, int warehouseId, string appUserId)
        {
            appUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var client = GetClient();

            var dto = new { quantity, code, productId, warehouseId, appUserId };
            var response = await client.PostAsJsonAsync("api/products/add-stock", dto);

            if (!response.IsSuccessStatusCode)
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = GetClient();
            await client.DeleteAsync($"api/products/hard-delete/{id}");
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int> ids)
        {
            var client = GetClient();
            await client.PostAsJsonAsync("api/products/delete-multiple", ids);           
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetActive(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/products/set-active/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetInActive(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/products/set-inactive/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDeleted(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/products/soft-delete/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> SetNotDeleted(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/products/restore/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        } 
       
        private async Task PopulateDropdowns()
        {
            var client = GetClient();

            var categoryResponse = await client.GetAsync("api/categories/get-all");
            if (categoryResponse.IsSuccessStatusCode)
            {
                var categories = await categoryResponse.Content.ReadFromJsonAsync<IEnumerable<CategoryGetDto>>();
                ViewBag.Categories = categories?.ToList() ?? new List<CategoryGetDto>();
            }
            else
            {
                ViewBag.Categories = new List<CategoryGetDto>();
            }

            var warehouseResponse = await client.GetAsync("api/warehouses/get-all");
            if (warehouseResponse.IsSuccessStatusCode)
            {
                var warehouses = await warehouseResponse.Content.ReadFromJsonAsync<IEnumerable<WarehouseGetDto>>();
                ViewBag.Warehouses = warehouses?.ToList() ?? new List<WarehouseGetDto>();
            }
            else
            {
                ViewBag.Warehouses = new List<WarehouseGetDto>();
            }
        }

        public async Task<IActionResult> GetTablePartial()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/products/get-all");
            if (!response.IsSuccessStatusCode)
                return PartialView("_ProductTablePartial", new List<ProductGetDto>());

            var data = await response.Content.ReadFromJsonAsync<IEnumerable<ProductGetDto>>();
            return PartialView("_ProductTablePartial", data);
        }

        public async Task<IActionResult> GetWholeDataPartial()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/products/get-whole-data");
            if (!response.IsSuccessStatusCode)
                return PartialView("_ProductTablePartial", new List<ProductGetDto>());

            var data = await response.Content.ReadFromJsonAsync<IEnumerable<ProductGetDto>>();
            return PartialView("_ProductTablePartial", data);
        }

        public async Task<IActionResult> GetWarningStockPartial()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/products/get-warning-stock");
            if (!response.IsSuccessStatusCode)
                return PartialView("_ProductTablePartial", new List<ProductGetDto>());

            var data = await response.Content.ReadFromJsonAsync<IEnumerable<ProductGetDto>>();
            return PartialView("_ProductTablePartial", data);
        }
    }
}
