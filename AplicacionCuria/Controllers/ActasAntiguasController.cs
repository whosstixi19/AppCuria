using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    public class ActasAntiguasController(ConexionPostgreSQL sql) : Controller
    {

        #region CRUD
        [HttpGet]
        public IActionResult GetMinistros()
        {
            var lista = sql.ministros.ToList();
            return Json(lista);
        }

        [HttpGet]
        public IActionResult GetIglesias()
        {
            var lista = sql.iglesias.ToList();
            return Json(lista);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MatrimoniosAntiguos model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.id = 0;
                    var zonaEcuador = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

                    model.fechamatrimonio = TimeZoneInfo.ConvertTimeToUtc(model.fechamatrimonio, zonaEcuador);
                    model.fechabautizonovia = TimeZoneInfo.ConvertTimeToUtc(model.fechabautizonovia, zonaEcuador);
                    model.fechabautizonovio = TimeZoneInfo.ConvertTimeToUtc(model.fechabautizonovio, zonaEcuador);
                    model.fecharegistro = TimeZoneInfo.ConvertTimeToUtc(model.fecharegistro, zonaEcuador);
                    model.fecharegistrocivil = TimeZoneInfo.ConvertTimeToUtc(model.fecharegistrocivil, zonaEcuador);
                    model.fechanacimientonovia = TimeZoneInfo.ConvertTimeToUtc(model.fechanacimientonovia, zonaEcuador);
                    model.fechanacimientonovio = TimeZoneInfo.ConvertTimeToUtc(model.fechanacimientonovio, zonaEcuador);
                    // model.estado = model.estados.ToString();
                    await sql.AddAsync(model);
                    await sql.SaveChangesAsync();
                    //return RedirectToAction("BautizadosView", "Submenus");
                    return Json(new { success = true });
                }
                return Json(new
                {
                    success = false,
                    errors = ModelState.Values
                                    .SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage)
                                    .ToList()
                });
            }
            catch (Exception e)
            {
                return Json(new { success = false, errors = $"Ocurrió un error al crear el registro. {e.Message}", exception = e.Message });
            }
        }
        
        [HttpPost]
        public IActionResult Edit(int id, MatrimoniosAntiguos model)
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

                    model.fechamatrimonio = TimeZoneInfo.ConvertTimeToUtc(model.fechamatrimonio, zonaEcuador);
                    model.fechabautizonovia = TimeZoneInfo.ConvertTimeToUtc(model.fechabautizonovia, zonaEcuador);
                    model.fechabautizonovio = TimeZoneInfo.ConvertTimeToUtc(model.fechabautizonovio, zonaEcuador);
                    model.fecharegistro = TimeZoneInfo.ConvertTimeToUtc(model.fecharegistro, zonaEcuador);
                    model.fecharegistrocivil = TimeZoneInfo.ConvertTimeToUtc(model.fecharegistrocivil, zonaEcuador);
                    model.fechanacimientonovia = TimeZoneInfo.ConvertTimeToUtc(model.fechanacimientonovia, zonaEcuador);
                    model.fechanacimientonovio = TimeZoneInfo.ConvertTimeToUtc(model.fechanacimientonovio, zonaEcuador);

                    sql.Entry(model).State = EntityState.Modified;
                    sql.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BautizadoExists(model.id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return Json(new { success = true, message = "Matrimonio antiguo modificado correctamente." });
            }
            return Json(new { success = false, message = "Error al validar el formulario." });
        }

        private bool BautizadoExists(int id)
        {
            return sql.matrimoniosantiguos.Any(e => e.id == id);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await sql.matrimoniosantiguos.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            else
            {
                sql.matrimoniosantiguos.Remove(usuario);
                sql.SaveChanges();
            }
            return View("~/Views/Submenus/MatrimoniosView.cshtml");
        }
        #endregion

        #region busquedas
        [HttpGet]
        public async Task<IActionResult> Search()
        {
            var resultados = await sql.matrimoniosantiguos.ToListAsync();
            if (resultados.Count > 0)
                return Json(resultados); // Envía los resultados a la vista

            return Json(resultados);
        }

        [HttpGet]
        public IActionResult SearchByNames(string nombrecompleto)
        {
            if (string.IsNullOrWhiteSpace(nombrecompleto))
                return Json(new List<object>());

            var resultados = sql.matrimoniosantiguos
                .Where(a =>
                    a.nombrenovio.ToLower().Contains(nombrecompleto.ToLower()) ||
                    a.nombrenovia.ToLower().Contains(nombrecompleto.ToLower()))
                .ToList();

            return Json(resultados);
        }
        #endregion

        #region abrirPestanas

        public async Task<IActionResult> ModifyActasAntiguasView(int? id)
        {
            if (id == null || sql.matrimoniosantiguos == null)
            {
                return NotFound();
            }

            var usuario = await sql.matrimoniosantiguos.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }


        public async Task<IActionResult> DeleteActasAntiguasView(int? id)
        {
            if (id == null)
                return NotFound();

            var acta = await sql.matrimoniosantiguos.FindAsync(id);
            if (acta == null)
                return NotFound();

            // Cargar nombres de ministro e iglesia sin usar propiedades de navegación
            var ministroNombre = await sql.ministros
                .Where(m => m.id == acta.ministro_id)
                .Select(m => m.nombre)
                .FirstOrDefaultAsync();

            var iglesiaNombre = await sql.iglesias
                .Where(i => i.id == acta.iglesia_id)
                .Select(i => i.nombre)
                .FirstOrDefaultAsync();

            ViewBag.MinistroNombre = ministroNombre ?? "No registrado";
            ViewBag.IglesiaNombre = iglesiaNombre ?? "No registrada";

            return View(acta);
        }


        #endregion

    }
}
