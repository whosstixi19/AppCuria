using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    public class PrimeraComunionController(ConexionPostgreSQL sql) : Controller
    {

        #region CRUD

        [HttpPost]
        public async Task<IActionResult> Create(PrimeraComunion model)
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

                model.fechaprimeracomunion = DateTime.SpecifyKind(model.fechaprimeracomunion, DateTimeKind.Unspecified);
                model.fechaprimeracomunion = TimeZoneInfo.ConvertTimeToUtc(model.fechaprimeracomunion, zonaEcuador);

                var sacerdote = sql.sacerdotes.Find(model.sacerdote_id);
                var parroquia = sql.parroquias.Find(model.parroquia_id);

                if (sacerdote == null || parroquia == null)
                {
                    return Json(new { success = false, errors = "Sacerdote o Parroquia no encontrados." });
                }

                Console.WriteLine($"Antes de asignar: {model.alumno_id}");
                var alumno = sql.alumnoscatequesis.FirstOrDefault(a => a.cedula == model.cedula);
                if (alumno == null)
                {
                    return Json(new { success = false, errors = "No se encontró un alumno con esa cédula. Verifica si está registrado." });
                }
                Console.WriteLine($"Cédula enviada: {model.cedula}");
                Console.WriteLine($"Alumno encontrado: {(alumno != null ? alumno.cedula : "NO ENCONTRADO")}");

                model.alumno_id = alumno.id;
                Console.WriteLine($"Después de asignar: {model.alumno_id}");
                model.sacerdote_id = sacerdote.id;
                model.parroquia_id = parroquia.id;

                sql.Add(model);
                await sql.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = "Ocurrió un error al crear el registro.", exception = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Edit(int id, PrimeraComunion model)
        {
            if (id != model.id)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }
            try
            {
                var alumno = sql.alumnoscatequesis.FirstOrDefault(a => a.cedula == model.cedula);
                if (alumno == null)
                {
                    return Json(new { success = false, errors = "No se encontró un alumno con esa cédula. Verifica si está registrado." });
                }
                model.alumno_id = alumno.id;

                var zonaEcuador = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

                model.fechanacimiento = DateTime.SpecifyKind(model.fechanacimiento, DateTimeKind.Unspecified);
                model.fechanacimiento = TimeZoneInfo.ConvertTimeToUtc(model.fechanacimiento, zonaEcuador);

                model.fechaprimeracomunion = DateTime.SpecifyKind(model.fechaprimeracomunion, DateTimeKind.Unspecified);
                model.fechaprimeracomunion = TimeZoneInfo.ConvertTimeToUtc(model.fechaprimeracomunion, zonaEcuador);

                sql.Update(model);
                sql.SaveChanges();
                return Json(new { success = true });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrimeraComunionExists(model.id))
                {
                    return NotFound();
                }
                throw;
            }
        }

        private bool PrimeraComunionExists(int id)
        {
            return sql.primerascomuniones.Any(e => e.id == id);
        }

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await sql.primerascomuniones.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            sql.primerascomuniones.Remove(usuario);
            await sql.SaveChangesAsync();
            return Json(new { success = true });
        }
        #endregion
        #region abrirPestanas

        public async Task<IActionResult> ModifyPrimeraComunionView(int? id)
        {
            if (id == null || sql.primerascomuniones == null)
            {
                return NotFound();
            }

            var usuario = await sql.primerascomuniones.FindAsync(id);
            ViewBag.Parroquias = await sql.parroquias.ToListAsync();
            ViewBag.Sacerdotes = await sql.sacerdotes.ToListAsync();


            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        public async Task<IActionResult> DeletePrimeraComunionView(int? id)
        {
            if (id == null || sql.primerascomuniones == null)
            {
                return NotFound();
            }

            var usuario = await sql.primerascomuniones.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            // buscar nombres usando los IDs
            var parroquia = await sql.parroquias.FindAsync(usuario.parroquia_id);
            var sacerdote = await sql.sacerdotes.FindAsync(usuario.sacerdote_id);

            ViewBag.ParroquiaNombre = parroquia?.nombre ?? "";
            ViewBag.SacerdoteNombre = sacerdote?.nombre ?? "";

            return View(usuario);
        }

        public async Task<IActionResult> Valores(int id)
        {
            var usuario = await sql.primerascomuniones.FindAsync(id);
            return Json(usuario);
        }

        #endregion

        #region listados

        [HttpGet]
        public IActionResult GetParroquias()
        {
            var parroquias = sql.parroquias.Select(p => new
            {
                p.id,
                p.nombre
            }).ToList();

            return Json(parroquias);
        }

        [HttpGet]
        public IActionResult GetMinistros()
        {
            var ministros = sql.ministros.Select(m => new
            {
                m.id,
                m.nombre
            }).ToList();

            return Json(ministros);
        }
        [HttpGet]
        public IActionResult GetSacerdotes()
        {
            var sacerdotes = sql.sacerdotes.Select(s => new
            {
                s.id,
                s.nombre
            }).ToList();

            return Json(sacerdotes);
        }

        #endregion
        #region busquedas

        [HttpGet]
        public async Task<IActionResult> Search()
        {
            var resultados = await sql.primerascomuniones.ToListAsync();
            return Json(resultados);
        }

        [HttpGet]
        public IActionResult SearchByName(string nombrecompleto)
        {
            var resultados = (
                from c in sql.primerascomuniones
                join p in sql.parroquias on c.parroquia_id equals p.id into parroquiasJoin
                from p in parroquiasJoin.DefaultIfEmpty()
                where c.nombre.ToLower().Contains(nombrecompleto.ToLower()) || c.apellido.ToLower().Contains(nombrecompleto.ToLower())
                select new
                {
                    c.id,
                    c.nombre,
                    c.apellido,
                    c.telefono,
                    fechanacimiento = c.fechanacimiento,
                    parroquia = p != null ? p.nombre : "Sin asignar"
                }).ToList();

            return Json(resultados);
        }
        
        [HttpGet]
        public IActionResult GetListado()
        {
            var result = sql.primerascomuniones
                .OrderByDescending(p => p.id)
                .Take(8) 
                .Select(p => new {
                    p.id,
                    p.nombre,
                    p.apellido,
                    p.telefono,
                    p.fechanacimiento, 
                    parroquia = sql.parroquias
                        .Where(pa => pa.id == p.parroquia_id)
                        .Select(pa => pa.nombre)
                        .FirstOrDefault()
                }).ToList();

            return Json(result);
        }

        #endregion
    }
}