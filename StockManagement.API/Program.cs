using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using StockManagement.Business.Constants.Handlers;
using StockManagement.Business.Constants.Options.ApiKeyOptions;
using StockManagement.Business.DependencyResolver.DependencyInjection;
using StockManagement.Business.Extensions;
using StockManagement.Business.Filters;
using StockManagement.Business.Mapping;
using StockManagement.DataAccess.Concrete.Context.EntityFramework.Mssql;
using StockManagement.Shared.Helpers;
using StockManagement.Shared.Helpers.Configurations;

var builder = WebApplication.CreateBuilder(args);

var logPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Logs", "SerilogLogs.txt");
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information().WriteTo.Console().WriteTo
    .File(logPath, rollingInterval: RollingInterval.Infinite, rollOnFileSizeLimit: true, fileSizeLimitBytes: 10 * 1024 * 1024,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}").CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddMappingProfiles();
builder.Services.DependencyService(builder.Configuration, builder.Environment);
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidatorActionFilter>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddCustomRateLimiter(builder.Configuration);
builder.Services.AddCustomJwtAuthentication(builder.Configuration);
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My Api",
        Version = "v1"
    });
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(4);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});
builder.Services.AddAuthentication("ApiKey").AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>("ApiKey", null);
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddDataProtection();
builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.CookieManager = null;
});
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("SignalRCors", policy => policy
        .WithOrigins("https://localhost:7252").AllowAnyMethod().AllowAnyHeader().AllowCredentials());
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDesktopApp", policy =>
    {
        policy.SetIsOriginAllowed(origin => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    });
});

var app = builder.Build();

ServiceProviderHelper.ServiceProvider = app.Services;
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRateLimiter();
app.UseRouting();
app.UseCors("AllowDesktopApp");
app.UseCors("SignalRCors");
app.UseCors("StockCorsPolicy");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapAllHubs();
app.MapControllers();

app.Run();
Log.CloseAndFlush();