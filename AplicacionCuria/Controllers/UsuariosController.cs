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
        [HttpGet]
        public IActionResult CrearUsuario()
        {
            var usuarios = sql.usuarios.ToList();
            ViewBag.Usuarios = usuarios;
            return View("~/Views/Home/CrearUsuario.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CrearUsuario(UsuarioRegisterRequest request)
        {
            // ✅ Validar contraseña solo si es nuevo usuario
            if (request.Id == 0 && string.IsNullOrWhiteSpace(request.Password))
            {
                ModelState.AddModelError(nameof(request.Password), "La contraseña es obligatoria.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Usuarios = sql.usuarios.ToList();
                return View("~/Views/Home/CrearUsuario.cshtml", request);
            }

            if (request.Id > 0) // Modo edición
            {
                var existente = sql.usuarios.FirstOrDefault(u => u.id == request.Id);
                if (existente == null)
                {
                    ViewBag.Error = "Usuario no encontrado.";
                    ViewBag.Usuarios = sql.usuarios.ToList();
                    return View("~/Views/Home/CrearUsuario.cshtml", request);
                }

                existente.nombre = request.Nombre;
                existente.apellido = request.Apellido;
                existente.correo = request.Email;
                existente.rol = request.Rol;

                if (!string.IsNullOrWhiteSpace(request.Password))
                {
                    var hash = PasswordHelper.GenerateHash(request.Password);
                    existente.pass = hash.Password ?? "";
                    existente.salt = hash.Salt ?? "";
                    existente.iterations = hash.Iterations;
                }

                await sql.SaveChangesAsync();
                TempData["Success"] = "Usuario actualizado correctamente.";
            }
            else // Modo creación
            {
                var existente = sql.usuarios.FirstOrDefault(u => u.correo == request.Email);
                if (existente != null)
                {
                    ViewBag.Usuarios = sql.usuarios.ToList();
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
                    fecha_creacion = DateTime.UtcNow
                };

                sql.usuarios.Add(nuevo);
                TempData["Success"] = "Usuario creado exitosamente.";
                await sql.SaveChangesAsync();
            }

            return RedirectToAction("CrearUsuario");
        }

        [HttpGet]
        public IActionResult EditarUsuario(int id)
        {
            var usuario = sql.usuarios.FirstOrDefault(u => u.id == id);
            if (usuario == null) return NotFound();

            var model = new UsuarioRegisterRequest
            {
                Id = usuario.id,
                Email = usuario.correo,
                Nombre = usuario.nombre,
                Apellido = usuario.apellido,
                Rol = usuario.rol
            };

            ViewBag.IdUsuario = id;
            ViewBag.Usuarios = sql.usuarios.ToList();
            ViewBag.Editando = true;
            return View("~/Views/Home/CrearUsuario.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditarUsuario(int id, UsuarioRegisterRequest request)
        {
            var usuario = sql.usuarios.FirstOrDefault(u => u.id == id);
            if (usuario == null) return NotFound();

            usuario.nombre = request.Nombre;
            usuario.apellido = request.Apellido;
            usuario.rol = request.Rol;
            usuario.correo = request.Email;

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                var hash = PasswordHelper.GenerateHash(request.Password);
                usuario.pass = hash.Password ?? "";
                usuario.salt = hash.Salt ?? "";
                usuario.iterations = hash.Iterations;
            }

            await sql.SaveChangesAsync();
            TempData["Success"] = "Usuario actualizado correctamente.";
            return RedirectToAction("CrearUsuario");
        }

        [HttpGet]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            var usuario = sql.usuarios.FirstOrDefault(u => u.id == id);
            if (usuario == null) return NotFound();

            sql.usuarios.Remove(usuario);
            await sql.SaveChangesAsync();

            TempData["Success"] = "Usuario eliminado correctamente.";
            return RedirectToAction("CrearUsuario");
        }
    }
}
