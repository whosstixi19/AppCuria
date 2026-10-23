using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    public class IglesiaController(ConexionPostgreSQL sql) : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Create(Iglesia model)
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
        public IActionResult Edit(int id, Iglesia model)
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
                if (!IglesiaExists(model.id))
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
            var iglesia = await sql.iglesias.FindAsync(id);
            if (iglesia == null)
            {
                return NotFound();
            }
            sql.iglesias.Remove(iglesia);
            sql.SaveChanges();
            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult GetIglesias()
        {
            var iglesias = (from i in sql.iglesias
                            join m in sql.ministros on i.ministro_id equals m.id into mj
                            from m in mj.DefaultIfEmpty()
                            join p in sql.parroquias on i.parroquia_id equals p.id into pj
                            from p in pj.DefaultIfEmpty()
                            select new
                            {
                                i.id,
                                i.nombre,
                                i.direccion,
                                i.ciudad,
                                i.telefono,
                                i.email,
                                ministro_id = i.ministro_id,
                                ministro_nombre = m != null ? m.nombre + " " + m.apellido : "",
                                parroquia_id = i.parroquia_id,
                                parroquia_nombre = p != null ? p.nombre : "",
                                i.denominacion,
                                i.estado,
                                i.observaciones
                            }).ToList();

            return Json(iglesias);
        }

        [HttpGet]
        public IActionResult SearchByName(string nombre)
        {
            var filtro = (nombre ?? "").Trim().ToLower();
            var resultados = (from i in sql.iglesias
                              join m in sql.ministros on i.ministro_id equals m.id into mj
                              from m in mj.DefaultIfEmpty()
                              join p in sql.parroquias on i.parroquia_id equals p.id into pj
                              from p in pj.DefaultIfEmpty()
                              where i.nombre.ToLower().Contains(filtro)
                              select new
                              {
                                  i.id,
                                  i.nombre,
                                  i.direccion,
                                  i.ciudad,
                                  i.telefono,
                                  i.email,
                                  ministro_id = i.ministro_id,
                                  ministro_nombre = m != null ? m.nombre + " " + m.apellido : "",
                                  parroquia_id = i.parroquia_id,
                                  parroquia_nombre = p != null ? p.nombre : "",
                                  i.denominacion,
                                  i.estado,
                                  i.observaciones
                              }).ToList();

            return Json(resultados);
        }

        [HttpGet]
        public IActionResult GetMinistrosDisponibles()
        {
            var ministros = sql.ministros
                .Select(m => new { m.id, nombre = m.nombre + " " + m.apellido })
                .ToList();
            return Json(ministros);
        }

        [HttpGet]
        public IActionResult GetParroquiasDisponibles()
        {
            var parroquias = sql.parroquias
                .Select(p => new { p.id, p.nombre })
                .ToList();
            return Json(parroquias);
        }

        private bool IglesiaExists(int id)
        {
            return sql.iglesias.Any(e => e.id == id);
        }
    }
}