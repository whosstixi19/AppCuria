using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    public class MisasController(ConexionPostgreSQL sql) : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Create(Misa model)
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
        public IActionResult Edit(int id, Misa model)
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
                if (!MisaExists(model.id))
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
            var misa = await sql.misas.FindAsync(id);
            if (misa == null)
            {
                return NotFound();
            }
            sql.misas.Remove(misa);
            sql.SaveChanges();
            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult GetMisas()
        {
            var misas = (from m in sql.misas
                         join i in sql.iglesias on m.iglesia_id equals i.id into ij
                         from i in ij.DefaultIfEmpty()
                         join s in sql.sacerdotes on m.sacerdote_id equals s.id into sj
                         from s in sj.DefaultIfEmpty()
                         select new
                         {
                             m.id,
                             m.nombre,
                             m.hora_inicio,
                             m.hora_fin,
                             m.dia_semana,
                             iglesia_id = m.iglesia_id,
                             iglesia_nombre = i != null ? i.nombre : "",
                             sacerdote_id = m.sacerdote_id,
                             sacerdote_nombre = s != null ? s.nombre + " " + s.apellido : "",
                             m.descripcion,
                             m.estado
                         }).ToList();

            return Json(misas);
        }

        [HttpGet]
        public IActionResult SearchByName(string nombre)
        {
            var resultados = sql.misas
                .Where(m => m.nombre.ToLower().Contains(nombre.ToLower()))
                .ToList();

            return Json(resultados);
        }

        [HttpGet]
        public IActionResult GetIglesiasDisponibles()
        {
            var iglesias = sql.iglesias
                .Select(i => new { i.id, i.nombre })
                .ToList();
            return Json(iglesias);
        }

        [HttpGet]
        public IActionResult GetSacerdotesDisponibles()
        {
            var sacerdotes = sql.sacerdotes
                .Select(s => new { s.id, nombre = s.nombre + " " + s.apellido })
                .ToList();
            return Json(sacerdotes);
        }

        private bool MisaExists(int id)
        {
            return sql.misas.Any(e => e.id == id);
        }
    }
}