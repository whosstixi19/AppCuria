using AplicacionCuria.Conexion;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace AplicacionCuria.Services
{
    /// <summary>
    /// Envío de recordatorios de agenda. Se usa desde el controlador
    /// (petición del usuario) y desde RecordatorioBackgroundService (proceso de fondo),
    /// sin llamadas HTTP internas.
    /// </summary>
    public class RecordatorioService(ConexionPostgreSQL sql, EmailService emailService)
    {
        public async Task<int> EnviarRecordatoriosHoyAsync()
        {
            // Fecha "hoy" en hora de Ecuador (la app asume SA Pacific Standard Time)
            // Se marca como UTC solo para que Npgsql la escriba en la comparación timestamptz;
            // la parte de fecha es la del día ecuatoriano.
            var zonaEcuador = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            var hoy = DateTime.SpecifyKind(
                TimeZoneInfo.ConvertTime(DateTime.UtcNow, zonaEcuador).Date,
                DateTimeKind.Utc);

            var agendas = await sql.agendas
                .Where(a => a.notificar && a.fechaagenda.AddDays(-a.recordatorio_anticipacion).Date == hoy)
                .ToListAsync();

            int enviados = 0;

            foreach (var agenda in agendas)
            {
                var responsables = agenda.responsables_ids?.Split(',').ToList() ?? new List<string>();

                foreach (var responsableId in responsables)
                {
                    if (int.TryParse(responsableId, out int id))
                    {
                        var persona = await sql.personas_evento.FirstOrDefaultAsync(p => p.id == id);

                        if (persona == null)
                        {
                            Console.WriteLine($"[ERROR] No se encontró persona con ID: {id}");
                            continue;
                        }

                        if (string.IsNullOrEmpty(persona.email))
                        {
                            Console.WriteLine($"[ERROR] Persona con ID {id} no tiene email.");
                            continue;
                        }

                        // Convertir hora a AM/PM
                        string horaAmPm = agenda.hora.HasValue
                            ? DateTime.Today.Add(agenda.hora.Value).ToString("hh:mm tt", CultureInfo.InvariantCulture)
                            : "Sin hora definida";

                        string body = $@"
                        <html>
                        <body>
                            <h2>🔔 Recordatorio: {agenda.titulo}</h2>
                            <p>Estimado <strong>{persona.nombre}</strong>,</p>
                            <p>Evento: {agenda.tipo_evento}</p>
                            <p>Fecha: {agenda.fechaagenda:dd/MM/yyyy}</p>
                            <p>Hora: {horaAmPm}</p>
                            <p>Lugar: {agenda.lugar}</p>
                            <p><b>Mensaje:</b> {agenda.descripcion}</p>
                        </body>
                        </html>";

                        await emailService.SendEmailAsync(persona.email, $"Recordatorio: {agenda.titulo}", body);
                        enviados++;
                    }
                    else
                    {
                        Console.WriteLine($"[ERROR] ID inválido en responsables: '{responsableId}'");
                    }
                }
            }

            return enviados;
        }
    }
}
