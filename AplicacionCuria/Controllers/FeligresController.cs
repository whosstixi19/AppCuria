using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    public class FeligresController(ConexionPostgreSQL sql) : Controller
    {
        [HttpGet]
        public IActionResult GetFeligreses()
        {
            var feligreses = sql.feligreses
                .Select(f => new
                {
                    f.id,
                    f.nombre,
                    f.apellido,
                    f.cedula,
                    f.fechanacimiento,
                    f.direccion,
                    f.telefono,
                    f.email,
                    f.estado,
                    f.observaciones
                })
                .ToList();

            return Json(feligreses);
        }

        [HttpGet]
        public IActionResult SearchByName(string nombre)
        {
            var filtro = (nombre ?? "").Trim().ToLower();
            var resultados = sql.feligreses
                .Where(f => (f.nombre + " " + f.apellido).ToLower().Contains(filtro) ||
                            (f.cedula ?? "").ToLower().Contains(filtro))
                .Select(f => new
                {
                    f.id,
                    f.nombre,
                    f.apellido,
                    f.cedula,
                    f.fechanacimiento,
                    f.direccion,
                    f.telefono,
                    f.email,
                    f.estado,
                    f.observaciones
                })
                .ToList();

            return Json(resultados);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Feligres model)
        {
            // Validación de campos obligatorios
            if (string.IsNullOrWhiteSpace(model.nombre) ||
                string.IsNullOrWhiteSpace(model.apellido) ||
                string.IsNullOrWhiteSpace(model.cedula) ||
                model.fechanacimiento == default ||
                string.IsNullOrWhiteSpace(model.direccion) ||
                string.IsNullOrWhiteSpace(model.telefono) ||
                string.IsNullOrWhiteSpace(model.email) ||
                string.IsNullOrWhiteSpace(model.estado) ||
                string.IsNullOrWhiteSpace(model.observaciones))
            {
                return Json(new { success = false, message = "Todos los campos son obligatorios." });
            }

            // Normalizar nombre y apellido
            model.nombre = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(model.nombre.Trim().ToLower());
            model.apellido = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(model.apellido.Trim().ToLower());

            // Validar que la cédula no se repita
            if (sql.feligreses.Any(f => f.cedula == model.cedula))
            {
                return Json(new { success = false, message = "La cédula ya está registrada." });
            }

            // Asegura que la fecha tenga Kind=Utc para PostgreSQL
            if (model.fechanacimiento.Kind == DateTimeKind.Unspecified)
                model.fechanacimiento = DateTime.SpecifyKind(model.fechanacimiento, DateTimeKind.Utc);

            try
            {
                sql.Add(model);
                await sql.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al registrar feligrés: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Edit(int id, [FromBody] Feligres model)
        {
            if (id != model.id)
            {
                return NotFound();
            }

            // Solución: Forzar fechanacimiento a UTC si viene como Unspecified
            if (model.fechanacimiento.Kind == DateTimeKind.Unspecified)
                model.fechanacimiento = DateTime.SpecifyKind(model.fechanacimiento, DateTimeKind.Utc);

            try
            {
                sql.Update(model);
                sql.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeligresExists(model.id))
                {
                    return NotFound();
                }
                throw;
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult DeleteConfirmed([FromBody] Feligres model)
        {
            var feligres = sql.feligreses.FirstOrDefault(f => f.id == model.id);
            if (feligres == null)
                return Json(new { success = false, message = "Feligrés no encontrado." });

            try
            {
                sql.feligreses.Remove(feligres);
                sql.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al eliminar: " + ex.Message });
            }
        }

        private bool FeligresExists(int id)
        {
            return sql.feligreses.Any(e => e.id == id);
        }

        [HttpGet]
        public IActionResult GetLast(int count = 4)
        {
            var feligreses = sql.feligreses
                .OrderByDescending(f => f.id)
                .Take(count)
                .Select(f => new
                {
                    f.id,
                    nombre = f.nombre + " " + f.apellido,
                    f.cedula
                })
                .ToList();

            return Json(feligreses);
        }

        [HttpGet]
        public IActionResult GetLastFull(int count = 10)
        {
            var feligreses = sql.feligreses
                .OrderByDescending(f => f.id)
                .Take(count)
                .Select(f => new
                {
                    f.id,
                    f.nombre,
                    f.apellido,
                    f.cedula,
                    f.fechanacimiento,
                    f.direccion,
                    f.telefono,
                    f.email,
                    f.estado,
                    f.observaciones
                })
                .ToList();

            return Json(feligreses);
        }
    }
}