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
                var faltantes = new List<string>();
                if (string.IsNullOrWhiteSpace(model.nombre)) faltantes.Add("Nombre");
                if (string.IsNullOrWhiteSpace(model.telefono)) faltantes.Add("Teléfono");
                if (string.IsNullOrWhiteSpace(model.email)) faltantes.Add("Correo electrónico");
                if (model.sacerdote_id == null || model.sacerdote_id <= 0)
                    faltantes.Add("Sacerdote (debe registrar y seleccionar un sacerdote primero)");
                if (string.IsNullOrWhiteSpace(model.direccion)) faltantes.Add("Dirección");
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

                model.observaciones ??= "";
                sql.Add(model);
                await sql.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "Error al guardar parroquia: " + e.Message });
            }
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
                var faltantes = new List<string>();
                if (string.IsNullOrWhiteSpace(model.nombre)) faltantes.Add("Nombre");
                if (string.IsNullOrWhiteSpace(model.telefono)) faltantes.Add("Teléfono");
                if (string.IsNullOrWhiteSpace(model.email)) faltantes.Add("Correo electrónico");
                if (model.sacerdote_id == null || model.sacerdote_id <= 0)
                    faltantes.Add("Sacerdote (debe seleccionar un sacerdote)");
                if (string.IsNullOrWhiteSpace(model.direccion)) faltantes.Add("Dirección");
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

                model.observaciones ??= "";
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
            catch (Exception e)
            {
                return Json(new { success = false, message = "Error al actualizar parroquia: " + e.Message });
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
                                  p.observaciones,
                                  p.logo_base64
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
                                  p.observaciones,
                                  p.logo_base64
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