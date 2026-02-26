using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Business.Filters;
using StockManagement.Shared.Dtos.MappingDtos.AppUserDtos;

namespace StockManagement.WebUI.Controllers
{
    [Authorize]
    [ExceptionHandler]
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient GetClient() => _httpClientFactory.CreateClient("StokApiClient");
        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/dashboards/me");
            if (!response.IsSuccessStatusCode)
                return View(new AppUserGetDto());

            var user = await response.Content.ReadFromJsonAsync<AppUserGetDto>();
            return View(user);
        }

        [Route("Home/NotFound")]
        public IActionResult NotFound([FromQuery] int code)
        {
            ViewBag.StatusCode = code;
            return View();
        }

        [Route("Home/BadRequest")]
        public IActionResult BadRequest([FromQuery] int code)
        {
            ViewBag.StatusCode = code;
            return View();
        }

        [Route("Home/ServerError")]
        public IActionResult ServerError([FromQuery] int code)
        {
            ViewBag.StatusCode = code;
            return View();
        }
    }
}
