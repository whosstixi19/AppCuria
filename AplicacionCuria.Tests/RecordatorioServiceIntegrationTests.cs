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

            var enviados = await recordatorioService.EnviarRecordatoriosHoyAsync();
            Assert.True(enviados >= 1);
        }
    }
}
