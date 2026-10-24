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
        public IActionResult SearchAprobadoByName(string? nombrecompleto = null, string? nombre = null)
        {
            var filtro = (nombrecompleto ?? nombre ?? "").Trim().ToLower();

            var query = sql.cursillosprematrimoniales
                .Where(a => (a.estado ?? "").Trim().ToLower() == "aprobado");

            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(a =>
                    (a.nombrenovio ?? "").ToLower().Contains(filtro) ||
                    (a.nombrenovia ?? "").ToLower().Contains(filtro));
            }

            var resultados = query
                .OrderByDescending(a => a.id)
                .Select(a => new
                {
                    a.id,
                    a.nombrenovio,
                    a.nombrenovia,
                    padrino = "",
                    madrina = "",
                    a.fechacursillo,
                    a.fechamatrimonio,
                    a.estado
                })
                .ToList();

            return Json(resultados);
        }

        [HttpGet]
        public IActionResult SearchAprobado()
        {
            var cursillos = sql.cursillosprematrimoniales
                .Where(c => c.estado.Trim().ToLower() == "aprobado")
                .OrderByDescending(c => c.id)
                .Select(c => new
                {
                    id = c.id,
                    nombrenovio = c.nombrenovio,
                    nombrenovia = c.nombrenovia,
                    padrino = "",
                    madrina = "",
                    fechacursillo = c.fechacursillo,
                    fechamatrimonio = c.fechamatrimonio,
                    estado = c.estado
                })
                .ToList();

            return Json(cursillos);
        }

        [HttpGet]
        public IActionResult SearchByName(string? nombrecompleto = null, string? nombre = null)
        {
            var filtro = (nombrecompleto ?? nombre ?? "").Trim().ToLower();

            var query = sql.cursillosprematrimoniales.AsQueryable();

            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(a =>
                    (a.nombrenovia ?? "").ToLower().Contains(filtro) ||
                    (a.nombrenovio ?? "").ToLower().Contains(filtro));
            }

            var resultados = query
                .OrderByDescending(a => a.id)
                .Select(a => new
                {
                    a.id,
                    a.nombrenovio,
                    a.nombrenovia,
                    padrino = "",
                    madrina = "",
                    a.fechacursillo,
                    a.fechamatrimonio,
                    a.estado,
                    ParroquiaNombre = a.Parroquia != null ? a.Parroquia.nombre : "N/A"
                })
                .ToList();

            return Json(resultados);
        }

        [HttpGet]
        public IActionResult SearchByNames(string? nombrecompleto = null, string? nombre = null)
        {
            return SearchByName(nombrecompleto, nombre);
        }

        [HttpGet]
        public IActionResult SearchByNameAprobado(string? nombrecompleto = null, string? nombre = null)
        {
            return SearchAprobadoByName(nombrecompleto, nombre);
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
