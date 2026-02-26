using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockManagement.Business.Constants.Options.RareLimitOptions;

namespace StockManagement.Business.Extensions
{
    public static class RateLimitExtensions
    {
        public static IServiceCollection AddCustomRateLimiter(this IServiceCollection services, IConfiguration configuration)
        {
            var rateLimitSettings = configuration.GetSection("RateLimit").Get<RateLimitOptions>();

            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("WebPolicy", opt =>
                {
                    opt.PermitLimit = rateLimitSettings.Web.PermitLimit;
                    opt.Window = TimeSpan.FromSeconds(rateLimitSettings.Web.WindowSeconds);
                    opt.QueueLimit = 0;
                });

                options.AddFixedWindowLimiter("SignalRPolicy", opt =>
                {
                    opt.PermitLimit = rateLimitSettings.SignalR.PermitLimit;
                    opt.Window = TimeSpan.FromSeconds(rateLimitSettings.SignalR.WindowSeconds);
                    opt.QueueLimit = 0;
                });

                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.HttpContext.Response.WriteAsync("Too many requests! Limit exceeded.", token);
                };
            });
            return services;
        }
    }
}
