using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplicacionCuria.Controllers
{
    
    [Authorize]
    public class CursilloPreMatrimonialController : Controller
    {
        private readonly ConexionPostgreSQL sql;

        public CursilloPreMatrimonialController(ConexionPostgreSQL sql)
        {
            this.sql = sql;
        }

        #region CRUD
        public async Task<IActionResult> Create(CursilloPreMatrimonial model)
        {
            ModelState.Remove("Parroquia");
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            model.id = 0;

            var zonaEcuador = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

            model.fechamatrimonio = DateTime.SpecifyKind(model.fechamatrimonio, DateTimeKind.Unspecified);
            model.fechamatrimonio = TimeZoneInfo.ConvertTimeToUtc(model.fechamatrimonio, zonaEcuador);

            model.fechacursillo = DateTime.SpecifyKind(model.fechacursillo, DateTimeKind.Unspecified);
            model.fechacursillo = TimeZoneInfo.ConvertTimeToUtc(model.fechacursillo, zonaEcuador);

            model.fechanacimientonovia = DateTime.SpecifyKind(model.fechanacimientonovia, DateTimeKind.Unspecified);
            model.fechanacimientonovia = TimeZoneInfo.ConvertTimeToUtc(model.fechanacimientonovia, zonaEcuador);

            model.fechanacimientonovio = DateTime.SpecifyKind(model.fechanacimientonovio, DateTimeKind.Unspecified);
            model.fechanacimientonovio = TimeZoneInfo.ConvertTimeToUtc(model.fechanacimientonovio, zonaEcuador);

            // model.estado = model.estados.ToString();
            await sql.AddAsync(model);
            await sql.SaveChangesAsync();
            //return RedirectToAction("BautizadosView", "Submenus");
            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult GetSacerdotes()
        {
            var sacerdotes = sql.sacerdotes
                .Select(s => new
                {
                    id = s.id,
                    nombre = s.nombre + " " + s.apellido
                })
                .ToList();

            return Json(sacerdotes);
        }

        [HttpGet]
        public IActionResult GetParroquias()
        {
            var parroquias = sql.parroquias
                .Select(p => new
                {
                    id = p.id,
                    nombre = p.nombre
                })
                .ToList();

            return Json(parroquias);
        }

        [HttpPost]
        public IActionResult Edit(int id, CursilloPreMatrimonial model)
        {
            ModelState.Remove("Parroquia");
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
                var zonaEcuador = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

                model.fechamatrimonio = DateTime.SpecifyKind(model.fechamatrimonio, DateTimeKind.Unspecified);
                model.fechamatrimonio = TimeZoneInfo.ConvertTimeToUtc(model.fechamatrimonio, zonaEcuador);

                model.fechacursillo = DateTime.SpecifyKind(model.fechacursillo, DateTimeKind.Unspecified);
                model.fechacursillo = TimeZoneInfo.ConvertTimeToUtc(model.fechacursillo, zonaEcuador);

                model.fechanacimientonovia = DateTime.SpecifyKind(model.fechanacimientonovia, DateTimeKind.Unspecified);
                model.fechanacimientonovia = TimeZoneInfo.ConvertTimeToUtc(model.fechanacimientonovia, zonaEcuador);

                model.fechanacimientonovio = DateTime.SpecifyKind(model.fechanacimientonovio, DateTimeKind.Unspecified);
                model.fechanacimientonovio = TimeZoneInfo.ConvertTimeToUtc(model.fechanacimientonovio, zonaEcuador);

                sql.Update(model);
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
            TempData["SuccessMessage"] = "¡El cursillo ha sido actualizado correctamente!";
            return RedirectToAction("MatrimoniosView", "Home");
        }

        private bool BautizadoExists(int id)
        {
            return sql.cursillosprematrimoniales.Any(e => e.id == id);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await sql.cursillosprematrimoniales.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            else
            {
                sql.cursillosprematrimoniales.Remove(usuario);
                sql.SaveChanges();
            }
            TempData["SuccessMessage"] = "¡El cursillo se eliminó correctamente!";
            return View("~/Views/Submenus/MatrimoniosView.cshtml");
        }

        #endregion

        #region busquedas
        [HttpGet]
        public async Task<IActionResult> Search()
        {
            var resultados = await sql.cursillosprematrimoniales.ToListAsync();
            if (resultados.Count > 0)
                return Json(resultados); // Envía los resultados a la vista

            return Json(resultados);
        }

        [HttpGet]
        public IActionResult SearchAprobadoByName(string nombrecompleto)
        {

            if (string.IsNullOrWhiteSpace(nombrecompleto))
            {
                return Json(new { success = false, message = "El parámetro nombrecompleto es requerido." });
            }

            nombrecompleto = nombrecompleto.Trim().ToLower();

            var resultados = sql.cursillosprematrimoniales
                .Where(a =>
                    ((a.nombrenovio ?? "").ToLower().Contains(nombrecompleto) ||
                    (a.nombrenovia ?? "").ToLower().Contains(nombrecompleto)) &&
                    (a.estado ?? "").Trim().ToLower() == "aprobado")
                .Select(a => new
                {
                    a.id,
                    a.nombrenovio,
                    a.nombrenovia,
                    a.fechacursillo,
                    a.fechamatrimonio,
                    a.estado
                    // NO devolvemos a.parroquia para evitar el conflicto
                })
                .ToList();

            Console.WriteLine("[DEBUG] Resultados encontrados: " + resultados.Count);

            return Json(resultados);
        }

        [HttpGet]
        public IActionResult SearchAprobado()
        {
            var cursillos = sql.cursillosprematrimoniales
                .Where(c => c.estado.Trim().ToLower() == "aprobado")
                .Select(c => new
                {
                    id = c.id,
                    nombrenovio = c.nombrenovio,
                    nombrenovia = c.nombrenovia,
                    fechacursillo = c.fechacursillo,
                    fechamatrimonio = c.fechamatrimonio
                })
                .ToList();

            return Json(cursillos);
        }

        [HttpGet]
        public IActionResult SearchByName(string nombrecompleto)
        {
            Console.WriteLine($"=== DEBUG SearchByName ===");
            Console.WriteLine($"nombrecompleto recibido: '{nombrecompleto}'");

            if (string.IsNullOrWhiteSpace(nombrecompleto))
            {
                return Json(new { success = false, message = "El parámetro nombrecompleto es requerido." });
            }

            nombrecompleto = nombrecompleto.Trim();

            var resultados = sql.cursillosprematrimoniales
                .Where(a => EF.Functions.ILike(a.nombrenovia, $"%{nombrecompleto}%") 
                        || EF.Functions.ILike(a.nombrenovio, $"%{nombrecompleto}%"))
                .Select(a => new
                {
                    a.id,
                    a.nombrenovio,
                    a.nombrenovia,
                    a.fechacursillo,
                    a.fechamatrimonio,
                    a.estado,
                    ParroquiaNombre = a.Parroquia != null ? a.Parroquia.nombre : "N/A"
                })
                .ToList();

            Console.WriteLine($"Resultados encontrados: {resultados.Count}");

            return Json(resultados);
        }

        [HttpGet]
        public IActionResult SearchByNameAprobado(string nombrecompleto)
        {
            if (string.IsNullOrWhiteSpace(nombrecompleto))
            {
                return Json(new { success = false, message = "El parámetro nombrecompleto es requerido." });
            }

            var resultados = sql.cursillosprematrimoniales
                .Where(a =>
                    ((a.nombrenovio ?? "").ToLower().Contains(nombrecompleto.ToLower()) ||
                    (a.nombrenovia ?? "").ToLower().Contains(nombrecompleto.ToLower())) &&
                    (a.estado ?? "").ToLower() == "aprobado")
                .Select(a => new
                {
                    a.id,
                    a.nombrenovio,
                    a.nombrenovia,
                    a.fechacursillo,
                    a.fechamatrimonio,
                    a.estado
                })
                .ToList();

            return Json(resultados);
        }


        [HttpGet]
        public IActionResult GetById(int id)
        {
            var cursillo = sql.cursillosprematrimoniales
                .Where(c => c.id == id)
                .Select(c => new
                {
                    c.id,
                    c.nombrenovio,
                    c.nombrenovia,
                    c.cedulanovio,
                    c.cedulanovia,
                    c.telefononovio,
                    c.telefononovia,
                    c.fechacursillo,
                    c.fechamatrimonio,
                    c.fechanacimientonovia,
                    c.fechanacimientonovio,
                    c.estado,
                    c.observaciones,
                    parroquiaId = c.parroquia // Devuelves solo el id de parroquia
                })
                .FirstOrDefault();

            if (cursillo == null)
            {
                return NotFound();
            }

            return Json(cursillo);
        }


        #endregion

        #region abrirPestanas

        public async Task<IActionResult> ModifyCursilloMatView(int? id)
        {
            if (id == null || sql.cursillosprematrimoniales == null)
            {
                return NotFound();
            }

            var usuario = await sql.cursillosprematrimoniales.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }


        public async Task<IActionResult> DeleteCursilloMatView(int? id)
        {
            if (id == null || sql.cursillosprematrimoniales == null)
            {
                return NotFound();
            }

            var usuario = await sql.cursillosprematrimoniales.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        #endregion
    }
}
