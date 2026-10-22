using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AplicacionCuria.Services
{
    public class RecordatorioBackgroundService : BackgroundService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RecordatorioBackgroundService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var client = _httpClientFactory.CreateClient();
                    var response = await client.GetAsync("http://localhost:5155/Notificaciones/EnviarRecordatorios");

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"[OK] Recordatorios enviados: {DateTime.Now}");
                    }
                    else
                    {
                        Console.WriteLine($"[ERROR] No se pudieron enviar los recordatorios: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] BackgroundService: {ex.Message}");
                }

                // Esperar 24 horas
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
