using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;

namespace StockManagement.Business.Services.Abstract
{
    public interface IAzureService
    {
        Task<T> GetFromAzureWithIncludesAsync<T>(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes) where T : class;
        Task<IEnumerable<T>> GetAllFromAzureAsync<T>(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes) where T : class;
        Task<T> GetFromAzureAsync<T>(object id) where T : class;
        Task<string> UploadImageToAzureAsync(IFormFile file);
        T GetFromAzureWithIncludes<T>(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes) where T : class;
    }
}
