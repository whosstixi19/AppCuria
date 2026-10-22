using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    public class ConfirmacionController(ConexionPostgreSQL sql) : Controller
    {
        #region CRUD

        [HttpPost]
        public async Task<IActionResult> Create(Confirmacion model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new
                    {
                        success = false,
                        errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var zonaEcuador = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

                model.fechanacimiento = DateTime.SpecifyKind(model.fechanacimiento, DateTimeKind.Unspecified);
                model.fechanacimiento = TimeZoneInfo.ConvertTimeToUtc(model.fechanacimiento, zonaEcuador);

                model.fechaconfirmacion = DateTime.SpecifyKind(model.fechaconfirmacion, DateTimeKind.Unspecified);
                model.fechaconfirmacion = TimeZoneInfo.ConvertTimeToUtc(model.fechaconfirmacion, zonaEcuador);


                var alumno = sql.alumnoscatequesis.FirstOrDefault(a => a.cedula == model.cedula);
                if (alumno == null)
                {
                    return Json(new
                    {
                        success = false,
                        errors = "No se encontró un alumno con esa cédula. Verifica si está registrado."
                    });
                }

                model.alumno_id = alumno.id;
                await sql.confirmaciones.AddAsync(model);
                await sql.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = "Ocurrió un error al guardar la confirmación.", exception = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Edit(int id, Confirmacion model)
        {
            if (id != model.id)
            {
                return NotFound();
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new
                    {
                        success = false,
                        errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var zonaEcuador = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

                model.fechanacimiento = DateTime.SpecifyKind(model.fechanacimiento, DateTimeKind.Unspecified);
                model.fechanacimiento = TimeZoneInfo.ConvertTimeToUtc(model.fechanacimiento, zonaEcuador);

                model.fechaconfirmacion = DateTime.SpecifyKind(model.fechaconfirmacion, DateTimeKind.Unspecified);
                model.fechaconfirmacion = TimeZoneInfo.ConvertTimeToUtc(model.fechaconfirmacion, zonaEcuador);

                // Recuperar alumno por cédula para asegurar integridad referencial
                var alumno = sql.alumnoscatequesis.FirstOrDefault(a => a.cedula == model.cedula);
                if (alumno == null)
                {
                    return Json(new
                    {
                        success = false,
                        errors = "No se encontró un alumno con esa cédula. Verifica si está registrado."
                    });
                }
                model.alumno_id = alumno.id;

                sql.Entry(model).State = EntityState.Modified;
                sql.SaveChanges();

                return Json(new { success = true });
            }
            catch (DbUpdateException ex)
            {
                return Json(new
                {
                    success = false,
                    errors = "Error al guardar en base de datos.",
                    exception = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        private bool ConfirmacionExists(int id)
        {
            return sql.confirmaciones.Any(e => e.id == id);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await sql.confirmaciones.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            sql.confirmaciones.Remove(usuario);
            await sql.SaveChangesAsync();
            return Json(new { success = true });
        }

        #endregion

        #region abrirPestanas

        public async Task<IActionResult> DeleteConfirmacionView(int? id)
        {
            if (id == null || sql.confirmaciones == null)
            {
                return NotFound();
            }

            var usuario = await sql.confirmaciones.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            var parroquia = await sql.parroquias.FindAsync(usuario.parroquia_id);
            var sacerdote = await sql.sacerdotes.FindAsync(usuario.sacerdote_id);

            ViewBag.ParroquiaNombre = parroquia?.nombre ?? "";
            ViewBag.SacerdoteNombre = sacerdote?.nombre ?? "";

            return View(usuario);
        }

        public async Task<IActionResult> ModifyConfirmacionView(int id)
        {
            var model = await sql.confirmaciones.FindAsync(id);
            if (model == null)
                return NotFound();

            ViewBag.Parroquias = await sql.parroquias.ToListAsync();
            ViewBag.Sacerdotes = await sql.sacerdotes.ToListAsync();

            return View("ModifyConfirmacionView", model);
        }

        #endregion

        #region listados

        [HttpGet]
        public IActionResult GetParroquias()
        {
            var parroquias = sql.parroquias.Select(p => new { p.id, p.nombre }).ToList();
            return Json(parroquias);
        }

        [HttpGet]
        public IActionResult GetSacerdotes()
        {
            var sacerdotes = sql.sacerdotes.Select(s => new { s.id, s.nombre }).ToList();
            return Json(sacerdotes);
        }

        [HttpGet]
        public IActionResult GetListado(string? nombrecompleto)
        {
            try
            {
                var data = (
                    from c in sql.confirmaciones
                    join p in sql.parroquias on c.parroquia_id equals p.id into parroquiasJoin
                    from p in parroquiasJoin.DefaultIfEmpty()
                    join s in sql.sacerdotes on c.sacerdote_id equals s.id into sacerdotesJoin
                    from s in sacerdotesJoin.DefaultIfEmpty()
                    select new
                    {
                        c.id,
                        c.nombre,
                        c.apellido,
                        c.telefono,
                        fechanacimiento = c.fechanacimiento.ToString("dd/MM/yyyy"),
                        parroquia = p != null ? p.nombre : "Sin parroquia",
                        sacerdote = s != null ? s.nombre : "Sin sacerdote"
                    }
                );

                if (!string.IsNullOrWhiteSpace(nombrecompleto))
                {
                    nombrecompleto = nombrecompleto.ToLower();
                    data = data.Where(x =>
                        (!string.IsNullOrEmpty(x.nombre) && x.nombre.ToLower().Contains(nombrecompleto)) ||
                        (!string.IsNullOrEmpty(x.apellido) && x.apellido.ToLower().Contains(nombrecompleto))
                    );
                }
                else
                {
                    data = data.OrderByDescending(x => x.id).Take(8); // Solo los 8 últimos si no hay búsqueda
                }

                return Json(data.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = "Error al obtener listado",
                    detail = ex.Message
                });
            }
        }

        #endregion

        #region busquedas

        [HttpGet]
        public async Task<IActionResult> Search()
        {
            var resultados = await sql.confirmaciones.ToListAsync();
            return Json(resultados);
        }

        [HttpGet]
        public IActionResult SearchByName(string nombrecompleto)
        {
            var resultados = sql.confirmaciones
                .Where(a => a.nombre.ToLower().Contains(nombrecompleto.ToLower()) || a.apellido.ToLower().Contains(nombrecompleto.ToLower()))
                .ToList();

            return Json(resultados);
        }

        #endregion
    }
}
