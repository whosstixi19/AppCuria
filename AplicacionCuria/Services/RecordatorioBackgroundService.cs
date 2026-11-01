using Microsoft.Extensions.Hosting;

namespace AplicacionCuria.Services
{
    public class RecordatorioBackgroundService(IServiceScopeFactory scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var recordatorios = scope.ServiceProvider.GetRequiredService<RecordatorioService>();
                    var enviados = await recordatorios.EnviarRecordatoriosHoyAsync();

                    Console.WriteLine($"[OK] Recordatorios enviados: {DateTime.Now} ({enviados})");

                    // Revisar de nuevo mañana
                    await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Cierre de la aplicación: salir sin error
                    break;
                }
                catch (Exception ex)
                {
                    // Fallo transitorio (SMTP caído, etc.: reintentar pronto, no esperar 24h)
                    Console.WriteLine($"[ERROR] RecordatorioBackgroundService: {ex.Message}");
                    await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
                }
            }
        }
    }
}
