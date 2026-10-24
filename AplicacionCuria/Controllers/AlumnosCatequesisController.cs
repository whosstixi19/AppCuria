using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    public class AlumnosCatequesisController(ConexionPostgreSQL sql) : Controller
    {

        #region CRUD

        [HttpPost]
        public async Task<IActionResult> Create(AlumnoCatequesis model, IFormFile? foto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
                }

                model.id = 0;
                var zonaEcuador = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

                model.fechabautizo = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(model.fechabautizo, DateTimeKind.Unspecified), zonaEcuador);
                model.fechanacimiento = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(model.fechanacimiento, DateTimeKind.Unspecified), zonaEcuador);

                // Solo procesamos la foto si no es nula y tiene contenido
                if (foto != null && foto.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await foto.CopyToAsync(ms);
                    model.foto = ms.ToArray();
                }

                await sql.AddAsync(model);
                await sql.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = $"Ocurrió un error al crear el registro. {ex.Message}", exception = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, AlumnoCatequesis model, IFormFile foto)
        {
            if (id != model.id)
            {
                return NotFound();
            }

            try
            {
                var zonaEcuador = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

                model.fechabautizo = TimeZoneInfo.ConvertTimeToUtc(
                    DateTime.SpecifyKind(model.fechabautizo, DateTimeKind.Unspecified), zonaEcuador);

                model.fechanacimiento = TimeZoneInfo.ConvertTimeToUtc(
                    DateTime.SpecifyKind(model.fechanacimiento, DateTimeKind.Unspecified), zonaEcuador);

                if (foto != null && foto.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await foto.CopyToAsync(ms);
                        model.foto = ms.ToArray();
                    }
                }
                else
                {
                    // Conservar la foto actual si no se subió una nueva
                    var alumnoExistente = await sql.alumnoscatequesis.AsNoTracking()
                        .FirstOrDefaultAsync(a => a.id == model.id);
                    if (alumnoExistente != null)
                    {
                        model.foto = alumnoExistente.foto;
                    }
                }

                sql.Update(model);
                await sql.SaveChangesAsync();

                TempData["SuccessMessage"] = "Alumno actualizado correctamente.";

                //  Redirige a MenuCatequesis
                return RedirectToAction("MenuCatequesis", "Home");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BautizadoExists(model.id))
                {
                    return NotFound();
                }

                TempData["ErrorMessage"] = "Error al actualizar el alumno.";
                return RedirectToAction("ModifyAlumnosView", new { id = model.id });
            }
        }

        private bool BautizadoExists(int id)
        {
            return sql.alumnoscatequesis.Any(e => e.id == id);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await sql.alumnoscatequesis.FindAsync(id);
            if (usuario == null)
            {
                TempData["ErrorMessage"] = "El alumno no fue encontrado.";
                return RedirectToAction("MenuCatequesis", "Home");
            }

            sql.alumnoscatequesis.Remove(usuario);
            await sql.SaveChangesAsync();

            TempData["SuccessMessage"] = "Alumno eliminado correctamente.";
            return RedirectToAction("MenuCatequesis", "Home");
        }

        private bool AlumnoCatequesisExists(int id)
        {
            return sql.alumnoscatequesis.Any(e => e.id == id);
        }

        #endregion

        #region Busquedas

        [HttpGet]
        public async Task<IActionResult> Search()
        {
            var resultados = await sql.alumnoscatequesis.ToListAsync();
            return Json(resultados);
        }

        [HttpGet]
        public IActionResult SearchByName(string? nombrecompleto)
        {
            var filtro = (nombrecompleto ?? "").Trim().ToLower();
            var resultados = sql.alumnoscatequesis
                .Where(a => string.IsNullOrEmpty(filtro) ||
                            (a.nombre + " " + a.apellido).ToLower().Contains(filtro) ||
                            a.nombre.ToLower().Contains(filtro) ||
                            a.apellido.ToLower().Contains(filtro) ||
                            (a.cedula ?? "").ToLower().Contains(filtro))
                .ToList();

            return Json(resultados);
        }

        #endregion

        #region AbrirPestanas

        public async Task<IActionResult> ModifyAlumnosView(int? id)
        {
            if (id == null || sql.alumnoscatequesis == null)
            {
                return NotFound();
            }

            var usuario = await sql.alumnoscatequesis.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            //  Cargar lista de catequistas activos
            ViewBag.Catequistas = await sql.catequistas
                .Where(c => c.estado == "ACTIVO")
                .ToListAsync();

            //  Cargar lista de periodos disponibles
            ViewBag.Periodos = await sql.periodos
                .ToListAsync();

            Console.WriteLine($"Nivel: {usuario.nivel}, Catequista ID: {usuario.catequista_id}, Periodo ID: {usuario.periodo_id}");

            return View(usuario);
        }

        public async Task<IActionResult> DeleteAlumnosView(int? id)
        {
            if (id == null || sql.alumnoscatequesis == null)
            {
                return NotFound();
            }

            var usuario = await sql.alumnoscatequesis.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }
        [HttpGet]
        public IActionResult GetCatequistas()
        {
            var catequistas = sql.catequistas
                .Where(c => c.estado == "ACTIVO")
                .Select(c => new
                {
                    c.id,
                    c.nombre
                })
                .ToList();

            return Json(catequistas);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetPeriodos()
        {
            var periodos = await sql.periodos
                .Select(p => new { p.id, p.nombre }) // Ajusta el campo nombre si se llama distinto
                .ToListAsync();

            return Json(periodos);
        }
        #endregion
    }
}