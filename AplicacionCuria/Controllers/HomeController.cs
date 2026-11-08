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

        [HttpPost]
        public IActionResult CambiarParroquiaActiva(int? id, string? returnUrl)
        {
            if (ParroquiaTenantHelper.EsSuperAdmin(User))
            {
                if (id.HasValue && id.Value > 0)
                {
                    Response.Cookies.Append(ParroquiaTenantHelper.CookieSuperAdminParroquia, id.Value.ToString(), new CookieOptions
                    {
                        HttpOnly = true,
                        Expires = DateTimeOffset.UtcNow.AddDays(7),
                        SameSite = SameSiteMode.Lax
                    });
                }
                else
                {
                    Response.Cookies.Delete(ParroquiaTenantHelper.CookieSuperAdminParroquia);
                }
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

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
            return RedirectToAction("BautizadosView", "Submenus");
        }

        public IActionResult MenuCertificado()
        {
            return RedirectToAction("BautizosCView", "Submenus");
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
            return RedirectToAction("FeligresesView", "Submenus");
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
    }
}
