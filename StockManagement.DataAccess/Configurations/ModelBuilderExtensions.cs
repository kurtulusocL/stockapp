using Microsoft.EntityFrameworkCore;
using StockManagement.Domain.Entities;

namespace StockManagement.DataAccess.Configurations
{
    public static class ModelBuilderExtensions
    {
        public static void ConfigureDbContexts(this ModelBuilder modelBuilder, bool includeLocalEntities = false)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var navigation in entityType.GetNavigations())
                {
                    navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
                }
            }

            modelBuilder.Entity<Audit>().HasOne(us => us.AppUser).WithMany(u => u.Audits).HasForeignKey(us => us.AppUserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<StockMovement>().HasOne(us => us.AppUser).WithMany(u => u.StockMovements).HasForeignKey(us => us.AppUserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Product>().HasOne(us => us.AppUser).WithMany(u => u.Products).HasForeignKey(us => us.AppUserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserSession>().HasOne(us => us.AppUser).WithMany(u => u.UserSessions).HasForeignKey(us => us.AppUserId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Audit>(entity =>
            {
                entity.HasIndex(e => e.Id).HasDatabaseName("IX_Audit_Id").IsUnique();
                entity.HasIndex(e => e.AppUserId).HasDatabaseName("IX_Audit_AppUserId");
                entity.HasIndex(e => new { e.IsActive, e.IsDeleted }).HasDatabaseName("IX_Audit_IsActive_IsDeleted");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => e.Id).HasDatabaseName("IX_Category_Id").IsUnique();
                entity.Property(e => e.Code).IsRequired().HasMaxLength(3).IsFixedLength();
                entity.HasIndex(e => new { e.IsActive, e.IsDeleted }).HasDatabaseName("IX_Category_IsActive_IsDeleted");
            });

            modelBuilder.Entity<ExceptionLogger>(entity =>
            {
                entity.HasIndex(e => e.Id).HasDatabaseName("IX_ExceptionLogger_Id").IsUnique();
                entity.HasIndex(e => new { e.IsActive, e.IsDeleted }).HasDatabaseName("IX_ExceptionLogger_IsActive_IsDeleted");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.Id).HasDatabaseName("IX_Product_Id").IsUnique();
                entity.Property(e => e.Code).IsRequired().HasMaxLength(6).IsFixedLength();
                entity.HasIndex(e => e.Price).HasDatabaseName("IX_Price");
                entity.HasIndex(e => e.CategoryId).HasDatabaseName("IX_CategoryId");
                entity.HasIndex(e => e.AppUserId).HasDatabaseName("IX_AppUserId");
                entity.HasIndex(e => e.WarehouseId).HasDatabaseName("IX_WarehouseId");
                entity.HasIndex(e => new { e.IsActive, e.IsDeleted }).HasDatabaseName("IX_Product_IsActive_IsDeleted");
            });

            modelBuilder.Entity<StockMovement>(entity =>
            {
                entity.HasIndex(e => e.Id).HasDatabaseName("IX_StockMovement_Id").IsUnique();
                entity.HasIndex(e => e.Quantity).HasDatabaseName("IX_StockMovement_Quantity");
                entity.HasIndex(e => e.AppUserId).HasDatabaseName("IX_StockMovement_AppUserId");
                entity.HasIndex(e => e.ProductId).HasDatabaseName("IX_StockMovement_ProductId");
                entity.HasIndex(e => new { e.IsActive, e.IsDeleted }).HasDatabaseName("IX_StockMovement_IsActive_IsDeleted");
            });

            modelBuilder.Entity<UnitInStock>(entity =>
            {
                entity.HasIndex(e => e.Id).HasDatabaseName("IX_UnitInStock_Id").IsUnique();
                entity.Property(e => e.Code).IsRequired().HasMaxLength(6).IsFixedLength();
                entity.HasIndex(e => e.Quantity).HasDatabaseName("IX_UnitInStock_Quantity");
                entity.HasIndex(e => e.ProductId).HasDatabaseName("IX_UnitInStock_ProductId");
                entity.HasIndex(e => e.WarehouseId).HasDatabaseName("IX_UnitInStock_WarehouseId");
                entity.HasIndex(e => e.AppUserId).HasDatabaseName("IX_UnitInStock_AppUserId");
                entity.HasIndex(e => new { e.IsActive, e.IsDeleted }).HasDatabaseName("IX_UnitInStock_IsActive_IsDeleted");
            });

            modelBuilder.Entity<UserSession>(entity =>
            {
                entity.HasIndex(e => e.Id).HasDatabaseName("IX_UserSession_Id").IsUnique();
                entity.HasIndex(e => e.AppUserId).HasDatabaseName("IX_UserSession_AppUserId");
                entity.HasIndex(e => e.IsOnline).HasDatabaseName("IX_UserSession_IsOnline");
                entity.HasIndex(e => new { e.IsActive, e.IsDeleted }).HasDatabaseName("IX_UserSession_IsActive_IsDeleted");
            });

            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.HasIndex(e => e.Id).HasDatabaseName("IX_Warehouse_Id").IsUnique();
                entity.Property(e => e.Code).IsRequired().HasMaxLength(6).IsFixedLength();
                entity.HasIndex(e => new { e.IsActive, e.IsDeleted }).HasDatabaseName("IX_Warehouse_IsActive_IsDeleted");
            });

            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.HasIndex(e => e.Id).HasDatabaseName("IX_AppUser_Id").IsUnique();
                entity.HasIndex(e => e.IsLoginConfirmCodeActive).HasDatabaseName("IX_AppUser_IsLoginConfirmCodeActive");
                entity.HasIndex(e => new { e.IsActive, e.IsDeleted }).HasDatabaseName("IX_AppUser_IsActive_IsDeleted");
            });

            modelBuilder.Entity<AppRole>(entity =>
            {
                entity.HasIndex(e => e.Id).HasDatabaseName("IX_AppRole_Id").IsUnique();
                entity.HasIndex(e => new { e.IsActive, e.IsDeleted }).HasDatabaseName("IX_AppRole_IsActive_IsDeleted");
            });

            if (includeLocalEntities)
            {
                modelBuilder.Entity<OutboxEvent>(entity =>
                {
                    entity.HasIndex(e => e.Id).HasDatabaseName("IX_OutboxEvent_Id").IsUnique();
                    entity.HasIndex(e => e.IsProcessed).HasDatabaseName("IX_OutboxEvent_IsProcessed");
                    entity.HasIndex(e => new { e.IsActive, e.IsDeleted }).HasDatabaseName("IX_OutboxEvent_IsActive_IsDeleted");
                });
            }
        }
    }
}
