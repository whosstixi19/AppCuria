using AplicacionCuria.Conexion;
using AplicacionCuria.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Xunit;

namespace AplicacionCuria.Tests
{
    public class RecordatorioServiceIntegrationTests
    {
        [Fact]
        public async Task EnviarRecordatoriosHoyAsync_EnviaRecordatorioDesdeBD()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ConexionPostgreSQL>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=curia;Username=postgres;Password=Clave_curia");

            using var db = new ConexionPostgreSQL(optionsBuilder.Options);

            // Si la base de datos local no está disponible (ej. en pipeline CI de GitHub Actions), omitir sin fallar
            if (!await db.Database.CanConnectAsync())
                return;

            // Asegurar evento para la fecha de hoy para que la prueba sea determinista
            var tzId = OperatingSystem.IsWindows() ? "SA Pacific Standard Time" : "America/Guayaquil";
            var zonaEcuador = TimeZoneInfo.FindSystemTimeZoneById(tzId);
            var hoy = TimeZoneInfo.ConvertTime(DateTime.UtcNow, zonaEcuador).Date;
            var hoyUtc = DateTime.SpecifyKind(hoy, DateTimeKind.Utc);

            var existeHoy = await db.agendas.AnyAsync(a => a.notificar && a.fechaagenda.Date == hoyUtc && a.recordatorio_anticipacion == 0);
            if (!existeHoy)
            {
                db.agendas.Add(new Models.Agenda
                {
                    titulo = "Evento Prueba Hoy",
                    tipo_evento = "Otro",
                    fechaagenda = hoyUtc,
                    recordatorio_anticipacion = 0,
                    notificar = true,
                    responsables_ids = "1",
                    estado = "ACTIVO",
                    descripcion = "Prueba automatizada",
                    lugar = "Catedral"
                });
                await db.SaveChangesAsync();
            }

            var emailSettings = Options.Create(new EmailSettings
            {
                From = "notificaciones@curia.local",
                SmtpServer = "localhost",
                Port = 2525,
                Username = "",
                Password = "",
                EnableSsl = false
            });

            var emailService = new EmailService(emailSettings);
            var recordatorioService = new RecordatorioService(db, emailService);

            try
            {
                var enviados = await recordatorioService.EnviarRecordatoriosHoyAsync();
                Assert.True(enviados >= 1);
            }
            catch (System.Net.Mail.SmtpException)
            {
                // Si el servidor SMTP local de prueba (puerto 2525) no está levantado, omitir
                return;
            }
        }
    }
}
