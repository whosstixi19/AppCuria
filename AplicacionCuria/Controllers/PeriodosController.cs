using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    public class PeriodosController : Controller
    {
        private readonly ConexionPostgreSQL sql;

        public PeriodosController(ConexionPostgreSQL sql)
        {
            this.sql = sql;
        }

        #region CRUD

        [HttpPost]
        public async Task<IActionResult> Create(Periodo model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }
            try
            {
                model.id = 0;
                model.estado = "ACTIVO";
                model.fecha_inicio = DateTime.SpecifyKind(model.fecha_inicio, DateTimeKind.Utc);
                model.fecha_fin = DateTime.SpecifyKind(model.fecha_fin, DateTimeKind.Utc);

                await sql.periodos.AddAsync(model);
                await sql.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = new[] { ex.Message } });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Periodo model)
        {
            if (id != model.id)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    model.fecha_inicio = DateTime.SpecifyKind(model.fecha_inicio, DateTimeKind.Utc);
                    model.fecha_fin = DateTime.SpecifyKind(model.fecha_fin, DateTimeKind.Utc);

                    sql.Update(model);
                    await sql.SaveChangesAsync();
                    return Json(new { success = true });
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PeriodoExists(model.id))
                {
                    return NotFound();
                }
                return Json(new { success = false });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = new[] { ex.Message } });
            }

            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var periodo = await sql.periodos.FindAsync(id);
            if (periodo == null)
            {
                return NotFound();
            }
            sql.periodos.Remove(periodo);
            await sql.SaveChangesAsync();
            return Json(new { success = true });
        }

        private bool PeriodoExists(int id)
        {
            return sql.periodos.Any(e => e.id == id);
        }

        #endregion

        #region Busquedas

        [HttpGet]
        public async Task<IActionResult> Search()
        {
            try
            {
                var resultados = await sql.periodos.ToListAsync();
                return Json(resultados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message + (ex.InnerException != null ? " - " + ex.InnerException.Message : ""));
            }
        }

        [HttpGet]
        public IActionResult SearchByName(string nombre)
        {
            var resultados = sql.periodos
                .Where(p => p.nombre.ToLower().Contains(nombre.ToLower()))
                .ToList();
            return Json(resultados);
        }

        #endregion

        #region AbrirPestanas

        [HttpGet]
        public async Task<IActionResult> ModifyPeriodoView(int? id)
        {
            if (id == null || sql.periodos == null)
            {
                return NotFound();
            }

            var periodo = await sql.periodos.FindAsync(id);
            if (periodo == null)
            {
                return NotFound();
            }
            return Json(periodo);
        }

        #endregion
    }
}