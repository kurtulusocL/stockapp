using StockManagement.Domain.Entities;

namespace StockManagement.Business.Constants.Services
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}
