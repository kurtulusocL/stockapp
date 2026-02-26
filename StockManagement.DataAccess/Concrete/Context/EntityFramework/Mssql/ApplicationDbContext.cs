using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StockManagement.DataAccess.Configurations;
using StockManagement.Domain.Entities;

namespace StockManagement.DataAccess.Concrete.Context.EntityFramework.Mssql
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        public DbSet<Audit> Audits { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ExceptionLogger> ExceptionLoggers { get; set; }
        public DbSet<OutboxEvent> OutboxEvents { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<UnitInStock> UnitInStocks { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ConfigureDbContexts(includeLocalEntities: true);
        }
    }
}