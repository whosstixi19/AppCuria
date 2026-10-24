using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    public class CursilloPrebautismalController(ConexionPostgreSQL sql) : Controller
    {

        [HttpPost]
        public async Task<IActionResult> Create(CursilloPrebautismal model)
        {
            // Recibe las cédulas desde el formulario (los selects envían la cédula)
            model.nombrepadre = Request.Form["nombrepadre"];
            model.nombremadre = Request.Form["nombremadre"];
            model.nombrepadrino = Request.Form["nombrepadrino"];
            model.nombremadrina = Request.Form["nombremadrina"];
            model.parroquia = int.TryParse(Request.Form["parroquia"], out var parroquiaId) ? parroquiaId : 0;

            // Remueve los errores automáticos para estos campos
            ModelState.Remove("nombrepadre");
            ModelState.Remove("nombremadre");
            ModelState.Remove("nombrepadrino");
            ModelState.Remove("nombremadrina");
            ModelState.Remove("parroquia");

            // Validación manual
            var errores = new List<string>();
            if (string.IsNullOrWhiteSpace(model.nombrepadre))
                errores.Add("La cédula del padre es obligatoria.");
            if (string.IsNullOrWhiteSpace(model.nombremadre))
                errores.Add("La cédula de la madre es obligatoria.");
            if (string.IsNullOrWhiteSpace(model.nombrepadrino))
                errores.Add("La cédula del padrino es obligatoria.");
            if (string.IsNullOrWhiteSpace(model.nombremadrina))
                errores.Add("La cédula de la madrina es obligatoria.");
            if (model.parroquia == 0)
                errores.Add("La parroquia es obligatoria.");

            if (errores.Count > 0)
            {
                return Json(new
                {
                    success = false,
                    errors = errores
                });
            }

            // Validación de ModelState para otros campos
            if (!ModelState.IsValid)
            {
                var modelStateErrors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new
                {
                    success = false,
                    errors = modelStateErrors
                });
            }

            try
            {
                model.id = 0;
                var zonaEcuador = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
                model.fechabautizo = TimeZoneInfo.ConvertTimeToUtc(model.fechabautizo, zonaEcuador);
                model.fechacursillo = TimeZoneInfo.ConvertTimeToUtc(model.fechacursillo, zonaEcuador);

                await sql.AddAsync(model);
                await sql.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    errors = $"Ocurrió un error al crear el registro. {ex.Message}"
                });
            }
        }

        #region busquedas
        [HttpGet]
        public async Task<IActionResult> Search()
        {
            var resultados = await sql.cursillosprebautismales.ToListAsync();
            if (resultados.Count > 0)
                return Json(resultados);
            return Json(resultados);
        }

        [HttpGet]
        public async Task<IActionResult> SearchAprobado()
        {
            try
            {
                var cursillosAprobados = await (
                    from c in sql.cursillosprebautismales
                    where c.estado.ToLower() == "aprobado"
                    join fp in sql.feligreses on c.nombrepadre equals fp.cedula into padreJoin
                    from padre in padreJoin.DefaultIfEmpty()
                    join fm in sql.feligreses on c.nombremadre equals fm.cedula into madreJoin
                    from madre in madreJoin.DefaultIfEmpty()
                    select new
                    {
                        id = c.id,
                        nombrecompletonino = c.nombrecompletonino,
                        fechacursillo = c.fechacursillo,
                        fechabautizo = c.fechabautizo,
                        nombrepadre = padre != null ? padre.nombre + " " + padre.apellido : c.nombrepadre,
                        nombremadre = madre != null ? madre.nombre + " " + madre.apellido : c.nombremadre,
                        parroquia = c.Parroquia != null ? c.Parroquia.nombre : "N/A"
                    }
                )
                .OrderByDescending(c => c.fechacursillo)
                .Take(10)
                .ToListAsync();

                return Json(cursillosAprobados);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
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

        [HttpGet]
        public IActionResult SearchByName(string? nombrecompleto)
        {
            var filtro = (nombrecompleto ?? "").Trim().ToLower();

            var query = sql.cursillosprebautismales.AsQueryable();

            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(a => a.nombrecompletonino.ToLower().Contains(filtro));
            }

            var resultados = query
                .OrderByDescending(a => a.id)
                .Select(a => new
                {
                    a.id,
                    a.nombrecompletonino,
                    a.telefono,
                    a.fechacursillo,
                    a.fechabautizo,
                    a.estado,
                    parroquia = a.Parroquia != null ? a.Parroquia.nombre : "N/A"
                })
                .ToList();

            return Json(resultados);
        }

        [HttpGet]
        public IActionResult SearchByNameAprobado(string? nombrecompleto)
        {
            var filtro = (nombrecompleto ?? "").Trim().ToLower();

            var query = from c in sql.cursillosprebautismales
                        where c.estado.ToLower() == "aprobado"
                        join fp in sql.feligreses on c.nombrepadre equals fp.cedula into padreJoin
                        from padre in padreJoin.DefaultIfEmpty()
                        join fm in sql.feligreses on c.nombremadre equals fm.cedula into madreJoin
                        from madre in madreJoin.DefaultIfEmpty()
                        select new
                        {
                            id = c.id,
                            nombrecompletonino = c.nombrecompletonino,
                            fechacursillo = c.fechacursillo,
                            fechabautizo = c.fechabautizo,
                            estado = c.estado,
                            nombrepadre = padre != null ? padre.nombre + " " + padre.apellido : c.nombrepadre,
                            nombremadre = madre != null ? madre.nombre + " " + madre.apellido : c.nombremadre,
                            parroquia = c.Parroquia != null ? c.Parroquia.nombre : "N/A"
                        };

            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(a => a.nombrecompletonino.ToLower().Contains(filtro));
            }

            return Json(query.OrderByDescending(c => c.fechacursillo).ToList());
        }

        #endregion

        #region otrasOperacioens

        [HttpPost]
        public IActionResult Update(int id, CursilloPrebautismal model)
        {
            // Asigna las cédulas y parroquia desde los selects
            model.nombrepadre = Request.Form["padre_id"];
            model.nombremadre = Request.Form["madre_id"];
            model.nombrepadrino = Request.Form["padrino_id"];
            model.nombremadrina = Request.Form["madrina_id"];
            model.quieninscribe = Request.Form["quieninscribe_select"];
            model.parroquia = int.TryParse(Request.Form["parroquia"], out var parroquiaId) ? parroquiaId : 0;

            // Remueve validaciones automáticas para estos campos
            ModelState.Remove("nombrepadre");
            ModelState.Remove("nombremadre");
            ModelState.Remove("nombrepadrino");
            ModelState.Remove("nombremadrina");
            ModelState.Remove("quieninscribe");
            ModelState.Remove("parroquia");

            // Validación manual
            var errores = new List<string>();
            if (string.IsNullOrWhiteSpace(model.nombrepadre))
                errores.Add("La cédula del padre es obligatoria.");
            if (string.IsNullOrWhiteSpace(model.nombremadre))
                errores.Add("La cédula de la madre es obligatoria.");
            if (string.IsNullOrWhiteSpace(model.nombrepadrino))
                errores.Add("La cédula del padrino es obligatoria.");
            if (string.IsNullOrWhiteSpace(model.nombremadrina))
                errores.Add("La cédula de la madrina es obligatoria.");
            if (model.parroquia == 0)
                errores.Add("La parroquia es obligatoria.");

            if (id != model.id)
            {
                errores.Add("El identificador no coincide.");
            }

            if (errores.Count > 0)
            {
                TempData["ErrorMessage"] = string.Join(" ", errores);
                return View(model);
            }

            // Validación de ModelState para otros campos
            if (!ModelState.IsValid)
            {
                var modelStateErrors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                TempData["ErrorMessage"] = string.Join(" ", modelStateErrors);
                return View(model);
            }

            try
            {
                var zonaEcuador = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
                model.fechabautizo = DateTime.SpecifyKind(model.fechabautizo, DateTimeKind.Unspecified);
                model.fechabautizo = TimeZoneInfo.ConvertTimeToUtc(model.fechabautizo, zonaEcuador);

                model.fechacursillo = DateTime.SpecifyKind(model.fechacursillo, DateTimeKind.Unspecified);
                model.fechacursillo = TimeZoneInfo.ConvertTimeToUtc(model.fechacursillo, zonaEcuador);

                sql.Update(model);
                sql.SaveChanges();

                TempData["SuccessMessage"] = "El cursillo se ha actualizado correctamente.";

                return RedirectToAction("BautizadosView", "Home");
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



        private bool BautizadoExists(int id)
        {
            return sql.cursillosprebautismales.Any(e => e.id == id);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await sql.cursillosprebautismales.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            else
            {
                sql.cursillosprebautismales.Remove(usuario);
                sql.SaveChanges();
            }
            return View("~/Views/Submenus/BautizadosView.cshtml");
        }

        public async Task<IActionResult> ModifyCursilloView(int? id)
        {
            if (id == null || sql.cursillosprebautismales == null)
            {
                return NotFound();
            }
            var usuario = await sql.cursillosprebautismales.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        public async Task<IActionResult> DeleteCursilloView(int? id)
        {
            if (id == null || sql.cursillosprebautismales == null)
            {
                return NotFound();
            }

            var usuario = await sql.cursillosprebautismales.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            TempData["SuccessMessage"] = "El cursillo se ha eliminado correctamente.";
            return View(usuario);
        }

        [HttpGet]
        public IActionResult Recent()
        {
            var recientes = sql.cursillosprebautismales
                .OrderByDescending(x => x.id)
                .Take(4)
                .Select(x => new
                {
                    x.id,
                    x.nombrecompletonino,
                    x.telefono,
                    x.fechacursillo,
                    x.estado,
                    parroquia = x.Parroquia != null ? x.Parroquia.nombre : null
                })
                .ToList();

            return Json(recientes);
        }

        [HttpGet]
        public IActionResult GetCursillo(int id)
        {
            var cursillo = sql.cursillosprebautismales
                .Where(c => c.id == id)
                .Select(c => new
                {
                    c.nombrecompletonino,
                    c.nombrepadre,
                    c.nombremadre,
                    c.nombrepadrino,
                    c.nombremadrina,
                    c.telefono,
                    c.celular
                    // Agrega más campos si necesitas
                })
                .FirstOrDefault();

            if (cursillo == null)
                return Json(new { success = false, message = "No se encontró el cursillo." });

            return Json(new { success = true, cursillo });
        }
        
        #endregion
    }
}
