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
                var faltantes = new List<string>();
                if (string.IsNullOrWhiteSpace(model.nombre)) faltantes.Add("Nombre");
                if (string.IsNullOrWhiteSpace(model.dia_semana)) faltantes.Add("Día de la semana");
                if (model.iglesia_id == null || model.iglesia_id <= 0)
                    faltantes.Add("Iglesia (debe registrar y seleccionar una iglesia primero)");
                if (model.sacerdote_id == null || model.sacerdote_id <= 0)
                    faltantes.Add("Sacerdote (debe registrar y seleccionar un sacerdote primero)");
                if (string.IsNullOrWhiteSpace(model.estado)) faltantes.Add("Estado");

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

                model.descripcion ??= "";
                sql.Add(model);
                await sql.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "Error al guardar misa: " + e.Message });
            }
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
            var filtro = (nombre ?? "").Trim().ToLower();
            var resultados = (from m in sql.misas
                              join i in sql.iglesias on m.iglesia_id equals i.id into ij
                              from i in ij.DefaultIfEmpty()
                              join s in sql.sacerdotes on m.sacerdote_id equals s.id into sj
                              from s in sj.DefaultIfEmpty()
                              where m.nombre.ToLower().Contains(filtro)
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