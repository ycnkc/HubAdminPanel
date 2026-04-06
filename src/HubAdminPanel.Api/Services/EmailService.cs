using HubAdminPanel.Core.Entities;
using HubAdminPanel.Core.Interfaces;
using HubAdminPanel.Core.Entities;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace HubAdminPanel.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using (var smtpClient = new SmtpClient(_emailSettings.Host))
            {
                smtpClient.Port = _emailSettings.Port;
                smtpClient.Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password);
                smtpClient.EnableSsl = true;

                smtpClient.Timeout = 5000;

                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(_emailSettings.Email, _emailSettings.DisplayName);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.To.Add(to);

                    await Task.Run(() => smtpClient.Send(mailMessage));
                }
            }
        }
    }
}