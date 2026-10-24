using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    public class ActaMatrimonialController(ConexionPostgreSQL sql) : Controller
    {

        #region CRUD

        [HttpPost]
        public async Task<IActionResult> Create(ActaDeMatrimonio model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Buscar el acta prematrimonial relacionada
                    var actaPre = await sql.actasprematrimoniales
                        .FirstOrDefaultAsync(a => a.id == model.acta_prematrimonial_id);

                    if (actaPre != null)
                    {
                        model.fecha_matrimonio = actaPre.fechamatrimonio;
                        model.acta = actaPre.acta;
                        model.tomo = actaPre.tomo;
                        model.pagina = actaPre.pagina;
                        model.iglesia_id = actaPre.iglesia_id; 
                        model.ministro_id = actaPre.ministro_id; 
                    }
                    else
                    {
                        return Json(new { success = false, errors = "No se encontró el acta prematrimonial." });
                    }
                

                    model.id = 0;
                    var zonaEcuador = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

                    model.fecha_matrimonio = TimeZoneInfo.ConvertTimeToUtc(model.fecha_matrimonio, zonaEcuador);
                    if (model.fecharegistro.HasValue)
                        model.fecharegistro = TimeZoneInfo.ConvertTimeToUtc(model.fecharegistro.Value, zonaEcuador);



                    await sql.AddAsync(model);
                    await sql.SaveChangesAsync();

                    return Json(new { success = true });
                }

                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    success = false,
                    errors = "Ocurrió un error al crear el registro. " + e.Message,
                    exception = e.Message
                });
            }
        }
        
        [HttpPost]
        public IActionResult Edit(ActaDeMatrimonio model)
        {
            try
            {
                if (!BautizadoExists(model.id))
                {
                    return Json(new { success = false, errors = "El acta no existe." });
                }

                if (ModelState.IsValid)
                {
                    var zonaEcuador = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

                    model.fecha_matrimonio = TimeZoneInfo.ConvertTimeToUtc(model.fecha_matrimonio, zonaEcuador);
                    if (model.fecharegistro.HasValue)
                        model.fecharegistro = TimeZoneInfo.ConvertTimeToUtc(model.fecharegistro.Value, zonaEcuador);

                    sql.Update(model);
                    sql.SaveChanges();

                    return Json(new { success = true });
                }

                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    success = false,
                    errors = "Ocurrió un error al modificar el registro. " + e.Message,
                    exception = e.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var acta = await sql.actasprematrimoniales.FirstOrDefaultAsync(c => c.id == id);

            if (acta == null)
                return NotFound();

            return Json(new
            {
                acta.id,
                acta.fechamatrimonio,
                acta.pagina,
                acta.tomo,
                acta.acta,  
                acta.ministro_id,
                acta.iglesia_id
            });
        }


        private bool BautizadoExists(int id)
        {
            return sql.actasdematrimonio.Any(e => e.id == id);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await sql.actasdematrimonio.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            else
            {
                sql.actasdematrimonio.Remove(usuario);
                sql.SaveChanges();
            }
            return Json(new { Success = true });
        }
        #endregion

        #region busquedas
        [HttpGet]
        public async Task<IActionResult> Search()
        {
            var resultados = await sql.actasdematrimonio.ToListAsync();
            if (resultados.Count > 0)
                return Json(resultados); // Envía los resultados a la vista

            return Json(resultados);
        }
        public IActionResult SearchByDate(string nombrecompleto)
        {
            var resultados = sql.actasdematrimonio.Where(a => a.fecharegistro.ToString().Contains(nombrecompleto)).ToList();

            return Json(resultados); // Devolver resultados en formato JSON
        }
        #endregion

        [HttpGet]
        public IActionResult BuscarPorNombre(string? nombre)
        {
            var filtro = (nombre ?? "").Trim().ToLower();

            var query = sql.actasprematrimoniales.AsQueryable();

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
                    id = a.id,
                    nombreNovio = a.nombrenovio,
                    nombreNovia = a.nombrenovia,
                    fechaMatrimonio = a.fechamatrimonio
                })
                .ToList();

            return Json(resultados);
        }

        [HttpGet]
        public IActionResult BuscarPorNombreMatrimoniales(string? nombre)
        {
            var filtro = (nombre ?? "").Trim().ToLower();

            var query = from a in sql.actasdematrimonio
                        join ap in sql.actasprematrimoniales
                            on a.acta_prematrimonial_id equals ap.id
                        select new
                        {
                            id = a.id,
                            padrino1 = a.padrino1,
                            padrino2 = a.padrino2,
                            fechaRegistro = a.fecharegistro,
                            nombreNovio = ap.nombrenovio,
                            nombreNovia = ap.nombrenovia
                        };

            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(x =>
                    (x.nombreNovio ?? "").ToLower().Contains(filtro) ||
                    (x.nombreNovia ?? "").ToLower().Contains(filtro) ||
                    (x.padrino1 ?? "").ToLower().Contains(filtro) ||
                    (x.padrino2 ?? "").ToLower().Contains(filtro));
            }

            return Json(query.OrderByDescending(x => x.id).ToList());
        }

        #region abrirPestanas

        public async Task<IActionResult> ModifyActamatrimonialView(int? id)
        {
            if (id == null || sql.actasdematrimonio == null)
            {
                return NotFound();
            }

            var usuario = await sql.actasdematrimonio.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
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


        public async Task<IActionResult> DeleteActamatrimonialView(int? id)
        {
            if (id == null || sql.actasdematrimonio == null)
            {
                return NotFound();
            }

            var usuario = await sql.actasdematrimonio.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        #endregion

    }
}
