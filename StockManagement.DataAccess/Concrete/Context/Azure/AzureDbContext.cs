using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StockManagement.DataAccess.Configurations;
using StockManagement.Domain.Entities;

namespace StockManagement.DataAccess.Concrete.Context.Azure
{
    public class AzureDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public AzureDbContext(DbContextOptions<AzureDbContext> options) : base(options)
        {
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        public DbSet<Audit> Audits { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ExceptionLogger> ExceptionLoggers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<UnitInStock> UnitInStocks { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var navigation in entityType.GetNavigations())
                {
                    navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
                }

                var idProperty = entityType.FindProperty("Id");
                if (idProperty != null && idProperty.ClrType == typeof(int))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property("Id").ValueGeneratedNever();
                }
            }
            modelBuilder.ConfigureDbContexts();
        }
    }
}
