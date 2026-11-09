using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using AplicacionCuria.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AplicacionCuria.Controllers
{
    [Authorize(Roles = "admin")]
    public class UsuariosController(ConexionPostgreSQL sql) : Controller
    {
        private void PrepararViewData(bool esSuperAdmin, int? userParroquiaId)
        {
            var query = sql.usuarios.AsQueryable();
            if (!esSuperAdmin && userParroquiaId.HasValue)
            {
                query = query.Where(u => u.parroquia_id == userParroquiaId.Value);
            }

            ViewBag.Usuarios = query.OrderBy(u => u.id).ToList();
            ViewBag.Parroquias = sql.parroquias.Where(p => p.estado == "ACTIVO").OrderBy(p => p.nombre).ToList();
            ViewBag.EsSuperAdmin = esSuperAdmin;
            ViewBag.UserParroquiaId = userParroquiaId;
        }

        [HttpGet]
        public IActionResult CrearUsuario()
        {
            var esSuperAdmin = ParroquiaTenantHelper.EsSuperAdmin(User);
            var userParroquiaId = ParroquiaTenantHelper.ObtenerParroquiaAsignada(User);
            PrepararViewData(esSuperAdmin, userParroquiaId);
            return View("~/Views/Home/CrearUsuario.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CrearUsuario(UsuarioRegisterRequest request)
        {
            var esSuperAdmin = ParroquiaTenantHelper.EsSuperAdmin(User);
            var userParroquiaId = ParroquiaTenantHelper.ObtenerParroquiaAsignada(User);

            // Si es admin de parroquia, el usuario creado queda forzosamente asignado a su sede
            if (!esSuperAdmin)
            {
                request.ParroquiaId = userParroquiaId;
            }

            // Validar contraseña solo si es nuevo usuario
            if (request.Id == 0 && string.IsNullOrWhiteSpace(request.Password))
            {
                ModelState.AddModelError(nameof(request.Password), "La contraseña es obligatoria.");
            }

            if (!ModelState.IsValid)
            {
                PrepararViewData(esSuperAdmin, userParroquiaId);
                return View("~/Views/Home/CrearUsuario.cshtml", request);
            }

            if (request.Id > 0) // Modo edición
            {
                var existente = sql.usuarios.FirstOrDefault(u => u.id == request.Id);
                if (existente == null)
                {
                    ViewBag.Error = "Usuario no encontrado.";
                    PrepararViewData(esSuperAdmin, userParroquiaId);
                    return View("~/Views/Home/CrearUsuario.cshtml", request);
                }

                // Restringir a admin local sobre usuarios ajenos
                if (!esSuperAdmin && existente.parroquia_id != userParroquiaId)
                {
                    return Forbid();
                }

                existente.nombre = request.Nombre;
                existente.apellido = request.Apellido;
                existente.correo = request.Email;
                existente.rol = request.Rol;
                if (esSuperAdmin)
                {
                    existente.parroquia_id = request.ParroquiaId;
                }

                if (!string.IsNullOrWhiteSpace(request.Password))
                {
                    var hash = PasswordHelper.GenerateHash(request.Password);
                    existente.pass = hash.Password ?? "";
                    existente.salt = hash.Salt ?? "";
                    existente.iterations = hash.Iterations;
                    existente.ultimo_acceso = null; // Clave temporal: debe cambiarla en su próximo login
                }

                await sql.SaveChangesAsync();
                TempData["Success"] = "Usuario actualizado correctamente.";
            }
            else // Modo creación
            {
                var existente = sql.usuarios.FirstOrDefault(u => u.correo == request.Email);
                if (existente != null)
                {
                    PrepararViewData(esSuperAdmin, userParroquiaId);
                    ViewBag.Error = "Este correo ya está registrado.";
                    return View("~/Views/Home/CrearUsuario.cshtml", request);
                }

                var hash = PasswordHelper.GenerateHash(request.Password);
                var nuevo = new Usuario
                {
                    correo = request.Email,
                    nombre = request.Nombre,
                    apellido = request.Apellido,
                    pass = hash.Password ?? "",
                    salt = hash.Salt ?? "",
                    iterations = hash.Iterations,
                    rol = request.Rol,
                    estado = "ACTIVO",
                    fecha_creacion = DateTime.UtcNow,
                    ultimo_acceso = null, // Clave temporal inicial entregada por el admin
                    parroquia_id = esSuperAdmin ? request.ParroquiaId : userParroquiaId
                };

                sql.usuarios.Add(nuevo);
                await sql.SaveChangesAsync();
                TempData["Success"] = "Usuario creado exitosamente.";
            }

            return RedirectToAction("CrearUsuario");
        }

        [HttpGet]
        public IActionResult EditarUsuario(int id)
        {
            var esSuperAdmin = ParroquiaTenantHelper.EsSuperAdmin(User);
            var userParroquiaId = ParroquiaTenantHelper.ObtenerParroquiaAsignada(User);

            var usuario = sql.usuarios.FirstOrDefault(u => u.id == id);
            if (usuario == null) return NotFound();

            if (!esSuperAdmin && usuario.parroquia_id != userParroquiaId)
            {
                return Forbid();
            }

            var model = new UsuarioRegisterRequest
            {
                Id = usuario.id,
                Email = usuario.correo,
                Nombre = usuario.nombre,
                Apellido = usuario.apellido,
                Rol = usuario.rol,
                ParroquiaId = usuario.parroquia_id
            };

            ViewBag.IdUsuario = id;
            PrepararViewData(esSuperAdmin, userParroquiaId);
            ViewBag.Editando = true;
            return View("~/Views/Home/CrearUsuario.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditarUsuario(int id, UsuarioRegisterRequest request)
        {
            var esSuperAdmin = ParroquiaTenantHelper.EsSuperAdmin(User);
            var userParroquiaId = ParroquiaTenantHelper.ObtenerParroquiaAsignada(User);

            var usuario = sql.usuarios.FirstOrDefault(u => u.id == id);
            if (usuario == null) return NotFound();

            if (!esSuperAdmin && usuario.parroquia_id != userParroquiaId)
            {
                return Forbid();
            }

            usuario.nombre = request.Nombre;
            usuario.apellido = request.Apellido;
            usuario.rol = request.Rol;
            usuario.correo = request.Email;
            if (esSuperAdmin)
            {
                usuario.parroquia_id = request.ParroquiaId;
            }

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                var hash = PasswordHelper.GenerateHash(request.Password);
                usuario.pass = hash.Password ?? "";
                usuario.salt = hash.Salt ?? "";
                usuario.iterations = hash.Iterations;
                usuario.ultimo_acceso = null; // Clave temporal: debe cambiarla en su próximo login
            }

            await sql.SaveChangesAsync();
            TempData["Success"] = "Usuario actualizado correctamente.";
            return RedirectToAction("CrearUsuario");
        }

        [HttpGet]
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Logout() => RedirectToAction("Logout", "Auth");

        [HttpPost]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            var esSuperAdmin = ParroquiaTenantHelper.EsSuperAdmin(User);
            var userParroquiaId = ParroquiaTenantHelper.ObtenerParroquiaAsignada(User);

            var usuario = sql.usuarios.FirstOrDefault(u => u.id == id);
            if (usuario == null) return NotFound();

            if (!esSuperAdmin && usuario.parroquia_id != userParroquiaId)
            {
                return Forbid();
            }

            sql.usuarios.Remove(usuario);
            await sql.SaveChangesAsync();

            TempData["Success"] = "Usuario eliminado correctamente.";
            return RedirectToAction("CrearUsuario");
        }
    }
}
