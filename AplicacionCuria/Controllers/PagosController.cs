using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    public class PagosController(ConexionPostgreSQL sql) : Controller
    {

        #region CRUD

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Pago model, [FromForm] string Referencia_Id, [FromForm] string Referencia_Tabla)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Asignar campos adicionales
                    model.fechaPago = DateTime.SpecifyKind(model.fechaPago, DateTimeKind.Utc);
                    model.fecha_registro = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                    model.estado = "PAGADO";

                    // Convertir referencia_id (string a int)
                    if (int.TryParse(Referencia_Id, out int refId))
                    {
                        model.referencia_id = refId;
                    }
                    else
                    {
                        return Json(new { success = false, errors = new[] { "Referencia_Id inválido" } });
                    }

                    model.referencia_tabla = Referencia_Tabla ?? "";

                    // Guardar en BD
                    await sql.pagos.AddAsync(model);
                    await sql.SaveChangesAsync();

                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = new[] { "Error del servidor: " + ex.Message } });
            }

            // Retornar errores de validación
            return Json(new
            {
                success = false,
                errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });
        }

        [HttpPost]
        public IActionResult Edit(int id, Pago model)
        {
            if (id != model.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    model.fechaPago = DateTime.SpecifyKind(model.fechaPago, DateTimeKind.Utc);
                    sql.pagos.Update(model);
                    sql.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PagoExists(model.id))
                        return NotFound();
                    throw;
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pago = await sql.pagos.FindAsync(id);
            if (pago == null)
                return NotFound();

            sql.pagos.Remove(pago);
            await sql.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool PagoExists(int id)
        {
            return sql.pagos.Any(e => e.id == id);
        }

        #endregion

        #region Listados

        [HttpGet]
        public async Task<IActionResult> ListPagos()
        {
            var pagos = await sql.pagos.ToListAsync();
            return Json(pagos);
        }

        #endregion

        #region Busquedas

        [HttpGet]
        public async Task<IActionResult> Search()
        {
            var resultados = await sql.pagos.ToListAsync();
            return Json(resultados);
        }

        [HttpGet]
        public IActionResult SearchByNombre(string nombre)
        {
            var resultados = sql.pagos
                .Where(p => p.nombres.ToLower().Contains(nombre.ToLower()) || p.apellidos.ToLower().Contains(nombre.ToLower()))
                .ToList();

            return Json(resultados);
        }

        #endregion

        #region Referencias

        [HttpGet]
        public async Task<IActionResult> GetReferencias(string tipo_pago)
        {
            if (tipo_pago == "bautismo-celebracion")
                return Json(await sql.actasbautismo.Select(a => new { a.id, nombre = a.nombrecompleto }).ToListAsync());
            if (tipo_pago == "bautismo-cursillo")
                return Json(await sql.cursillosprematrimoniales.Select(c => new { c.id, nombre = c.nombrenovio + " y " + c.nombrenovia }).ToListAsync());
            if (tipo_pago == "matrimonio-celebracion")
                return Json(await sql.actasprematrimoniales.Select(a => new { a.id, nombre = a.nombrenovio + " y " + a.nombrenovia }).ToListAsync());
            if (tipo_pago == "matrimonio-cursillo")
                return Json(await sql.cursillosprematrimoniales.Select(c => new { c.id, nombre = c.nombrenovio + " y " + c.nombrenovia }).ToListAsync());

            return Json(new List<object>());
        }

        #endregion
    }
}
