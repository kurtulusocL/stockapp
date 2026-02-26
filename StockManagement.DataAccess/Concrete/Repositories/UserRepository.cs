using Microsoft.EntityFrameworkCore;
using StockManagement.DataAccess.Abstract;
using StockManagement.DataAccess.Concrete.Context.EntityFramework.Mssql;
using StockManagement.DataAccess.GenericRepository.EntityFramework;
using StockManagement.Domain.Entities;

namespace StockManagement.DataAccess.Concrete.Repositories
{
    public class UserRepository : EntityRepositoryBase<AppUser, ApplicationDbContext>, IUserRepository
    {
        readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> SetActiveAsync(string id)
        {
            try
            {
                if (id == null)
                    throw new ArgumentNullException(nameof(id), "id was null");

                var data = await _context.Set<AppUser>().Where(i => i.Id == id).FirstOrDefaultAsync();
                if (data == null)
                {
                    return false;
                }
                data.IsActive = true;

                var audits = await _context.Set<Audit>().Where(a => a.AppUserId == id).ToListAsync();
                foreach (var audit in audits)
                {
                    audit.IsActive = true;
                }

                var stockMovements = await _context.Set<StockMovement>().Where(a => a.AppUserId == id).ToListAsync();
                foreach (var stockMovement in stockMovements)
                {
                    stockMovement.IsActive = true;
                }

                var products = await _context.Set<Product>().Where(a => a.AppUserId == id).ToListAsync();
                foreach (var product in products)
                {
                    product.IsActive = true;
                }

                var unitInStocks = await _context.Set<UnitInStock>().Where(a => a.AppUserId == id).ToListAsync();
                foreach (var unitInStock in unitInStocks)
                {
                    unitInStock.IsActive = true;
                }

                var userSessions = await _context.Set<UserSession>().Where(a => a.AppUserId == id).ToListAsync();
                foreach (var userSession in userSessions)
                {
                    userSession.IsActive = true;
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting Active the entity.", ex);
            }
        }

        public async Task<bool> SetActiveLoginConfirmCodeAsync(string id)
        {
            try
            {
                var active = await _context.Set<AppUser>().Where(i => i.Id == id).FirstOrDefaultAsync();
                if (active != null)
                {
                    active.IsLoginConfirmCodeActive = true;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting Active Login Confirm Code the entity.", ex);
            }
        }

        public async Task<bool> SetDeletedAsync(string id)
        {
            try
            {
                if (id == null)
                    throw new ArgumentNullException(nameof(id), "id was null");

                var data = await _context.Set<AppUser>().Where(i => i.Id == id).FirstOrDefaultAsync();
                if (data == null)
                {
                    return false;
                }
                data.IsDeleted = true;
                data.DeletedDate = DateTime.UtcNow;

                var audits = await _context.Set<Audit>().Where(a => a.AppUserId == id).ToListAsync();
                foreach (var audit in audits)
                {
                    audit.IsDeleted = true;
                    audit.DeletedDate = DateTime.UtcNow;
                }

                var stockMovements = await _context.Set<StockMovement>().Where(a => a.AppUserId == id).ToListAsync();
                foreach (var stockMovement in stockMovements)
                {
                    stockMovement.IsDeleted = true;
                    stockMovement.DeletedDate = DateTime.UtcNow;
                }

                var products = await _context.Set<Product>().Where(a => a.AppUserId == id).ToListAsync();
                foreach (var product in products)
                {
                    product.IsDeleted = true;
                    product.DeletedDate = DateTime.UtcNow;
                }

                var unitInStocks = await _context.Set<UnitInStock>().Where(a => a.AppUserId == id).ToListAsync();
                foreach (var unitInStock in unitInStocks)
                {
                    unitInStock.IsActive = true;
                    unitInStock.DeletedDate = DateTime.UtcNow;
                }

                var userSessions = await _context.Set<UserSession>().Where(a => a.AppUserId == id).ToListAsync();
                foreach (var userSession in userSessions)
                {
                    userSession.IsDeleted = true;
                    userSession.DeletedDate = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting Deleted the entity.", ex);
            }
        }

        public async Task<bool> SetInActiveAsync(string id)
        {
            try
            {
                if (id == null)
                    throw new ArgumentNullException(nameof(id), "id was null");

                var data = await _context.Set<AppUser>().Where(i => i.Id == id).FirstOrDefaultAsync();
                if (data == null)
                {
                    return false;
                }
                data.IsActive = false;
                data.SuspendedDate = DateTime.UtcNow;

                var audits = await _context.Set<Audit>().Where(a => a.AppUserId == id).ToListAsync();
                foreach (var audit in audits)
                {
                    audit.IsActive = false;
                    audit.SuspendedDate = DateTime.UtcNow;
                }

                var stockMovements = await _context.Set<StockMovement>().Where(a => a.AppUserId == id).ToListAsync();
                foreach (var stockMovement in stockMovements)
                {
                    stockMovement.IsActive = false;
                    stockMovement.SuspendedDate = DateTime.UtcNow;
                }

                var products = await _context.Set<Product>().Where(a => a.AppUserId == id).ToListAsync();
                foreach (var product in products)
                {
                    product.IsActive = false;
                    product.SuspendedDate = DateTime.UtcNow;
                }

                var unitInStocks = await _context.Set<UnitInStock>().Where(a => a.AppUserId == id).ToListAsync();
                foreach (var unitInStock in unitInStocks)
                {
                    unitInStock.IsActive = false;
                    unitInStock.SuspendedDate = DateTime.UtcNow;
                }

                var userSessions = await _context.Set<UserSession>().Where(a => a.AppUserId == id).ToListAsync();
                foreach (var userSession in userSessions)
                {
                    userSession.IsActive = false;
                    userSession.SuspendedDate = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting InActive the entity.", ex);
            }
        }

        public async Task<bool> SetInActiveLoginConfirmCodeAsync(string id)
        {
            try
            {
                var active = await _context.Set<AppUser>().Where(i => i.Id == id).FirstOrDefaultAsync();
                if (active != null)
                {
                    active.IsLoginConfirmCodeActive = false;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while setting DeActive Login Confirm Code the entity.", ex);
            }
        }

        public async Task<bool> SetNotDeletedAsync(string id)
        {
            try
            {
                if (id == null)
                    throw new ArgumentNullException(nameof(id), "id was null");

                var data = await _context.Set<AppUser>().Where(i => i.Id == id).FirstOrDefaultAsync();
                if (data == null)
                {
                    return false;
                }
                data.IsDeleted = false;

                var audits = await _context.Set<Audit>().Where(a => a.AppUserId == id).ToListAsync();
                foreach (var audit in audits)
                {
                    audit.IsDeleted = false;
                }

                var stockMovements = await _context.Set<StockMovement>().Where(a => a.AppUserId == id).ToListAsync();
                foreach (var stockMovement in stockMovements)
                {
                    stockMovement.IsDeleted = false;
                }

                var products = await _context.Set<Product>().Where(a => a.AppUserId == id).ToListAsync();
                foreach (var product in products)
                {
                    product.IsDeleted = false;
                }

                var unitInStocks = await _context.Set<UnitInStock>().Where(a => a.AppUserId == id).ToListAsync();
                foreach (var unitInStock in unitInStocks)
                {
                    unitInStock.IsActive = false;
                }

                var userSessions = await _context.Set<UserSession>().Where(a => a.AppUserId == id).ToListAsync();
                foreach (var userSession in userSessions)
                {
                    userSession.IsDeleted = false;
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
