using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    public class CatequistaController : Controller
    {
        private readonly ConexionPostgreSQL sql;

        public CatequistaController(ConexionPostgreSQL sql)
        {
            this.sql = sql;
        }

        #region CRUD

        [HttpPost]
        public async Task<IActionResult> Create(Catequista model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }
            try
            {
                if (model.fecha_nacimiento.HasValue)
                    model.fecha_nacimiento = DateTime.SpecifyKind(model.fecha_nacimiento.Value, DateTimeKind.Utc);

                model.id = 0;
                model.estado = "ACTIVO";
                await sql.catequistas.AddAsync(model);
                await sql.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = new[] { ex.Message } });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Catequista model)
        {
            if (id != model.id)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    if (model.fecha_nacimiento.HasValue)
                        model.fecha_nacimiento = DateTime.SpecifyKind(model.fecha_nacimiento.Value, DateTimeKind.Utc);

                    sql.Update(model);
                    await sql.SaveChangesAsync();
                    return Json(new { success = true });
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CatequistaExists(model.id))
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
            var catequista = await sql.catequistas.FindAsync(id);
            if (catequista == null)
            {
                return NotFound();
            }
            sql.catequistas.Remove(catequista);
            await sql.SaveChangesAsync();
            return Json(new { success = true });
        }

        private bool CatequistaExists(int id)
        {
            return sql.catequistas.Any(e => e.id == id);
        }

        #endregion

        #region Busquedas

        [HttpGet]
        public async Task<IActionResult> Search()
        {
            try
            {
                var resultados = await sql.catequistas.ToListAsync();
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
            var resultados = sql.catequistas
                .Where(c => c.nombre.ToLower().Contains(nombre.ToLower()) ||
                            c.apellido.ToLower().Contains(nombre.ToLower()) ||
                            (c.cedula ?? "").ToLower().Contains(nombre.ToLower()))
                .ToList();
            return Json(resultados);
        }

        #endregion

        #region AbrirPestanas

        [HttpGet]
        public async Task<IActionResult> ModifyCatequistaView(int? id)
        {
            if (id == null || sql.catequistas == null)
            {
                return NotFound();
            }

            var catequista = await sql.catequistas.FindAsync(id);
            if (catequista == null)
            {
                return NotFound();
            }
            return Json(catequista);
        }

        #endregion
    }
}