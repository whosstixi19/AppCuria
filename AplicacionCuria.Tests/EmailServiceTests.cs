using AplicacionCuria.Services;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AplicacionCuria.Tests
{
    public class EmailServiceTests
    {
        [Fact]
        public async Task SendEmailAsync_CorreoVacio_LanzaArgumentException()
        {
            var options = Options.Create(new EmailSettings());
            var service = new EmailService(options);

            await Assert.ThrowsAsync<ArgumentException>(() => service.SendEmailAsync("", "Asunto", "Cuerpo"));
        }

        [Fact]
        public async Task SendEmailAsync_SmtpServerNoConfigurado_NoLanzaExcepcion()
        {
            var options = Options.Create(new EmailSettings { SmtpServer = "" });
            var service = new EmailService(options);

            var exception = await Record.ExceptionAsync(() => service.SendEmailAsync("destino@curia.local", "Asunto", "Cuerpo"));

            Assert.Null(exception);
        }

        [Fact]
        public async Task SendCalendarInviteAsync_CorreoVacio_LanzaArgumentException()
        {
            var options = Options.Create(new EmailSettings());
            var service = new EmailService(options);

            await Assert.ThrowsAsync<ArgumentException>(() => service.SendCalendarInviteAsync(
                "", "Asunto", "<p>Test</p>", "uid-1", "Titulo", "Desc", "Lugar", DateTime.UtcNow, DateTime.UtcNow.AddHours(1)
            ));
        }

        [Fact]
        public async Task SendCalendarInviteAsync_SmtpServerNoConfigurado_NoLanzaExcepcion()
        {
            var options = Options.Create(new EmailSettings { SmtpServer = "" });
            var service = new EmailService(options);

            var exception = await Record.ExceptionAsync(() => service.SendCalendarInviteAsync(
                "destino@curia.local", "Asunto", "<p>Test</p>", "uid-1", "Titulo", "Desc", "Lugar", DateTime.UtcNow, DateTime.UtcNow.AddHours(1)
            ));

            Assert.Null(exception);
        }
    }
}
