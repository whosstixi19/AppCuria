using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    public class AntiguosController(ConexionPostgreSQL sql) : Controller
    {

        [HttpPost]
        public async Task<IActionResult> Create(Antiguos model)
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

                model.fechabautizo = DateTime.SpecifyKind(model.fechabautizo, DateTimeKind.Unspecified);
                model.fechabautizo = TimeZoneInfo.ConvertTimeToUtc(model.fechabautizo, zonaEcuador);

                model.fechanacimiento = DateTime.SpecifyKind(model.fechanacimiento, DateTimeKind.Unspecified);
                model.fechanacimiento = TimeZoneInfo.ConvertTimeToUtc(model.fechanacimiento, zonaEcuador);

                model.registrocivilfecha = DateTime.SpecifyKind(model.registrocivilfecha, DateTimeKind.Unspecified);
                model.registrocivilfecha = TimeZoneInfo.ConvertTimeToUtc(model.registrocivilfecha, zonaEcuador);

                var ministro = await sql.ministros.FindAsync(model.ministro_id);
                if (ministro == null)
                {
                    return Json(new { success = false, errors = "Ministro no encontrado." });
                }

                var sacerdote = await sql.sacerdotes.FindAsync(model.sacerdote_id);
                if (sacerdote == null)
                {
                    return Json(new { success = false, errors = "Sacerdote no encontrado." });
                }

                var iglesia = await sql.iglesias.FindAsync(model.iglesia_id);
                if (iglesia == null)
                {
                    return Json(new { success = false, errors = "Iglesia no encontrada." });
                }

                var parroquia = await sql.parroquias.FindAsync(model.parroquia_id);
                if (parroquia == null)
                {
                    return Json(new { success = false, errors = "Parroquia no encontrada." });
                }

                sql.bautismosantiguos.Add(model);
                await sql.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    errors = $"Ocurrió un error al crear el registro. {ex.Message}",
                    exception = ex.ToString()
                });
            }
        }


        [HttpPost]
        public IActionResult Edit(int id, Antiguos model)
        {
            if (id != model.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var zonaEcuador = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

                    model.fechabautizo = DateTime.SpecifyKind(model.fechabautizo, DateTimeKind.Unspecified);
                    model.fechabautizo = TimeZoneInfo.ConvertTimeToUtc(model.fechabautizo, zonaEcuador);

                    model.fechanacimiento = DateTime.SpecifyKind(model.fechanacimiento, DateTimeKind.Unspecified);
                    model.fechanacimiento = TimeZoneInfo.ConvertTimeToUtc(model.fechanacimiento, zonaEcuador);

                    model.registrocivilfecha = DateTime.SpecifyKind(model.registrocivilfecha, DateTimeKind.Unspecified);
                    model.registrocivilfecha = TimeZoneInfo.ConvertTimeToUtc(model.registrocivilfecha, zonaEcuador);

                    sql.bautismosantiguos.Update(model);
                    sql.SaveChanges();
                    return Json(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BautizadoExists(model.id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await sql.bautismosantiguos.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            sql.bautismosantiguos.Remove(usuario);
            await sql.SaveChangesAsync();
            return Json(new { success = true });
        }

        private bool BautizadoExists(int id)
        {
            return sql.bautismosantiguos.Any(e => e.id == id);
        }

        #region busquedas

        [HttpGet]
        public async Task<IActionResult> Search()
        {
            var resultados = await sql.bautismosantiguos.ToListAsync();
            return Json(resultados);
        }

        [HttpGet]
        public IActionResult SearchByName(string nombrecompleto)
        {
            var resultados = sql.bautismosantiguos
                .Where(a => a.nombrecompleto.ToLower().Contains(nombrecompleto.ToLower()))
                .Select(a => new
                {
                    a.id,
                    a.nombrecompleto,
                    a.fechabautizo,
                    iglesia = sql.iglesias.FirstOrDefault(i => i.id == a.iglesia_id).nombre
                })
                .ToList();

            return Json(resultados);
        }

        [HttpGet]
        public async Task<IActionResult> ListActasAntiguas()
        {
            var resultados = await sql.bautismosantiguos
                .OrderByDescending(a => a.id)
                .Take(8)
                .Select(a => new {
                    a.id,
                    a.nombrecompleto,
                    a.fechabautizo,
                    iglesia = sql.iglesias.FirstOrDefault(i => i.id == a.iglesia_id).nombre
                })
                .ToListAsync();

            return Json(resultados);
        }

        #endregion

        #region vistas

        public async Task<IActionResult> ModifyAntiguosView(int? id)
        {
            if (id == null || sql.bautismosantiguos == null)
            {
                return NotFound();
            }

            var usuario = await sql.bautismosantiguos.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            ViewBag.Ministros = sql.ministros.ToList();
            ViewBag.Iglesias = sql.iglesias.ToList();
            ViewBag.Parroquias = sql.parroquias.ToList();
            ViewBag.Sacerdotes = sql.sacerdotes.ToList();

            return View(usuario);
        }

        public async Task<IActionResult> DeleteAntiguosView(int? id)
        {
            if (id == null || sql.bautismosantiguos == null)
                return NotFound();

            var antiguo = await sql.bautismosantiguos.FindAsync(id);
            if (antiguo == null)
                return NotFound();

            // Buscar nombres usando los IDs
            var ministro = await sql.ministros.FindAsync(antiguo.ministro_id);
            var iglesia = await sql.iglesias.FindAsync(antiguo.iglesia_id);
            var parroquia = await sql.parroquias
                .Where(p => p.id == antiguo.parroquia_id)
                .Select(p => p.nombre)
                .FirstOrDefaultAsync();

            var sacerdote = await sql.sacerdotes
                .Where(s => s.id == antiguo.sacerdote_id)
                .Select(s => s.nombre)
                .FirstOrDefaultAsync();

            ViewBag.MinistroNombre = ministro?.nombre ?? "";
            ViewBag.IglesiaNombre = iglesia?.nombre ?? "";
            ViewBag.ParroquiaNombre = parroquia ?? "";
            ViewBag.SacerdoteNombre = sacerdote ?? "";


            return View(antiguo);
        }

        public async Task<IActionResult> Valores(int id)
        {
            var usuario = await sql.bautismosantiguos.FindAsync(id);
            return Json(usuario);
        }

        #endregion
    }
}