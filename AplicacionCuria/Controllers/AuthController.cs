using System.Security.Claims;
using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using AplicacionCuria.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;


namespace AplicacionCuria.Controllers
{
    [Authorize]
    public class AuthController(ConexionPostgreSQL sql) : Controller
    {

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var cliente = sql.usuarios.FirstOrDefault(c => c.correo == request.Email);
            
            if (cliente == null)
            {
                ViewBag.Error = "Correo no encontrado";
                ViewBag.ErrorType = "email"; // Limpiar ambos campos
                return View();
            }

            var isPasswordValid = PasswordHelper.VerifyPassword(request.Password, cliente.salt, cliente.pass);

            if (!isPasswordValid)
            {
                ViewBag.Error = "Contraseña incorrecta";
                ViewBag.ErrorType = "password"; // Solo limpiar contraseña
                ViewBag.Email = request.Email; // <- conservar el correo ingresado
                return View();
            }

            var primerLogin = cliente.ultimo_acceso == null;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, cliente.nombre),
                new(ClaimTypes.Email, cliente.correo),
                new(ClaimTypes.Role, cliente.rol)
            };

            if (cliente.parroquia_id.HasValue)
            {
                claims.Add(new Claim(ParroquiaTenantHelper.ClaimParroquiaId, cliente.parroquia_id.Value.ToString()));
            }

            if (primerLogin)
            {
                // Contraseña temporal (semilla del sistema o clave inicial
                // entregada por el admin): obligar a definirla nueva antes de
                // usar la aplicación (ver DebeCambiarClaveFilter)
                claims.Add(new Claim("debe_cambiar_clave", "true"));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            // Registrar el acceso solo si ya no tiene contraseña temporal pendiente
            if (!primerLogin)
            {
                cliente.ultimo_acceso = DateTime.UtcNow;
                await sql.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: pantalla para definir la nueva contraseña del primer login
        [HttpGet]
        public IActionResult CambiarClave()
        {
            return View();
        }

        // POST: definir la nueva contraseña (primer login)
        [HttpPost]
        public async Task<IActionResult> CambiarClave(string Clave, string Confirmar)
        {
            var correo = User.FindFirstValue(ClaimTypes.Email);
            var usuario = sql.usuarios.FirstOrDefault(u => u.correo == correo);

            if (usuario == null)
            {
                // Sesión inconsistente: forzar a iniciar de nuevo
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login");
            }

            if (string.IsNullOrWhiteSpace(Clave) || Clave.Length < 8)
            {
                ViewBag.Error = "La contraseña debe tener al menos 8 caracteres.";
                return View();
            }

            if (Clave != Confirmar)
            {
                ViewBag.Error = "Las contraseñas no coinciden.";
                return View();
            }

            if (PasswordHelper.VerifyPassword(Clave, usuario.salt, usuario.pass))
            {
                ViewBag.Error = "La contraseña nueva no puede ser igual a la actual.";
                return View();
            }

            var hash = PasswordHelper.GenerateHash(Clave);
            usuario.pass = hash.Password ?? string.Empty;
            usuario.salt = hash.Salt ?? string.Empty;
            usuario.iterations = hash.Iterations;
            usuario.ultimo_acceso = DateTime.UtcNow;
            await sql.SaveChangesAsync();

            // Re-emitir la cookie sin la marca de cambio pendiente
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, usuario.nombre),
                new(ClaimTypes.Email, usuario.correo),
                new(ClaimTypes.Role, usuario.rol)
            };
            if (usuario.parroquia_id.HasValue)
            {
                claims.Add(new Claim(ParroquiaTenantHelper.ClaimParroquiaId, usuario.parroquia_id.Value.ToString()));
            }
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            TempData["SuccessMessage"] = "Contraseña actualizada correctamente.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete(ParroquiaTenantHelper.CookieSuperAdminParroquia);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }
    }
}
