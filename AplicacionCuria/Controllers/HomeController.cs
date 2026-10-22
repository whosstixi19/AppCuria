using AplicacionCuria.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AplicacionCuria.Conexion;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using AplicacionCuria.Security;
using Microsoft.AspNetCore.Identity.Data;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    public class HomeController(ConexionPostgreSQL sql) : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ComponenteNuevo()
        {
            return View();
        }

        public IActionResult MenuCatequesis()
        {
            return View();
        }
        public IActionResult MenuSacramentos()
        {
            return View();
        }

        public IActionResult MenuCertificado()
        {
            return View();
        }

        public IActionResult MenuAgenda()
        {
            return View();
        }

        public IActionResult MenuAyudaSocial()
        {
            return View();
        }

        public IActionResult MenuRegistrar()
        {
            return View();
        }

        public IActionResult MenuPagos()
        {
            return View();
        }

        public IActionResult MenuAyuda()
        {
            return View();
        }

        //public IActionResult Login()
        //{
        //    return View();
        //}

        public IActionResult BautizadosView()
        {
            return View("~/Views/Submenus/BautizadosView.cshtml");
        }

        public IActionResult ConfirmacionView()
        {
            return View("~/Views/Submenus/ConfirmacionView.cshtml");
        }

        public IActionResult MatrimoniosView()
        {
            return View("~/Views/Submenus/MatrimoniosView.cshtml");
        }

        public IActionResult PrimeraComunionView()
        {
            return View("~/Views/Submenus/PrimeraComunionView.cshtml");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var cliente = sql.usuarios.FirstOrDefault(c => c.correo == request.Email);
            if (cliente == null)
            {
                return NotFound(new { Message = "Cliente no encontrado" });
            }
            var isPasswordValid = PasswordHelper.VerifyPassword(request.Password, cliente.salt, cliente.pass);

            if (!isPasswordValid)
            {
                return Unauthorized(new { Message = "Contraseña incorrecta" });
            }
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, cliente.correo)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ModifyCursilloView()
        {
            return View();
        }

        public IActionResult ModifyAntiguosView()
        {
            return View();
        }

        public IActionResult ModifyPrimeraComunionView()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("", "");
        }
    }
}
