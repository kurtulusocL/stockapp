using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using StockManagement.Business.Constants.Services;
using StockManagement.Shared.Helpers.Configurations;

namespace StockManagement.Business.Constants.Utilities.Mail
{
    public class MailManager : IMailService
    {
        private readonly MailSettings _mailSettings;
        public MailManager(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail));

            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_mailSettings.Server, _mailSettings.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_mailSettings.SenderEmail, _mailSettings.Password);
                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                throw new Exception($"Email delivery failed.: {ex.Message}", ex);
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
