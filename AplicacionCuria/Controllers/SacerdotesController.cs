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
                if (ModelState.IsValid)
                {
                    // Si viene una firma, convertirla a bytes
                    if (firma != null && firma.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        await firma.CopyToAsync(ms);
                        model.firma = ms.ToArray();
                    }

                    sql.Add(model);
                    await sql.SaveChangesAsync();
                    return Json(new { success = true });
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }

            return Json(new { success = false });
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

                // Actualizar campos
                sacerdote.nombre = model.nombre;
                sacerdote.apellido = model.apellido;
                sacerdote.cedula = model.cedula;
                sacerdote.telefono = model.telefono;
                sacerdote.email = model.email;
                sacerdote.estado = model.estado;
                sacerdote.observaciones = model.observaciones;

                // Actualizar firma si viene una nueva
                if (firma != null && firma.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await firma.CopyToAsync(ms);
                    model.firma = ms.ToArray();
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