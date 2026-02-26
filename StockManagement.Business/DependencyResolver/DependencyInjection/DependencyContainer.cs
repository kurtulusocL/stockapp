using System.Reflection;
using FluentValidation;
using Ganss.Xss;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockManagement.Business.Constants.Services;
using StockManagement.Business.Constants.Utilities.Caching;
using StockManagement.Business.Constants.Utilities.Jwt;
using StockManagement.Business.Constants.Utilities.Mail;
using StockManagement.Business.Constants.Workers.Azure;
using StockManagement.Business.Extensions;
using StockManagement.Business.Services.Abstract;
using StockManagement.Business.Services.Concrete;
using StockManagement.DataAccess.Abstract;
using StockManagement.DataAccess.Concrete.Context.Azure;
using StockManagement.DataAccess.Concrete.Context.EntityFramework.Mssql;
using StockManagement.DataAccess.Concrete.Repositories;
using StockManagement.DataAccess.Interceptors;
using StockManagement.Domain.Entities;
using StockManagement.Shared.Factory;

namespace StockManagement.Business.DependencyResolver.DependencyInjection
{
    public static class DependencyContainer
    {
        public static void DependencyService(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            string connectionString = env.IsDevelopment()
                ? configuration.GetConnectionString("DefaultConnection")
                : configuration.GetConnectionString("AzureConnection");

            services.AddCors(options =>
            {
                options.AddPolicy("StockCorsPolicy", builder =>
                {
                    builder.WithOrigins("https://localhost:7252").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                });
            });

            services.AddSingleton<OutboxEventInterceptor>();
            services.AddDbContext<ApplicationDbContext>((sp, options) => options.UseSqlServer(connectionString, sqlOptions => sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)).AddInterceptors(sp.GetRequiredService<OutboxEventInterceptor>()));

            if (env.IsDevelopment())
            {
                services.AddDbContext<AzureDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("AzureConnection")));
                services.AddHostedService<AzureSyncWorker>();
            }

            services.AddHostedService<AzureSyncWorker>();
            services.AddIdentity<AppUser, AppRole>(opt =>
            {
                opt.Password.RequireDigit = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequiredLength = 8;
                opt.Lockout.AllowedForNewUsers = true;
                opt.User.RequireUniqueEmail = true;
                opt.Lockout.MaxFailedAccessAttempts = 4;

            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            services.AddScoped<IAzureService, AzureManager>();
            services.AddScoped<ICacheService, MemoryCacheManager>();
            services.AddScoped<ITokenService, TokenManager>();
            services.AddScoped<IAuthService, AuthManager>();
            services.AddScoped<IMailService, MailManager>();
            services.AddScoped<EncryptionService>();

            services.AddScoped<IAuditRepository, AuditRepository>();
            services.AddScoped<IAuditService, AuditManager>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryManager>();

            services.AddScoped<IExceptionLoggerRepository, ExceptionLoggerRepository>();
            services.AddScoped<IExceptionLoggerService, ExceptionLoggerManager>();

            services.AddScoped<IOutboxEventRepository, OutboxEventRepository>();
            services.AddScoped<IOutboxEventService, OutboxEventManager>();

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductManager>();

            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleService, RoleManager>();

            services.AddScoped<IStockMovementRepository, StockMovementRepository>();
            services.AddScoped<IStockMovementService, StockMovementManager>();

            services.AddScoped<IUnitInStockRepository, UnitInStockRepository>();
            services.AddScoped<IUnitInStockService, UnitInstockManager>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserSevice, UserManager>();

            services.AddScoped<IUserSessionRepository, UserSessionRepository>();
            services.AddScoped<IUserSessionService, UserSessionManager>();

            services.AddScoped<IWarehouseRepository, WarehouseRepository>();
            services.AddScoped<IWarehouseService, WarehouseManager>();

            services.AddSingleton<IHtmlSanitizer>(_ => HtmlSanitizerFactory.Create());

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddFluentValidationServices();
        }
    }
}