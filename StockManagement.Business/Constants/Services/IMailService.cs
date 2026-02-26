
namespace StockManagement.Business.Constants.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
