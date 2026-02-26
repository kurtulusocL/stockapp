using System.Linq.Expressions;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StockManagement.Business.Services.Abstract;
using StockManagement.DataAccess.Concrete.Context.Azure;

namespace StockManagement.Business.Services.Concrete
{
    public class AzureManager : IAzureService
    {
        private readonly string _storageConnectionString;
        private readonly string _azureConnectionString;
        private readonly string _containerName = "stock-management-images";

        public AzureManager(IConfiguration configuration)
        {
            _storageConnectionString = configuration.GetConnectionString("AzureStorageConnection");
            _azureConnectionString = configuration.GetConnectionString("AzureConnection");
        }
        public async Task<IEnumerable<T>> GetAllFromAzureAsync<T>(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes) where T : class
        {
            var optionsBuilder = new DbContextOptionsBuilder<AzureDbContext>();
            optionsBuilder.UseSqlServer(_azureConnectionString);

            using (var azureContext = new AzureDbContext(optionsBuilder.Options))
            {
                IQueryable<T> query = azureContext.Set<T>();
                if (filter != null) query = query.Where(filter);
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
                return await query.ToListAsync();
            }
        }

        public async Task<T> GetFromAzureWithIncludesAsync<T>(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes) where T : class
        {
            var optionsBuilder = new DbContextOptionsBuilder<AzureDbContext>();
            optionsBuilder.UseSqlServer(_azureConnectionString);
            using (var azureContext = new AzureDbContext(optionsBuilder.Options))
            {
                IQueryable<T> query = azureContext.Set<T>();
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
                return await query.FirstOrDefaultAsync(filter);
            }
        }

        public T GetFromAzureWithIncludes<T>(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes) where T : class
        {
            var optionsBuilder = new DbContextOptionsBuilder<AzureDbContext>();
            optionsBuilder.UseSqlServer(_azureConnectionString);
            using (var azureContext = new AzureDbContext(optionsBuilder.Options))
            {
                IQueryable<T> query = azureContext.Set<T>();
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
                return query.FirstOrDefault(filter);
            }
        }

        public async Task<T> GetFromAzureAsync<T>(object id) where T : class
        {
            var optionsBuilder = new DbContextOptionsBuilder<AzureDbContext>();
            optionsBuilder.UseSqlServer(_azureConnectionString);

            using (var azureContext = new AzureDbContext(optionsBuilder.Options))
            {
                return azureContext.Set<T>().Find(id);
            }
        }

        public async Task<string> UploadImageToAzureAsync(IFormFile file) //You can use this method when you want to physically send a file to Azure.
        {
            if (file == null || file.Length == 0) return null;

            var blobServiceClient = new BlobServiceClient(_storageConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            await containerClient.CreateIfNotExistsAsync();

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var blobClient = containerClient.GetBlobClient(fileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }
            return blobClient.Uri.ToString();
        }
    }
}
