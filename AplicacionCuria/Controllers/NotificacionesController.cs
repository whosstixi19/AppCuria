using AplicacionCuria.Conexion;
using AplicacionCuria.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace AplicacionCuria.Controllers
{
    [Route("[controller]/[action]")]
    public class NotificacionesController : Controller
    {
        private readonly ConexionPostgreSQL _sql;
        private readonly EmailService _emailService;

        public NotificacionesController(ConexionPostgreSQL sql, EmailService emailService)
        {
            _sql = sql;
            _emailService = emailService;
        }

        /// <summary>
        /// Envía recordatorios de agendas que cumplen la fecha actual.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> EnviarRecordatorios()
        {
            try
            {
                var hoy = DateTime.UtcNow.Date;

                var agendas = await _sql.agendas
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
                            var persona = await _sql.personas_evento.FirstOrDefaultAsync(p => p.id == id);

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

                            Console.WriteLine($"[INFO] Enviando correo a: {persona.email} para evento {agenda.titulo}");

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

                            await _emailService.SendEmailAsync(persona.email, $"Recordatorio: {agenda.titulo}", body);
                            enviados++;
                        }
                        else
                        {
                            Console.WriteLine($"[ERROR] ID inválido en responsables: '{responsableId}'");
                        }
                    }

                }

                return Json(new { enviado = true, total = enviados });
            }
            catch (Exception ex)
            {
                return Json(new { enviado = false, error = ex.Message });
            }
        }
    }
}
