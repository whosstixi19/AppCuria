using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    public class BarrioController(ConexionPostgreSQL sql) : Controller
    {
        [HttpGet]
        public IActionResult GetBarrios()
        {
            var barrios = (from b in sql.barrios
                           join m in sql.ministros on b.ministro_id equals m.id into mj
                           from m in mj.DefaultIfEmpty()
                           join i in sql.iglesias on b.iglesia_id equals i.id into ij
                           from i in ij.DefaultIfEmpty()
                           join mi in sql.misas on b.misa_id equals mi.id into mij
                           from mi in mij.DefaultIfEmpty()
                           select new
                           {
                               b.id,
                               b.nombre,
                               b.ministro_id,
                               ministro_nombre = m != null ? m.nombre + " " + m.apellido : "",
                               b.iglesia_id,
                               iglesia_nombre = i != null ? i.nombre : "",
                               b.misa_id,
                               misa_nombre = mi != null ? mi.nombre : "",
                               b.estado,
                               b.observaciones
                           }).ToList();
            return Json(barrios);
        }

        [HttpGet]
        public IActionResult SearchByName(string nombre)
        {
            var filtro = (nombre ?? "").Trim().ToLower();
            var barrios = (from b in sql.barrios
                           join m in sql.ministros on b.ministro_id equals m.id into mj
                           from m in mj.DefaultIfEmpty()
                           join i in sql.iglesias on b.iglesia_id equals i.id into ij
                           from i in ij.DefaultIfEmpty()
                           join mi in sql.misas on b.misa_id equals mi.id into mij
                           from mi in mij.DefaultIfEmpty()
                           where b.nombre.ToLower().Contains(filtro)
                           select new
                           {
                               b.id,
                               b.nombre,
                               b.ministro_id,
                               ministro_nombre = m != null ? m.nombre + " " + m.apellido : "",
                               b.iglesia_id,
                               iglesia_nombre = i != null ? i.nombre : "",
                               b.misa_id,
                               misa_nombre = mi != null ? mi.nombre : "",
                               b.estado,
                               b.observaciones
                           }).ToList();
            return Json(barrios);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Barrio model)
        {
            var faltantes = new List<string>();
            if (string.IsNullOrWhiteSpace(model.nombre)) faltantes.Add("Nombre");
            if (model.ministro_id == 0) faltantes.Add("Ministro");
            if (model.iglesia_id == 0) faltantes.Add("Iglesia");
            if (model.misa_id == 0) faltantes.Add("Misa");
            if (string.IsNullOrWhiteSpace(model.estado)) faltantes.Add("Estado");

            if (faltantes.Count > 0)
            {
                return Json(new { success = false, message = "Falta ingresar los siguientes datos obligatorios: " + string.Join(", ", faltantes) + "." });
            }

            try
            {
                model.observaciones ??= "";
                sql.Add(model);
                await sql.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al registrar barrio: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteConfirmed([FromBody] Barrio model)
        {
            var barrio = sql.barrios.FirstOrDefault(b => b.id == model.id);
            if (barrio == null)
                return Json(new { success = false, message = "Barrio no encontrado." });

            try
            {
                sql.barrios.Remove(barrio);
                sql.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al eliminar: " + ex.Message });
            }
        }
    }
}