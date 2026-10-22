using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    public class ActaPrematrimonialController(ConexionPostgreSQL sql) : Controller
    {
        #region CRUD

        [HttpPost]
        public async Task<IActionResult> Create(ActaPreMatrimonial model)
        {
            try
            {
                //Agregar validaciones para los campos

                //if (model.observaciones == string.Empty || model.observaciones == null)
                //{
                //    model.observaciones = "";
                //}
                //if (model.cedulatestigo3 == 0 || model.cedulatestigo3 == null)
                //{
                //    model.cedulatestigo3 = 999999999;
                //}
                //if (model.cedulatestigo4 == 0 || model.cedulatestigo4 == null)
                //{
                //    model.cedulatestigo4 = 999999999;
                //}


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
            catch (Exception e)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

        }
        [HttpPost]
        public IActionResult Edit(int id, ActaPreMatrimonial model)
        {
            if (id != model.id)
            {
                return Json(new { success = false, message = "ID no coincide con el modelo." });
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

                    sql.Update(model);
                    sql.SaveChanges();

                    return Json(new { success = true, message = "Acta prematrimonial actualizada correctamente." });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BautizadoExists(model.id))
                    {
                        return Json(new { success = false, message = "El acta no fue encontrada." });
                    }
                    return Json(new { success = false, message = "Error de concurrencia al actualizar." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error al actualizar: " + ex.Message });
                }
            }

            return Json(new { success = false, message = "Datos inválidos. Revisa el formulario." });
        }

        private bool BautizadoExists(int id)
        {
            return sql.cursillosprematrimoniales.Any(e => e.id == id);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var acta = await sql.actasprematrimoniales.FindAsync(id);
            if (acta == null)
            {
                return Json(new { success = false, message = "Acta prematrimonial no encontrada." });
            }

            try
            {
                sql.actasprematrimoniales.Remove(acta);
                sql.SaveChanges();

                return Json(new { success = true, message = "Acta prematrimonial eliminada correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al eliminar: " + ex.Message });
            }
        }
        #endregion

        #region busquedas


        [HttpGet]
        public IActionResult SearchByName(string nombrecompleto)
        {
            if (string.IsNullOrWhiteSpace(nombrecompleto))
                return Json(new List<object>());

            var resultados = sql.actasprematrimoniales
                .Where(a => (a.nombrenovio ?? "").ToLower().Contains(nombrecompleto.ToLower())
                        || (a.nombrenovia ?? "").ToLower().Contains(nombrecompleto.ToLower()))
                .Select(a => new
                {
                    a.id,
                    a.nombrenovio,
                    a.nombrenovia,
                    a.fechamatrimonio,
                    a.fecharegistro,
                    a.estado
                })
                .ToList();

            return Json(resultados);
        }

        [HttpGet]
        public IActionResult Search()
        {
            var todos = (from a in sql.actasdematrimonio
                        join p in sql.actasprematrimoniales
                        on a.acta_prematrimonial_id equals p.id
                        select new
                        {
                            a.id,
                            p.nombrenovio,
                            p.nombrenovia,
                            a.fecha_matrimonio,
                            a.fecharegistro
                        }).ToList();

            return Json(todos);
        }

        [HttpGet]
        public IActionResult SearchReporte(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return Json(new { success = false, message = "Debe ingresar un nombre" });

            var resultados = (from a in sql.actasdematrimonio
                            join p in sql.actasprematrimoniales
                                on a.acta_prematrimonial_id equals p.id
                            where p.nombrenovio.ToLower().Contains(nombre.ToLower()) 
                                    || p.nombrenovia.ToLower().Contains(nombre.ToLower())
                            orderby a.fecharegistro descending
                            select new
                            {
                                a.id,
                                novio = p.nombrenovio,
                                novia = p.nombrenovia,
                                fechaMatrimonio = a.fecha_matrimonio,
                                parroquia = "No registrada",   // Temporal
                                sacerdote = "No asignado"     // Temporal
                            })
                            .Take(10)
                            .ToList();

            return Json(resultados);
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            var acta = sql.actasprematrimoniales.FirstOrDefault(a => a.id == id);
            if (acta == null)
                return NotFound();
            return Json(acta);
        }


        public IActionResult SearchByDate(string nombrecompleto)
        {
            var resultados = sql.actasprematrimoniales.Where(a => a.fechamatrimonio.ToString().Contains(nombrecompleto)).ToList();

            return Json(resultados); // Devolver resultados en formato JSON
        }
        #endregion

        #region abrirPestanas

        public async Task<IActionResult> ModifyActaPrematrimonialView(int? id)
        {
            if (id == null)
                return NotFound();

            var acta = await sql.actasprematrimoniales.FindAsync(id);
            if (acta == null)
                return NotFound();

            ViewBag.Cursillos = await sql.cursillosprematrimoniales.ToListAsync();
            return View(acta);
        }


        public async Task<IActionResult> DeleteActaPrematrimonialView(int? id)
        {
            if (id == null || sql.actasprematrimoniales == null)
            {
                return NotFound();
            }

            var usuario = await sql.actasprematrimoniales.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        #endregion


    }
}
