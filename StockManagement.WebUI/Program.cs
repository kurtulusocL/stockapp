using Microsoft.AspNetCore.Authentication.Google;
using StockManagement.Business.Constants.Handlers;
using StockManagement.Business.Constants.Middlewares;
using StockManagement.Business.Filters;
using StockManagement.Shared.Helpers;

var builder = WebApplication.CreateBuilder(args);

var apiBaseUrl = builder.Configuration.GetValue<string>("ApiSettings:BaseUrl");

builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ValidatorActionFilter>();
}).AddRazorRuntimeCompilation().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddHttpClient("StockManagementAPI", client =>
{
    var apiBaseUrl = builder.Configuration["GoogleOAuthSettings:BaseUrl"];
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromHours(4);
        options.Cookie.Name = "StokApp.Auth";
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.IsEssential = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;

    })
     .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
     {
         options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
         options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
         options.CallbackPath = "/signin-google";
         options.SignInScheme = "CookieAuth";
         options.Scope.Add("email");
         options.Scope.Add("profile");
         options.SaveTokens = true;
     });

builder.Services.AddTransient<JwtAuthorizationHandler>();

builder.Services.AddHttpClient("StokApiClient", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<JwtAuthorizationHandler>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddProgressiveWebApp();
builder.Services.AddMvc();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(4);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

var app = builder.Build();

ServiceProviderHelper.ServiceProvider = app.Services;
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapStaticAssets();
app.UseMiddleware<StatusCodeHandlerMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();
