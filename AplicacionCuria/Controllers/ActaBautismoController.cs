using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    public class ActaBautismoController(ConexionPostgreSQL sql) : Controller
    {
        #region Listados

        [HttpGet]
        public IActionResult GetSacerdotes()
        {
            var sacerdotes = sql.sacerdotes.Select(s => new { s.id, s.nombre }).ToList();
            return Json(sacerdotes);
        }

        [HttpGet]
        public IActionResult GetSacerdotesCert()
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
        public IActionResult GetMinistros()
        {
            var ministros = sql.ministros.Select(m => new { m.id, m.nombre }).ToList();
            return Json(ministros);
        }

        [HttpGet]
        public IActionResult GetIglesias()
        {
            var iglesias = sql.iglesias.Select(i => new { i.id, i.nombre }).ToList();
            return Json(iglesias);
        }

        [HttpGet]
        public IActionResult GetParroquias()
        {
            var parroquias = sql.parroquias.Select(p => new { p.id, p.nombre }).ToList();
            return Json(parroquias);
        }

        [HttpGet]
        public async Task<IActionResult> ListActasBautismo()
        {
            var resultados = await sql.actasbautismo.ToListAsync();
            return Json(resultados);
        }

        [HttpGet]
        public IActionResult BuscarCursillosAprobados(string? nombre)
        {
            var filtro = (nombre ?? "").Trim().ToLower();

            var query = sql.cursillosprebautismales
                .Where(c => c.estado == "Aprobado");

            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(c => c.nombrecompletonino.ToLower().Contains(filtro));
            }

            var resultados = query
                .OrderByDescending(c => c.id)
                .Select(c => new
                {
                    c.id,
                    c.nombrecompletonino,
                    c.fechacursillo
                }).ToList();

            return Json(resultados);
        }


        #endregion

        #region CRUD

        [HttpPost]
        public async Task<IActionResult> Create(ActaBautismo model)
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

                if (model.cursillo_id == 0 || !sql.cursillosprebautismales.Any(c => c.id == model.cursillo_id))
                {
                    return Json(new { success = false, errors = new[] { "Debe seleccionar un cursillo prebautismal válido antes de registrar." } });
                }

                var zonaEcuador = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

                model.fechabautizo = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(model.fechabautizo, DateTimeKind.Unspecified), zonaEcuador);
                model.fechanacimiento = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(model.fechanacimiento, DateTimeKind.Unspecified), zonaEcuador);
                model.registrocivilfecha = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(model.registrocivilfecha, DateTimeKind.Unspecified), zonaEcuador);

                var ministro = await sql.ministros.FindAsync(model.ministro_id);
                var iglesia = await sql.iglesias.FindAsync(model.iglesia_id);
                var sacerdote = await sql.sacerdotes.FindAsync(model.sacerdote_id);

                List<string> errores = new();

                if (ministro == null) errores.Add("Ministro no encontrado");
                if (iglesia == null) errores.Add("Iglesia no encontrada");
                if (sacerdote == null) errores.Add("Sacerdote no encontrado");

                if (errores.Count > 0)
                    throw new Exception(string.Join(" | ", errores));

                sql.Add(model);
                await sql.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    errors = $"Ocurrió un error al crear el registro. {ex.Message}",
                    exception = ex.Message
                });
            }
        }

        [HttpPost]
        public IActionResult Edit(int id, ActaBautismo model)
        {
            if (id != model.id)
            {
                return Json(new { success = false, error = "ID no coincide" });
            }

            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            try
            {
                var zonaEcuador = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

                model.fechabautizo = DateTime.SpecifyKind(model.fechabautizo, DateTimeKind.Unspecified);
                model.fechabautizo = TimeZoneInfo.ConvertTimeToUtc(model.fechabautizo, zonaEcuador);

                model.fechanacimiento = DateTime.SpecifyKind(model.fechanacimiento, DateTimeKind.Unspecified);
                model.fechanacimiento = TimeZoneInfo.ConvertTimeToUtc(model.fechanacimiento, zonaEcuador);

                model.registrocivilfecha = DateTime.SpecifyKind(model.registrocivilfecha, DateTimeKind.Unspecified);
                model.registrocivilfecha = TimeZoneInfo.ConvertTimeToUtc(model.registrocivilfecha, zonaEcuador);

                sql.Update(model);
                sql.SaveChanges();

                return Json(new { success = true });
            }
            catch (DbUpdateConcurrencyException)
            {
                return Json(new { success = false, error = "Error de concurrencia o acta no encontrada" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var acta = await sql.actasbautismo.FindAsync(id);
            if (acta == null)
                return NotFound();

            sql.actasbautismo.Remove(acta);
            await sql.SaveChangesAsync();
            return Json(new { success = true });
        }

        private bool ActaBautismoExists(int id)
        {
            return sql.actasbautismo.Any(e => e.id == id);
        }

        #endregion

        #region AbrirPestanas

        public async Task<IActionResult> ModifyActaBautismoView(int? id)
        {
            if (id == null || sql.actasbautismo == null)
                return NotFound();

            var acta = await sql.actasbautismo.FindAsync(id);
            ViewBag.Ministros = sql.ministros.ToList();
            ViewBag.Iglesias = sql.iglesias.ToList();
            ViewBag.Parroquias = sql.parroquias.ToList();
            ViewBag.Cursillos = sql.cursillosprebautismales.ToList();

            if (acta == null)
                return NotFound();

            return View("ModifyActaBautismoView", acta);
        }

        public async Task<IActionResult> DeleteActaBautismoView(int? id)
        {
            if (id == null || sql.actasbautismo == null)
                return NotFound();

            var acta = await sql.actasbautismo.FindAsync(id);
            if (acta == null)
                return NotFound();

            // Buscar nombres usando los IDs
            var ministro = await sql.ministros.FindAsync(acta.ministro_id);
            var iglesia = await sql.iglesias.FindAsync(acta.iglesia_id);

            ViewBag.MinistroNombre = ministro?.nombre ?? "";
            ViewBag.IglesiaNombre = iglesia?.nombre ?? "";

            return View(acta);
        }

        public async Task<IActionResult> Valores(int id)
        {
            var acta = await sql.actasbautismo.FindAsync(id);
            return Json(acta);
        }

        // Certificado de Acta de Bautismo
        public async Task<IActionResult> CertificadoActaBautismo(int id)
        {
            var acta = await sql.actasbautismo.FindAsync(id);
            if (acta == null)
                return NotFound();

            // nombres relacionados
            var iglesia = await sql.iglesias.FindAsync(acta.iglesia_id);
            var ministro = await sql.ministros.FindAsync(acta.ministro_id);
            var parroquia = await sql.parroquias.FindAsync(acta.parroquia_id);

            ViewBag.IglesiaNombre = iglesia?.nombre ?? "";
            ViewBag.MinistroNombre = ministro?.nombre ?? "";
            ViewBag.ParroquiaNombre = parroquia?.nombre ?? "";

            return View("CertificadoActaBautismo", acta);
        }


        #endregion

        #region Busquedas

        [HttpGet]
        public IActionResult Search()
        {
            var resultados = (from a in sql.actasbautismo
                              join i in sql.iglesias on a.iglesia_id equals i.id
                              orderby a.id descending
                              select new
                              {
                                  a.id,
                                  a.nombrecompleto,
                                  a.fechabautizo,
                                  a.cedulapadre,
                                  a.cedulamadre,
                                  iglesia = i.nombre,
                                  iglesia_nombre = i.nombre
                              }).ToList();

            return Json(resultados);
        }

        [HttpGet]
        public IActionResult SearchByName(string? nombrecompleto)
        {
            var filtro = (nombrecompleto ?? "").Trim().ToLower();

            var query = from a in sql.actasbautismo
                        join i in sql.iglesias on a.iglesia_id equals i.id
                        select new
                        {
                            a.id,
                            a.nombrecompleto,
                            a.fechabautizo,
                            a.cedulapadre,
                            a.cedulamadre,
                            iglesia = i.nombre,
                            iglesia_nombre = i.nombre
                        };

            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(a => a.nombrecompleto.ToLower().Contains(filtro));
            }

            return Json(query.OrderByDescending(a => a.id).ToList());
        }

        [HttpGet]
        public IActionResult SearchAll(int count = 20)
        {
            var resultados = (from a in sql.actasbautismo
                              join i in sql.iglesias on a.iglesia_id equals i.id
                              orderby a.id descending
                              select new
                              {
                                  id = a.id,
                                  nombrecompleto = a.nombrecompleto,
                                  fechabautizo = a.fechabautizo,
                                  cedulapadre = a.cedulapadre,
                                  cedulamadre = a.cedulamadre,
                                  iglesia = i.nombre,
                                  iglesia_nombre = i.nombre
                              }).Take(count).ToList();

            return Json(resultados);
        }
        
        [HttpGet]
        public IActionResult SearchAllDetailed(int count = 20)
        {
            var resultados = (from a in sql.actasbautismo
                            join i in sql.iglesias on a.iglesia_id equals i.id
                            orderby a.id descending
                            select new
                            {
                                id = a.id,
                                nombrecompleto = a.nombrecompleto,
                                fechabautizo = a.fechabautizo,
                                cedulapadre = a.cedulapadre,
                                cedulamadre = a.cedulamadre,
                                iglesia = i.nombre,
                                iglesia_nombre = i.nombre
                            }).Take(count).ToList();

            return Json(resultados);
        }


        #endregion
    } 
}