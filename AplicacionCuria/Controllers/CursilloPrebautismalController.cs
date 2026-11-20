using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using AplicacionCuria.Security;
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

            var pid = ParroquiaTenantHelper.ObtenerParroquiaActivaId(User, HttpContext);
            model.parroquia = pid.HasValue ? pid.Value : (int.TryParse(Request.Form["parroquia"], out var parroquiaId) ? parroquiaId : 0);

            // Remueve los errores automáticos para estos campos
            ModelState.Remove("nombrepadre");
            ModelState.Remove("nombremadre");
            ModelState.Remove("nombrepadrino");
            ModelState.Remove("nombremadrina");
            ModelState.Remove("parroquia");
            ModelState.Remove("telefono");

            if (string.IsNullOrWhiteSpace(model.telefono))
            {
                model.telefono = model.celular;
            }

            // Validación manual: al menos el padre o la madre
            var errores = new List<string>();
            if (string.IsNullOrWhiteSpace(model.nombrepadre) && string.IsNullOrWhiteSpace(model.nombremadre))
                errores.Add("Debe ingresar al menos los datos del padre o de la madre.");
            if (model.parroquia == 0)
                errores.Add("La parroquia es obligatoria.");
            if (string.IsNullOrWhiteSpace(model.estado))
                model.estado = "Aprobado";

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
                var pid = ParroquiaTenantHelper.ObtenerParroquiaActivaId(User, HttpContext);
                var baseQuery = sql.cursillosprebautismales.Where(c => c.estado.ToLower() == "aprobado");
                if (pid.HasValue)
                {
                    baseQuery = baseQuery.Where(c => c.parroquia == pid.Value);
                }

                var cursillosAprobados = await (
                    from c in baseQuery
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
            var pid = ParroquiaTenantHelper.ObtenerParroquiaActivaId(User, HttpContext);
            var query = sql.parroquias.AsQueryable();
            if (pid.HasValue)
            {
                query = query.Where(p => p.id == pid.Value);
            }

            var parroquias = query
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
            var pid = ParroquiaTenantHelper.ObtenerParroquiaActivaId(User, HttpContext);

            var query = sql.cursillosprebautismales.AsQueryable();
            if (pid.HasValue)
            {
                query = query.Where(a => a.parroquia == pid.Value);
            }

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
            var pid = ParroquiaTenantHelper.ObtenerParroquiaActivaId(User, HttpContext);
            var baseQuery = sql.cursillosprebautismales.Where(c => c.estado.ToLower() == "aprobado");
            if (pid.HasValue)
            {
                baseQuery = baseQuery.Where(c => c.parroquia == pid.Value);
            }

            var query = from c in baseQuery
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
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                          Request.Headers.Accept.ToString().Contains("application/json");

            var padreVal = Request.Form["padre_id"].FirstOrDefault() ?? Request.Form["nombrepadre"].FirstOrDefault();
            var madreVal = Request.Form["madre_id"].FirstOrDefault() ?? Request.Form["nombremadre"].FirstOrDefault();
            var padrinoVal = Request.Form["padrino_id"].FirstOrDefault() ?? Request.Form["nombrepadrino"].FirstOrDefault();
            var madrinaVal = Request.Form["madrina_id"].FirstOrDefault() ?? Request.Form["nombremadrina"].FirstOrDefault();
            var quienInscribeVal = Request.Form["quieninscribe_id"].FirstOrDefault() ?? Request.Form["quieninscribe"].FirstOrDefault() ?? Request.Form["quieninscribe_select"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(padreVal)) model.nombrepadre = padreVal;
            if (!string.IsNullOrWhiteSpace(madreVal)) model.nombremadre = madreVal;
            if (!string.IsNullOrWhiteSpace(padrinoVal)) model.nombrepadrino = padrinoVal;
            if (!string.IsNullOrWhiteSpace(madrinaVal)) model.nombremadrina = madrinaVal;
            if (!string.IsNullOrWhiteSpace(quienInscribeVal)) model.quieninscribe = quienInscribeVal;
            if (int.TryParse(Request.Form["parroquia"], out var parroquiaId) && parroquiaId > 0) model.parroquia = parroquiaId;

            // Remueve validaciones automáticas para estos campos
            ModelState.Remove("nombrepadre");
            ModelState.Remove("nombremadre");
            ModelState.Remove("nombrepadrino");
            ModelState.Remove("nombremadrina");
            ModelState.Remove("quieninscribe");
            ModelState.Remove("parroquia");

            // Validación manual: al menos el padre o la madre
            var errores = new List<string>();
            if (string.IsNullOrWhiteSpace(model.nombrepadre) && string.IsNullOrWhiteSpace(model.nombremadre))
                errores.Add("Debe ingresar al menos los datos del padre o de la madre.");
            if (model.parroquia == 0)
                errores.Add("La parroquia es obligatoria.");
            if (string.IsNullOrWhiteSpace(model.estado))
                model.estado = "Aprobado";

            if (id != model.id && model.id != 0 && id != 0)
            {
                errores.Add("El identificador no coincide.");
            }
            if (model.id == 0 && id > 0)
            {
                model.id = id;
            }

            if (errores.Count > 0)
            {
                if (isAjax) return Json(new { success = false, message = string.Join(" ", errores) });
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

                if (isAjax) return Json(new { success = false, message = string.Join(" ", modelStateErrors) });
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

                if (isAjax)
                {
                    return Json(new { success = true, message = "El cursillo se ha actualizado correctamente." });
                }

                TempData["SuccessMessage"] = "El cursillo se ha actualizado correctamente.";
                return RedirectToAction("BautizadosView", "Submenus");
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
                    c.id,
                    c.nombrecompletonino,
                    c.nombrepadre,
                    c.nombremadre,
                    c.nombrepadrino,
                    c.nombremadrina,
                    c.telefono,
                    c.celular,
                    fechabautizo = c.fechabautizo.ToString("yyyy-MM-dd"),
                    fechacursillo = c.fechacursillo.ToString("yyyy-MM-dd"),
                    c.quieninscribe,
                    c.parroquia,
                    c.estado,
                    c.observaciones
                })
                .FirstOrDefault();

            if (cursillo == null)
                return Json(new { success = false, message = "No se encontró el cursillo." });

            return Json(new { success = true, cursillo });
        }
        
        #endregion
    }
}
