using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    public class EncargadosController(ConexionPostgreSQL sql) : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PersonasEvento model)
        {
            if (!ModelState.IsValid)
            {
                // Devolver los errores de validación al frontend
                var errores = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                return Json(new { success = false, message = errores });
            }

            try
            {
                sql.personas_evento.Add(model);
                await sql.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Edit(int id, PersonasEvento model)
        {
            if (id != model.id)
            {
                return NotFound();
            }
            try
            {
                sql.Update(model);
                sql.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonaEventoExists(model.id))
                {
                    return NotFound();
                }
                throw;
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var persona = await sql.personas_evento.FindAsync(id);
                if (persona == null)
                {
                    return Json(new { success = false, message = "No se encontró el encargado." });
                }

                sql.personas_evento.Remove(persona);
                await sql.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetEncargados()
        {
            var encargados = sql.personas_evento.Select(p => new
            {
                p.id,
                p.nombre,
                p.email,
                p.telefono,
                p.rol
            }).ToList();

            return Json(encargados);
        }

        [HttpGet]
        public IActionResult SearchByName(string nombre)
        {
            var resultados = sql.personas_evento
                .Where(p => p.nombre.ToLower().Contains(nombre.ToLower()))
                .ToList();

            return Json(resultados);
        }

        private bool PersonaEventoExists(int id)
        {
            return sql.personas_evento.Any(e => e.id == id);
        }
    }
}
