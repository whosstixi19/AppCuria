using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    [Route("Faltas")]
    public class FaltasController(ConexionPostgreSQL sql) : Controller
    {
        #region CRUD

        [HttpPost("Create")]
        public async Task<IActionResult> Create(Falta model)
        {
            try
            {
                model.id = 0;
                var zonaEcuador = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
                model.fechafalta = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(model.fechafalta, DateTimeKind.Unspecified), zonaEcuador);

                if (ModelState.IsValid)
                {
                    await sql.AddAsync(model);
                    await sql.SaveChangesAsync();
                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                // Log error si lo deseas
            }

            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
        }

       [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, Falta model)
        {
            if (id != model.id)
                return NotFound();

            try
            {
                if (ModelState.IsValid)
                {
                    var falta = await sql.faltas.FindAsync(id);
                    if (falta == null)
                        return NotFound();

                    // Convertir a UTC
                    var zonaEcuador = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
                    model.fechafalta = TimeZoneInfo.ConvertTimeToUtc(
                        DateTime.SpecifyKind(model.fechafalta, DateTimeKind.Unspecified),
                        zonaEcuador
                    );

                    falta.fechafalta = model.fechafalta;
                    falta.observacion = model.observacion;

                    await sql.SaveChangesAsync();

                    TempData["Success"] = "¡Falta modificada con éxito!";
                    return RedirectToAction("MenuCatequesis", "Home");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar la falta: " + ex.Message;
                return RedirectToAction("MenuCatequesis", "Home");
            }

            TempData["Error"] = "Error de validación.";
            return RedirectToAction("MenuCatequesis", "Home");
        }


        [HttpPost("DeleteConfirmed/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var falta = await sql.faltas.FindAsync(id);
            if (falta == null)
            {
                return Json(new { success = false });
            }
            sql.faltas.Remove(falta);
            await sql.SaveChangesAsync();
            return Json(new { success = true });
        }

        private bool FaltaExists(int id)
        {
            return sql.faltas.Any(e => e.id == id);
        }

        #endregion

        #region Busquedas

        [HttpGet]
        public async Task<IActionResult> Search()
        {
            var consulta = from a in sql.alumnoscatequesis
                           join f in sql.faltas on a.id equals f.alumno_id
                           select new
                           {
                               nombre = a.nombre + " " + a.apellido,
                               fechafalta = f.fechafalta,
                               observacion = f.observacion,
                               estado = f.estado,
                               id = f.id,
                               alumno_id = f.alumno_id
                           };

            var resultados = await consulta.ToListAsync();
            return Json(resultados);
        }

        [HttpGet]
        public IActionResult SearchByName(string nombrecompleto)
        {
            var resultados = sql.faltas
                .Where(f => f.observacion.ToLower().Contains(nombrecompleto.ToLower()))
                .ToList();
            return Json(resultados);
        }

        [HttpGet("BuscarPorNombre")]
        public IActionResult BuscarPorNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return BadRequest("Nombre inválido.");

            var resultado = (from f in sql.faltas
                            join a in sql.alumnoscatequesis on f.alumno_id equals a.id
                            where (a.nombre + " " + a.apellido).ToLower().Contains(nombre.ToLower())
                                && f.estado == "ACTIVO"
                            select new
                            {
                                id = f.id,
                                nombre = a.nombre + " " + a.apellido,
                                fechafalta = f.fechafalta.ToString("yyyy-MM-dd"),
                                observacion = f.observacion
                            }).ToList();

            return Json(resultado);
        }

        #endregion

        #region AbrirPestanas

        [HttpGet("ModifyFaltaAlumnosView/{id}")]
        public async Task<IActionResult> ModifyFaltaAlumnosView(int? id)
        {
            if (id == null || sql.faltas == null)
                return NotFound();

            var falta = await sql.faltas.FindAsync(id);
            if (falta == null)
                return NotFound();

            // Obtener nombre del alumno
            var alumno = await sql.alumnoscatequesis.FindAsync(falta.alumno_id);
            ViewBag.NombreAlumno = alumno != null ? $"{alumno.nombre} {alumno.apellido}" : "Alumno no encontrado";

            return View(falta);
        }

        public async Task<IActionResult> DeleteFaltaAlumnosView(int? id)
        {
            if (id == null || sql.faltas == null)
            {
                return NotFound();
            }

            var falta = await sql.faltas.FindAsync(id);
            if (falta == null)
            {
                return NotFound();
            }
            return View(falta);
        }

        #endregion
    }
}