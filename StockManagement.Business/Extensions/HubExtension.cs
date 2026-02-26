using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using StockManagement.Business.Hubs;

namespace StockManagement.Business.Extensions
{
    public static class HubExtension
    {
        public static void MapAllHubs(this IEndpointRouteBuilder app)
        {
            app.MapHub<StockHub>("/stockHub");
            app.MapHub<StockHub>("/productHub");
        }
    }
}
