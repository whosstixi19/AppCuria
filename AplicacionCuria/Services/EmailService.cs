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

        public async Task SendCalendarInviteAsync(
            string toEmail,
            string subject,
            string bodyHtml,
            string uid,
            string summary,
            string description,
            string location,
            DateTime startUtc,
            DateTime endUtc)
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
                    Subject = subject
                };
                mailMessage.To.Add(toEmail);

                // Vista HTML
                var htmlView = AlternateView.CreateAlternateViewFromString(bodyHtml, System.Text.Encoding.UTF8, "text/html");
                mailMessage.AlternateViews.Add(htmlView);

                // Contenido iCalendar (RFC 5545)
                var ics = new System.Text.StringBuilder();
                ics.AppendLine("BEGIN:VCALENDAR");
                ics.AppendLine("PRODID:-//Curia Diocesana//Sistema Curia//ES");
                ics.AppendLine("VERSION:2.0");
                ics.AppendLine("METHOD:REQUEST");
                ics.AppendLine("BEGIN:VEVENT");
                ics.AppendLine($"UID:{uid}");
                ics.AppendLine($"DTSTAMP:{DateTime.UtcNow:yyyyMMdd\\THHmmss\\Z}");
                ics.AppendLine($"DTSTART:{startUtc:yyyyMMdd\\THHmmss\\Z}");
                ics.AppendLine($"DTEND:{endUtc:yyyyMMdd\\THHmmss\\Z}");
                ics.AppendLine($"SUMMARY:{EscapeIcs(summary)}");
                ics.AppendLine($"DESCRIPTION:{EscapeIcs(description)}");
                if (!string.IsNullOrWhiteSpace(location))
                {
                    ics.AppendLine($"LOCATION:{EscapeIcs(location)}");
                }
                ics.AppendLine($"ORGANIZER;CN=\"Sistema Curia\":MAILTO:{fromAddress}");
                ics.AppendLine($"ATTENDEE;ROLE=REQ-PARTICIPANT;PARTSTAT=NEEDS-ACTION;RSVP=TRUE:MAILTO:{toEmail}");
                ics.AppendLine("STATUS:CONFIRMED");
                ics.AppendLine("SEQUENCE:0");
                ics.AppendLine("END:VEVENT");
                ics.AppendLine("END:VCALENDAR");

                var icsBytes = System.Text.Encoding.UTF8.GetBytes(ics.ToString());

                // Vista de calendario interactiva (Google Calendar, Outlook y smartphones)
                var calendarStream = new System.IO.MemoryStream(icsBytes);
                var calendarView = new AlternateView(calendarStream, new System.Net.Mime.ContentType("text/calendar; method=REQUEST; charset=utf-8"));
                calendarView.TransferEncoding = System.Net.Mime.TransferEncoding.SevenBit;
                mailMessage.AlternateViews.Add(calendarView);

                // Adjunto .ics de respaldo
                var attachment = new Attachment(new System.IO.MemoryStream(icsBytes), "invitacion.ics", "text/calendar; method=REQUEST; name=\"invitacion.ics\"");
                mailMessage.Attachments.Add(attachment);

                await client.SendMailAsync(mailMessage);
            }
        }

        private static string EscapeIcs(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Replace("\\", "\\\\")
                       .Replace(";", "\\;")
                       .Replace(",", "\\,")
                       .Replace("\r\n", "\\n")
                       .Replace("\n", "\\n");
        }
    }
}
