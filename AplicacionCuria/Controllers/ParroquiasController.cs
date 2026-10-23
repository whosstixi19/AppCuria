using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    public class ParroquiasController(ConexionPostgreSQL sql) : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Create(Parroquia model)
        {
            try
            {
                if (ModelState.IsValid)
                {
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
        public IActionResult Edit(int id, Parroquia model)
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
                if (!ParroquiaExists(model.id))
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
            var parroquia = await sql.parroquias.FindAsync(id);
            if (parroquia == null)
            {
                return NotFound();
            }
            sql.parroquias.Remove(parroquia);
            sql.SaveChanges();
            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult GetParroquias()
        {
            var parroquias = (from p in sql.parroquias
                              join s in sql.sacerdotes on p.sacerdote_id equals s.id into sj
                              from s in sj.DefaultIfEmpty()
                              select new
                              {
                                  p.id,
                                  p.nombre,
                                  p.direccion,
                                  p.telefono,
                                  p.email,
                                  sacerdote_id = p.sacerdote_id,
                                  sacerdote_nombre = s != null ? s.nombre + " " + s.apellido : "",
                                  p.estado,
                                  p.observaciones
                              }).ToList();

            return Json(parroquias);
        }

        [HttpGet]
        public IActionResult SearchByName(string nombre)
        {
            var filtro = (nombre ?? "").Trim().ToLower();
            var resultados = (from p in sql.parroquias
                              join s in sql.sacerdotes on p.sacerdote_id equals s.id into sj
                              from s in sj.DefaultIfEmpty()
                              where p.nombre.ToLower().Contains(filtro)
                              select new
                              {
                                  p.id,
                                  p.nombre,
                                  p.direccion,
                                  p.telefono,
                                  p.email,
                                  sacerdote_id = p.sacerdote_id,
                                  sacerdote_nombre = s != null ? s.nombre + " " + s.apellido : "",
                                  p.estado,
                                  p.observaciones
                              }).ToList();

            return Json(resultados);
        }

        [HttpGet]
        public IActionResult GetSacerdotesDisponibles()
        {
            var sacerdotes = sql.sacerdotes
                .Select(s => new { s.id, nombre = s.nombre + " " + s.apellido })
                .ToList();
            return Json(sacerdotes);
        }

        private bool ParroquiaExists(int id)
        {
            return sql.parroquias.Any(e => e.id == id);
        }
    }
}