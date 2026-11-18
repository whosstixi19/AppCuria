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
                var faltantes = new List<string>();
                if (string.IsNullOrWhiteSpace(model.nombre)) faltantes.Add("Nombre");
                if (string.IsNullOrWhiteSpace(model.ciudad)) faltantes.Add("Ciudad");
                if (string.IsNullOrWhiteSpace(model.direccion)) faltantes.Add("Dirección");
                if (string.IsNullOrWhiteSpace(model.telefono)) faltantes.Add("Teléfono");
                if (string.IsNullOrWhiteSpace(model.email)) faltantes.Add("Correo electrónico");
                if (model.ministro_id == null || model.ministro_id <= 0)
                    faltantes.Add("Ministro (debe registrar y seleccionar un ministro primero)");
                if (model.parroquia_id == null || model.parroquia_id <= 0)
                    faltantes.Add("Parroquia (debe registrar y seleccionar una parroquia primero)");
                if (string.IsNullOrWhiteSpace(model.denominacion)) faltantes.Add("Denominación");
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
                return Json(new { success = false, message = "Error al guardar iglesia: " + e.Message });
            }
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