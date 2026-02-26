using System.Linq.Expressions;
using StockManagement.Domain.EntityBase;

namespace StockManagement.DataAccess.GenericRepository
{
    public interface IEntityRepository<T> where T : class, IEntity, new()
    {
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null);
        Task<IEnumerable<T>> GetAllIncludeAsync(Expression<Func<T, bool>>[] filters, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAllIncludeByIdAsync(object id, string foreignKeyPropertyName, Expression<Func<T, bool>>[] conditions, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAllIncludingByPropertyPathAsync(object id, string foreignKeyPropertyPath, Expression<Func<T, bool>>[] conditions = null, params Expression<Func<T, object>>[] includes);
        Task<T> GetAsync(Expression<Func<T, bool>> filter, bool asNoTracking = false);
        Task<T> GetIncludeAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes);
        Task<bool> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
        Task<bool> DeleteByIdsAsync(IEnumerable<object> ids);
        IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null);
        IEnumerable<T> GetAllInclude(Expression<Func<T, bool>>[] filters, params Expression<Func<T, object>>[] includes);
        IEnumerable<T> GetAllIncludeById(object id, string foreignKeyPropertyName, Expression<Func<T, bool>>[] conditions, params Expression<Func<T, object>>[] includes);
        IEnumerable<T> GetAllIncludingByPropertyPath(object id, string foreignKeyPropertyPath, Expression<Func<T, bool>>[] conditions = null, params Expression<Func<T, object>>[] includes);
        T Get(Expression<Func<T, bool>> filter);
        T GetInclude(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes);
    }
}
