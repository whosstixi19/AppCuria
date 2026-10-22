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

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, cliente.nombre),
                new(ClaimTypes.Email, cliente.correo),
                new(ClaimTypes.Role, cliente.rol)
            };
            
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UsuarioRegisterRequest request)
        {
            if (ModelState.IsValid)
            {
                var existingUser = sql.usuarios.FirstOrDefault(u => u.correo == request.Email);
                if (existingUser != null)
                {
                    ViewBag.Error = "El correo ya está en uso";
                    return View();
                }

                var hashedPassword = PasswordHelper.GenerateHash(request.Password);

                var newUser = new Usuario
                {
                    correo = request.Email,
                    nombre = request.Nombre,
                    apellido = request.Apellido,
                    pass = hashedPassword.Password ?? string.Empty,
                    salt = hashedPassword.Salt ?? string.Empty,
                    iterations = hashedPassword.Iterations,
                    rol = "USUARIO",
                    estado = "ACTIVO", // debe ser string según tu base
                    fecha_creacion = DateTime.UtcNow,
                    ultimo_acceso = null
                };

                sql.usuarios.Add(newUser);
                await sql.SaveChangesAsync();

                TempData["SuccessMessage"] = "Registro exitoso. Ya puedes iniciar sesión.";
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
