using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AplicacionCuria.Services
{
    public class EmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("El correo del destinatario está vacío o nulo.", nameof(toEmail));

            if (string.IsNullOrWhiteSpace(_settings?.SmtpServer))
                return;

            using (var client = new SmtpClient(_settings.SmtpServer, _settings.Port))
            {
                if (!string.IsNullOrWhiteSpace(_settings.Username) && !string.IsNullOrWhiteSpace(_settings.Password))
                {
                    client.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
                }
                client.EnableSsl = _settings.EnableSsl;

                var fromAddress = !string.IsNullOrWhiteSpace(_settings.From) ? _settings.From : _settings.Username;
                if (string.IsNullOrWhiteSpace(fromAddress))
                {
                    fromAddress = "noreply@curia.local";
                }

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromAddress, "Sistema Curia"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
