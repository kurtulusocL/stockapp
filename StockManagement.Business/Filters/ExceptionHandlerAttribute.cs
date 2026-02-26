using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using StockManagement.DataAccess.Concrete.Context.EntityFramework.Mssql;
using StockManagement.Domain.Entities;
using StockManagement.Shared.Helpers;

namespace StockManagement.Business.Filters
{
    public class ExceptionHandlerAttribute : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            using (var scope = ServiceProviderHelper.ServiceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                if (!filterContext.ExceptionHandled)
                {
                    ExceptionLogger logger = new ExceptionLogger()
                    {
                        ExceptionMessage = filterContext.Exception.Message,
                        ExceptionStackTrace = filterContext.Exception.StackTrace,
                        ControllerName = filterContext.RouteData.Values["controller"].ToString(),
                        CreatedDate = DateTime.Now.ToLocalTime(),
                        IsActive = true,
                        IsDeleted = false
                    };
                    dbContext.ExceptionLoggers.Add(logger);
                    dbContext.SaveChanges();

                    dbContext.OutboxEvents.Add(new OutboxEvent
                    {
                        EntityType = "ExceptionLogger",
                        EventType = "Added",
                        Payload = JsonSerializer.Serialize(logger),
                        CreatedDate = DateTime.Now,
                        IsProcessed = false,
                        IsActive = true,
                        IsDeleted = false
                    });
                    dbContext.SaveChanges();
                    filterContext.ExceptionHandled = true;
                }
            }
        }
    }
}
