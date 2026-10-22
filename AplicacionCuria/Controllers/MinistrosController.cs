using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    public class MinistrosController(ConexionPostgreSQL sql) : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Create(Ministro model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.fechaordenacion.HasValue)
                        model.fechaordenacion = DateTime.SpecifyKind(model.fechaordenacion.Value, DateTimeKind.Utc);

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
        public IActionResult Edit(int id, Ministro model)
        {
            if (id != model.id)
            {
                return NotFound();
            }
            try
            {
                if (model.fechaordenacion.HasValue)
                    model.fechaordenacion = DateTime.SpecifyKind(model.fechaordenacion.Value, DateTimeKind.Utc);

                sql.Update(model);
                sql.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MinistroExists(model.id))
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
            var ministro = await sql.ministros.FindAsync(id);
            if (ministro == null)
            {
                return NotFound();
            }
            sql.ministros.Remove(ministro);
            sql.SaveChanges();
            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult GetMinistros()
        {
            var ministros = sql.ministros.Select(m => new
            {
                m.id,
                m.nombre,
                m.apellido,
                m.email,
                m.telefono,
                m.fechaordenacion,
                m.estado,
                m.observaciones
            }).ToList();

            return Json(ministros);
        }

        [HttpGet]
        public IActionResult SearchByName(string nombre)
        {
            var resultados = sql.ministros
                .Where(m => (m.nombre + " " + m.apellido).ToLower().Contains(nombre.ToLower()))
                .ToList();

            return Json(resultados);
        }

        private bool MinistroExists(int id)
        {
            return sql.ministros.Any(e => e.id == id);
        }
    }
}