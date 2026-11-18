using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    public class SacerdotesController(ConexionPostgreSQL sql) : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Sacerdote model, IFormFile? firma)
        {
            try
            {
                var faltantes = new List<string>();
                if (string.IsNullOrWhiteSpace(model.nombre)) faltantes.Add("Nombre");
                if (string.IsNullOrWhiteSpace(model.apellido)) faltantes.Add("Apellido");
                if (string.IsNullOrWhiteSpace(model.cedula)) faltantes.Add("Cédula");
                if (string.IsNullOrWhiteSpace(model.telefono)) faltantes.Add("Teléfono");
                if (string.IsNullOrWhiteSpace(model.email)) faltantes.Add("Correo electrónico");
                if (string.IsNullOrWhiteSpace(model.estado)) faltantes.Add("Estado");
                if (firma == null || firma.Length == 0) faltantes.Add("Firma Digital");

                if (faltantes.Count > 0)
                {
                    return Json(new { success = false, message = "Falta ingresar los siguientes datos obligatorios: " + string.Join(", ", faltantes) + "." });
                }

                if (!ModelState.IsValid)
                {
                    var errores = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .Where(m => !string.IsNullOrWhiteSpace(m));
                    return Json(new { success = false, message = "Datos inválidos: " + string.Join("; ", errores) });
                }

                if (firma!.Length > 2 * 1024 * 1024)
                    return Json(new { success = false, message = "La firma digital no puede superar los 2 MB." });

                using var ms = new MemoryStream();
                await firma.CopyToAsync(ms);
                model.firma = ms.ToArray();
                model.observaciones ??= "";

                sql.Add(model);

                var nombreCompleto = $"{model.nombre} {model.apellido}".Trim();
                if (!sql.personas_evento.Any(p => p.email.ToLower() == model.email.Trim().ToLower() || p.nombre.ToLower() == nombreCompleto.ToLower()))
                {
                    sql.personas_evento.Add(new PersonasEvento
                    {
                        nombre = nombreCompleto,
                        email = model.email.Trim(),
                        telefono = model.telefono ?? "",
                        rol = "Sacerdote"
                    });
                }

                await sql.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "Error al guardar sacerdote: " + e.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [FromForm] Sacerdote model, IFormFile? firma)
        {
            if (id != model.id)
            {
                return NotFound();
            }

            try
            {
                var sacerdote = await sql.sacerdotes.FindAsync(id);
                if (sacerdote == null)
                    return NotFound();

                var faltantes = new List<string>();
                if (string.IsNullOrWhiteSpace(model.nombre)) faltantes.Add("Nombre");
                if (string.IsNullOrWhiteSpace(model.apellido)) faltantes.Add("Apellido");
                if (string.IsNullOrWhiteSpace(model.cedula)) faltantes.Add("Cédula");
                if (string.IsNullOrWhiteSpace(model.telefono)) faltantes.Add("Teléfono");
                if (string.IsNullOrWhiteSpace(model.email)) faltantes.Add("Correo electrónico");
                if (string.IsNullOrWhiteSpace(model.estado)) faltantes.Add("Estado");
                if ((sacerdote.firma == null || sacerdote.firma.Length == 0) && (firma == null || firma.Length == 0))
                    faltantes.Add("Firma Digital");

                if (faltantes.Count > 0)
                {
                    return Json(new { success = false, message = "Falta ingresar los siguientes datos obligatorios: " + string.Join(", ", faltantes) + "." });
                }

                if (!ModelState.IsValid)
                {
                    var errores = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .Where(m => !string.IsNullOrWhiteSpace(m));
                    return Json(new { success = false, message = "Datos inválidos: " + string.Join("; ", errores) });
                }

                var nombreAnterior = $"{sacerdote.nombre} {sacerdote.apellido}".Trim();
                var emailAnterior = (sacerdote.email ?? "").Trim();

                // Actualizar campos
                sacerdote.nombre = model.nombre;
                sacerdote.apellido = model.apellido;
                sacerdote.cedula = model.cedula;
                sacerdote.telefono = model.telefono;
                sacerdote.email = model.email;
                sacerdote.estado = model.estado;
                sacerdote.observaciones = model.observaciones ?? "";

                var nombreNuevo = $"{sacerdote.nombre} {sacerdote.apellido}".Trim();
                var personaAgenda = sql.personas_evento.FirstOrDefault(p =>
                    (!string.IsNullOrEmpty(emailAnterior) && p.email.ToLower() == emailAnterior.ToLower()) ||
                    p.nombre.ToLower() == nombreAnterior.ToLower());

                if (personaAgenda != null)
                {
                    personaAgenda.nombre = nombreNuevo;
                    personaAgenda.email = sacerdote.email.Trim();
                    personaAgenda.telefono = sacerdote.telefono ?? "";
                    personaAgenda.rol = "Sacerdote";
                }
                else
                {
                    sql.personas_evento.Add(new PersonasEvento
                    {
                        nombre = nombreNuevo,
                        email = sacerdote.email.Trim(),
                        telefono = sacerdote.telefono ?? "",
                        rol = "Sacerdote"
                    });
                }

                // Actualizar firma si viene una nueva
                if (firma != null && firma.Length > 0)
                {
                    if (firma.Length > 2 * 1024 * 1024)
                        return Json(new { success = false, message = "La firma digital no puede superar los 2 MB." });

                    using var ms = new MemoryStream();
                    await firma.CopyToAsync(ms);
                    sacerdote.firma = ms.ToArray();
                }

                sql.Update(sacerdote);
                await sql.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SacerdoteExists(model.id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "Error al actualizar sacerdote: " + e.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sacerdote = await sql.sacerdotes.FindAsync(id);
            if (sacerdote == null)
            {
                return NotFound();
            }
            sql.sacerdotes.Remove(sacerdote);
            sql.SaveChanges();
            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult GetSacerdotes()
        {
            var sacerdotes = sql.sacerdotes.Select(s => new
            {
                s.id,
                s.nombre,
                s.apellido,
                s.cedula,
                s.telefono,
                s.email,
                s.estado,
                s.observaciones
            }).ToList();

            return Json(sacerdotes);
        }

        [HttpGet]
        public IActionResult SearchByName(string nombre)
        {
            var filtro = (nombre ?? "").Trim().ToLower();
            var resultados = sql.sacerdotes
                .Where(s => (s.nombre + " " + s.apellido).ToLower().Contains(filtro) ||
                            (s.cedula ?? "").ToLower().Contains(filtro))
                .Select(s => new
                {
                    s.id,
                    s.nombre,
                    s.apellido,
                    s.cedula,
                    s.telefono,
                    s.email,
                    s.estado,
                    s.observaciones
                })
                .ToList();

            return Json(resultados);
        }

        private bool SacerdoteExists(int id)
        {
            return sql.sacerdotes.Any(e => e.id == id);
        }
    }
}