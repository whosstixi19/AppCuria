using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using AplicacionCuria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    public class AgendaController(ConexionPostgreSQL sql, EmailService emailService) : Controller
    {
        [HttpGet]
        public IActionResult GetAllPersonas()
        {
            var personas = sql.personas_evento
                .Select(p => new {
                    p.id,
                    p.nombre,
                    p.email
                })
                .ToList();

            return Json(personas);
        }

        [HttpGet]
        public IActionResult MenuAgenda()
        {
            var agendas = sql.agendas.ToList();
            return View(agendas);
        }
        
    

        #region ingresosCRUD
        [HttpPost]
        public async Task<IActionResult> Create()
        {
            try
            {
                var responsablesRaw = Request.Form["responsables_ids"];
                List<int> responsablesIds = new List<int>();

                if (!string.IsNullOrEmpty(responsablesRaw))
                {
                    responsablesIds = System.Text.Json.JsonSerializer.Deserialize<List<int>>(responsablesRaw);
                }

                // Zona horaria de Ecuador
                var tzId = OperatingSystem.IsWindows() ? "SA Pacific Standard Time" : "America/Guayaquil";
                var zonaEcuador = TimeZoneInfo.FindSystemTimeZoneById(tzId);

                // Crear modelo Agenda
                var model = new Agenda
                {
                    titulo = Request.Form["titulo"],
                    tipo_evento = Request.Form["tipo_evento"],
                    descripcion = Request.Form["descripcion"],
                    lugar = Request.Form["lugar"],
                    estado = Request.Form["estado"],
                    notificar = Request.Form["notificar"] == "true",
                    recordatorio_anticipacion = int.Parse(Request.Form["recordatorio_anticipacion"]),

                    // Ajuste de fecha para que no reste un día
                    fechaagenda = TimeZoneInfo.ConvertTimeToUtc(
                        DateTime.SpecifyKind(DateTime.Parse(Request.Form["fechaagenda"]), DateTimeKind.Unspecified),
                        zonaEcuador
                    ),

                    hora = TimeSpan.Parse(Request.Form["hora"]),
                    responsables_ids = string.Join(",", responsablesIds)
                };

                sql.agendas.Add(model);
                await sql.SaveChangesAsync();

                if (model.notificar && responsablesIds.Count > 0)
                {
                    try
                    {
                        await NotificarResponsablesNuevaCitaAsync(model, responsablesIds);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[AVISO] No se pudo enviar la invitación por correo: {ex.Message}");
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private async Task NotificarResponsablesNuevaCitaAsync(Agenda agenda, List<int> responsablesIds)
        {
            var tzId = OperatingSystem.IsWindows() ? "SA Pacific Standard Time" : "America/Guayaquil";
            var zonaEcuador = TimeZoneInfo.FindSystemTimeZoneById(tzId);

            var hora = agenda.hora ?? TimeSpan.FromHours(8);
            var fechaLocal = TimeZoneInfo.ConvertTimeFromUtc(agenda.fechaagenda, zonaEcuador).Date;
            var inicioLocal = fechaLocal.Add(hora);
            var startUtc = TimeZoneInfo.ConvertTimeToUtc(inicioLocal, zonaEcuador);
            var endUtc = startUtc.AddHours(1);

            string horaAmPm = DateTime.Today.Add(hora).ToString("hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);

            var personas = await sql.personas_evento
                .Where(p => responsablesIds.Contains(p.id))
                .ToListAsync();

            foreach (var persona in personas)
            {
                if (string.IsNullOrWhiteSpace(persona.email)) continue;

                string subject = $"📅 Nueva Cita: {agenda.titulo}";
                string body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Nueva Cita Agendada: {agenda.titulo}</h2>
                    <p>Estimado/a <strong>{persona.nombre}</strong>,</p>
                    <p>Se ha registrado un evento en la agenda parroquial:</p>
                    <ul>
                        <li><strong>Tipo:</strong> {agenda.tipo_evento}</li>
                        <li><strong>Fecha:</strong> {fechaLocal:dd/MM/yyyy}</li>
                        <li><strong>Hora:</strong> {horaAmPm}</li>
                        <li><strong>Lugar:</strong> {agenda.lugar}</li>
                        <li><strong>Descripción:</strong> {agenda.descripcion}</li>
                    </ul>
                    <p style='color: #666;'>Este evento ha sido añadido a tu calendario.</p>
                </body>
                </html>";

                string uid = $"agenda-{agenda.id}-{persona.id}@curia.local";
                await emailService.SendCalendarInviteAsync(
                    persona.email,
                    subject,
                    body,
                    uid,
                    agenda.titulo ?? "Cita Parroquial",
                    agenda.descripcion ?? string.Empty,
                    agenda.lugar ?? string.Empty,
                    startUtc,
                    endUtc
                );
            }
        }


        [HttpPost]
        public IActionResult Edit(int id, Agenda model)
        {
            if (id != model.id)
            {
                return NotFound();
            }
            try
            {

                model.fechaagenda = DateTime.SpecifyKind(model.fechaagenda, DateTimeKind.Utc);

                sql.Update(model);
                sql.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AgendaExists(model.id))
                {
                    return NotFound();
                }
                throw;
            }
            return Json(new { success = true });

            //return View("~/Views/Submenus/BautizadosView.cshtml");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var agen = await sql.agendas.FindAsync(id);
            if (agen == null)
            {
                return NotFound();
            }
            else
            {
                sql.agendas.Remove(agen);
                sql.SaveChanges();
            }
            return Json(new { success = true });
        }
        #endregion

        #region abrirpestanias
        public async Task<IActionResult> ModifyAgendaView(int? id)
        {
            if (id == null || sql.agendas == null)
            {
                return NotFound();
            }

            var usuario = await sql.agendas.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }
        public async Task<IActionResult> Valores(int id)
        {
            var usuario = await sql.agendas.FindAsync(id);
            return Json(usuario);
        }
        public async Task<IActionResult> DeleteAgendaView(int? id)
        {
            if (id == null || sql.agendas == null)
            {
                return NotFound();
            }

            var usuario = await sql.agendas.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }
        private bool AgendaExists(int id)
        {
            return sql.agendas.Any(e => e.id == id);
        }

        #endregion


        [HttpGet]
    public IActionResult GetAgendas()
    {
        // Traemos todas las agendas primero
        var agendas = sql.agendas
            .AsNoTracking() // No trackeamos, mejora rendimiento
            .ToList();

        // Preparamos el listado de IDs de responsables (para hacer una sola consulta a personas_evento)
        var todosIds = agendas
            .Where(a => !string.IsNullOrEmpty(a.responsables_ids))
            .SelectMany(a => a.responsables_ids.Split(',').Select(id => int.Parse(id)))
            .Distinct()
            .ToList();

        // Cargamos todos los responsables de una sola vez
        var responsables = sql.personas_evento
            .Where(p => todosIds.Contains(p.id))
            .ToDictionary(p => p.id, p => p.nombre);

        // Generamos la respuesta JSON
        var resultado = agendas.Select(a => new
        {
            a.id,
            a.fechaagenda,
            a.descripcion,
            a.hora,
            a.lugar,
            responsables = ConvertirIdsANombres(a.responsables_ids, responsables),
            a.estado
        }).ToList();

        return Json(resultado);
    }

    private string ConvertirIdsANombres(string ids, Dictionary<int, string> responsables)
    {
        if (string.IsNullOrEmpty(ids)) return string.Empty;
        var idsArray = ids.Split(',').Select(id => int.Parse(id)).ToList();
        return string.Join(", ", idsArray.Select(id => responsables.ContainsKey(id) ? responsables[id] : $"ID {id}"));
    }



        [HttpGet]
        public IActionResult SearchByName(string? nombrecompleto)
        {
            var filtro = (nombrecompleto ?? "").Trim().ToLower();

            var agendas = sql.agendas
                .AsNoTracking()
                .Where(a => string.IsNullOrEmpty(filtro)
                         || (a.descripcion ?? "").ToLower().Contains(filtro)
                         || (a.titulo ?? "").ToLower().Contains(filtro)
                         || (a.lugar ?? "").ToLower().Contains(filtro))
                .ToList();

            var todosIds = agendas
                .Where(a => !string.IsNullOrEmpty(a.responsables_ids))
                .SelectMany(a => a.responsables_ids.Split(',').Select(id => int.Parse(id)))
                .Distinct()
                .ToList();

            var responsables = sql.personas_evento
                .Where(p => todosIds.Contains(p.id))
                .ToDictionary(p => p.id, p => p.nombre);

            var resultado = agendas.Select(a => new
            {
                a.id,
                a.fechaagenda,
                a.descripcion,
                a.hora,
                a.lugar,
                responsables = ConvertirIdsANombres(a.responsables_ids, responsables),
                a.estado
            }).ToList();

            return Json(resultado);
        }
    }
}
