using Microsoft.EntityFrameworkCore;
using StockManagement.DataAccess.Abstract;
using StockManagement.DataAccess.Concrete.Context.EntityFramework.Mssql;
using StockManagement.DataAccess.GenericRepository.EntityFramework;
using StockManagement.Domain.Entities;

namespace StockManagement.DataAccess.Concrete.Repositories
{
    public class WarehouseRepository : EntityRepositoryBase<Warehouse, ApplicationDbContext>, IWarehouseRepository
    {
        readonly ApplicationDbContext _context;
        public WarehouseRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> SetActiveAsync(int id)
        {
            try
            {
                if (id < 0)
                    throw new ArgumentNullException(nameof(id), "id was null");

                var data = await _context.Set<Warehouse>().Where(i => i.Id == id).FirstOrDefaultAsync();
                if (data == null)
                {
                    return false;
                }
                data.IsActive = true;
                
                var products = await _context.Set<Product>().Where(a => a.WarehouseId == id).ToListAsync();
                foreach (var product in products)
                {
                    product.IsActive = true;
                }
                var unitInStocks = await _context.Set<UnitInStock>().Where(a => a.WarehouseId == id).ToListAsync();
                foreach (var unitInStock in unitInStocks)
                {
                    unitInStock.IsActive = true;
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting Active the entity.", ex);
            }
        }

        public async Task<bool> SetDeletedAsync(int id)
        {
            try
            {
                if (id < 0)
                    throw new ArgumentNullException(nameof(id), "id was null");

                var data = await _context.Set<Warehouse>().Where(i => i.Id == id).FirstOrDefaultAsync();
                if (data == null)
                {
                    return false;
                }
                data.IsDeleted = true;
                data.DeletedDate = DateTime.UtcNow;

                var products = await _context.Set<Product>().Where(a => a.WarehouseId == id).ToListAsync();
                foreach (var product in products)
                {
                    product.IsDeleted = true;
                    product.DeletedDate = DateTime.UtcNow;
                }
                var unitInStocks = await _context.Set<UnitInStock>().Where(a => a.WarehouseId == id).ToListAsync();
                foreach (var unitInStock in unitInStocks)
                {
                    unitInStock.IsDeleted = true;
                    unitInStock.DeletedDate = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting Deleted the entity.", ex);
            }
        }

        public async Task<bool> SetInActiveAsync(int id)
        {
            try
            {
                if (id < 0)
                    throw new ArgumentNullException(nameof(id), "id was null");

                var data = await _context.Set<Warehouse>().Where(i => i.Id == id).FirstOrDefaultAsync();
                if (data == null)
                {
                    return false;
                }
                data.IsActive = true;
                data.SuspendedDate = DateTime.UtcNow;

                var products = await _context.Set<Product>().Where(a => a.WarehouseId == id).ToListAsync();
                foreach (var product in products)
                {
                    product.IsActive = true;
                    product.SuspendedDate = DateTime.UtcNow;
                }
                var unitInStocks = await _context.Set<UnitInStock>().Where(a => a.WarehouseId == id).ToListAsync();
                foreach (var unitInStock in unitInStocks)
                {
                    unitInStock.IsActive = true;
                    unitInStock.SuspendedDate = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting InActive the entity.", ex);
            }
        }

        public async Task<bool> SetNotDeletedAsync(int id)
        {
            try
            {
                if (id < 0)
                    throw new ArgumentNullException(nameof(id), "id was null");

                var data = await _context.Set<Warehouse>().Where(i => i.Id == id).FirstOrDefaultAsync();
                if (data == null)
                {
                    return false;
                }
                data.IsDeleted = false;

                var products = await _context.Set<Product>().Where(a => a.WarehouseId == id).ToListAsync();
                foreach (var product in products)
                {
                    product.IsDeleted = false;
                }
                var unitInStocks = await _context.Set<UnitInStock>().Where(a => a.WarehouseId == id).ToListAsync();
                foreach (var unitInStock in unitInStocks)
                {
                    unitInStock.IsDeleted = false;
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting Not Deleted the entity.", ex);
            }
        }
    }
}
