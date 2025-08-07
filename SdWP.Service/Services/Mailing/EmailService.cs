using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace SdWP.Service.Services.Mailing
{
    public class EmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendTestEmailAsync(string toEmail)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FormName, _settings.FromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "Test Email from SdWP Service";

            message.Body = new TextPart("plain")
            {
                Text = "This is a test email sent from the SdWP Service."
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, MailKit.Security.SecureSocketOptions.None);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
