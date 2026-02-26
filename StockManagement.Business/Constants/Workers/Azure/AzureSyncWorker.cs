using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StockManagement.DataAccess.Concrete.Context.Azure;
using StockManagement.DataAccess.Concrete.Context.EntityFramework.Mssql;

namespace StockManagement.Business.Constants.Workers.Azure
{
    public class AzureSyncWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AzureSyncWorker> _logger;

        public AzureSyncWorker(IServiceProvider serviceProvider, ILogger<AzureSyncWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var localContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var azureContext = scope.ServiceProvider.GetRequiredService<AzureDbContext>();

                    var pendingEvents = localContext.OutboxEvents.Where(e => !e.IsProcessed).OrderBy(e => e.CreatedDate).Take(50).ToList();
                    if (pendingEvents.Any())
                    {
                        _logger.LogInformation("{Count} unprocessed Outbox records were found. Synchronization is starting...", pendingEvents.Count);

                        foreach (var ev in pendingEvents)
                        {
                            try
                            {
                                if (ev.EntityType == "AspNetUserLogins")
                                {
                                    var loginData = JsonSerializer.Deserialize<Dictionary<string, string>>(ev.Payload);

                                    var userLogin = new Microsoft.AspNetCore.Identity.IdentityUserLogin<string>
                                    {
                                        LoginProvider = loginData["LoginProvider"],
                                        ProviderKey = loginData["ProviderKey"],
                                        ProviderDisplayName = loginData["ProviderDisplayName"],
                                        UserId = loginData["UserId"]
                                    };

                                    var exists = await azureContext.Set<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>()
                                        .AnyAsync(l => l.LoginProvider == userLogin.LoginProvider && l.ProviderKey == userLogin.ProviderKey, stoppingToken);

                                    if (!exists)
                                    {
                                        await azureContext.Set<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>().AddAsync(userLogin, stoppingToken);
                                        await azureContext.SaveChangesAsync(stoppingToken);
                                    }

                                    ev.IsProcessed = true;
                                    _logger.LogInformation("The OAuth login record has been successfully transferred to Azure..");
                                    continue;
                                }
                                else if (ev.EntityType == "AspNetUserRoles")
                                {
                                    var roleData = JsonSerializer.Deserialize<Dictionary<string, string>>(ev.Payload);
                                    var userRole = new Microsoft.AspNetCore.Identity.IdentityUserRole<string>
                                    {
                                        UserId = roleData["UserId"],
                                        RoleId = roleData["RoleId"]
                                    };

                                    var rExists = await azureContext.Set<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>()
                                        .AnyAsync(r => r.UserId == userRole.UserId && r.RoleId == userRole.RoleId, stoppingToken);

                                    if (!rExists)
                                    {
                                        await azureContext.Set<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>().AddAsync(userRole, stoppingToken);
                                        await azureContext.SaveChangesAsync(stoppingToken);
                                    }
                                    ev.IsProcessed = true;
                                    continue;
                                }

                                var entityType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).FirstOrDefault(t => t.Name == ev.EntityType && t.Namespace == "StockManagement.Domain.Entities");

                                if (entityType == null)
                                {
                                    _logger.LogWarning("Type not found.: {EntityType}", ev.EntityType);
                                    ev.IsProcessed = true;
                                    continue;
                                }

                                var entity = JsonSerializer.Deserialize(ev.Payload, entityType, new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                });
                                if (entity != null)
                                {
                                    if (ev.EventType == "Deleted")
                                    {
                                        var existing = await azureContext.FindAsync(entityType, GetEntityId(entity));
                                        if (existing != null)
                                        {
                                            azureContext.Remove(existing);
                                            await azureContext.SaveChangesAsync(stoppingToken);
                                        }
                                    }
                                    else
                                    {
                                        var existing = await azureContext.FindAsync(entityType, GetEntityId(entity));
                                        if (existing != null)
                                            azureContext.Entry(existing).CurrentValues.SetValues(entity);
                                        else
                                            await azureContext.AddAsync(entity, stoppingToken);

                                        await azureContext.SaveChangesAsync(stoppingToken);
                                    }
                                    ev.IsProcessed = true;
                                    _logger.LogInformation("Success: {EntityType} transferred to Azure. ID: {EventId}", ev.EntityType, ev.Id);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error: {EntityType}. Message: {Message}", ev.EntityType, ex.Message);
                                azureContext.ChangeTracker.Clear();
                            }
                        }
                        await localContext.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation("The Outbox table has been updated.");
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
            }
        }

        private object GetEntityId(object entity)
        {
            return entity.GetType().GetProperty("Id")?.GetValue(entity);
        }
    }
}