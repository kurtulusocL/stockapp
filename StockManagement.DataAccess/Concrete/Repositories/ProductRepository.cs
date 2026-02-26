using Microsoft.EntityFrameworkCore;
using StockManagement.DataAccess.Abstract;
using StockManagement.DataAccess.Concrete.Context.EntityFramework.Mssql;
using StockManagement.DataAccess.GenericRepository.EntityFramework;
using StockManagement.Domain.Entities;

namespace StockManagement.DataAccess.Concrete.Repositories
{
    public class ProductRepository : EntityRepositoryBase<Product, ApplicationDbContext>, IProductRepository
    {
        readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> SetActiveAsync(int id)
        {
            try
            {
                if (id < 0)
                    throw new ArgumentNullException(nameof(id), "id was null");

                var data = await _context.Set<Product>().Where(i => i.Id == id).FirstOrDefaultAsync();
                if (data == null)
                {
                    return false;
                }
                data.IsActive = true;

                var stockMovements = await _context.Set<StockMovement>().Where(a => a.ProductId == id).ToListAsync();
                foreach (var stockMovement in stockMovements)
                {
                    stockMovement.IsActive = true;
                }

                var unitInStocks = await _context.Set<UnitInStock>().Where(a => a.ProductId == id).ToListAsync();
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

                var data = await _context.Set<Product>().Where(i => i.Id == id).FirstOrDefaultAsync();
                if (data == null)
                {
                    return false;
                }
                data.IsDeleted = true;
                data.DeletedDate = DateTime.UtcNow;

                var stockMovements = await _context.Set<StockMovement>().Where(a => a.ProductId == id).ToListAsync();
                foreach (var stockMovement in stockMovements)
                {
                    stockMovement.IsDeleted = true;
                    stockMovement.DeletedDate = DateTime.UtcNow;
                }

                var unitInStocks = await _context.Set<UnitInStock>().Where(a => a.ProductId == id).ToListAsync();
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

                var data = await _context.Set<Product>().Where(i => i.Id == id).FirstOrDefaultAsync();
                if (data == null)
                {
                    return false;
                }
                data.IsActive = false;
                data.SuspendedDate = DateTime.UtcNow;

                var stockMovements = await _context.Set<StockMovement>().Where(a => a.ProductId == id).ToListAsync();
                foreach (var stockMovement in stockMovements)
                {
                    stockMovement.IsActive = false;
                    stockMovement.SuspendedDate = DateTime.UtcNow;
                }

                var unitInStocks = await _context.Set<UnitInStock>().Where(a => a.ProductId == id).ToListAsync();
                foreach (var unitInStock in unitInStocks)
                {
                    unitInStock.IsActive = false;
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

                var data = await _context.Set<Product>().Where(i => i.Id == id).FirstOrDefaultAsync();
                if (data == null)
                {
                    return false;
                }
                data.IsDeleted = false;

                var stockMovements = await _context.Set<StockMovement>().Where(a => a.ProductId == id).ToListAsync();
                foreach (var stockMovement in stockMovements)
                {
                    stockMovement.IsDeleted = false;
                }

                var unitInStocks = await _context.Set<UnitInStock>().Where(a => a.ProductId == id).ToListAsync();
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
