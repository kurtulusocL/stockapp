using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Business.Filters;
using StockManagement.Shared.Dtos.MappingDtos.CategoryDtos;

namespace StockManagement.WebUI.Controllers
{
    [Authorize]
    [ExceptionHandler]
    public class CategoryController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient GetClient() => _httpClientFactory.CreateClient("StokApiClient");
        public CategoryController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = GetClient();
            var response = await client.GetFromJsonAsync<List<CategoryGetDto>>("api/categories/get-all");
            return View(response);
        }

        [HttpGet]
        public async Task<IActionResult> WholeCategories()
        {
            var client = GetClient();
            var response = await client.GetFromJsonAsync<List<CategoryGetDto>>("api/categories/get-whole-data");
            return View(response);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/categories/{id}");
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index");
            var category = await response.Content.ReadFromJsonAsync<CategoryGetDto>();
            return View(category);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateDto category)
        {
            var client = GetClient();
            var response = await client.PostAsJsonAsync("api/categories", category);
            if (response.IsSuccessStatusCode) 
                return Redirect(Request.Headers["Referer"].ToString());
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = GetClient();
            var dto = await client.GetFromJsonAsync<CategoryUpdateDto>($"api/categories/get-for-edit/{id}");
            if (dto == null) return NotFound();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryUpdateDto category)
        {
            var client = GetClient();
            var response = await client.PutAsJsonAsync("api/categories", category);
            if (response.IsSuccessStatusCode)
                return Redirect(Request.Headers["Referer"].ToString());
           
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = GetClient();
            await client.DeleteAsync($"api/categories/hard-delete/{id}");
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple(List<int> ids)
        {
            var client = GetClient();
            await client.PostAsJsonAsync("api/categories/delete-multiple", ids);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetActive(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/categories/set-active/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetInactive(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/categories/set-inactive/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDeleted(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/categories/soft-delete/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetNotDeleted(int id)
        {
            var client = GetClient();
            await client.PatchAsync($"api/categories/restore/{id}", null);
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
