using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    public class MatrimonioController(ConexionPostgreSQL sql) : Controller
    {
        public IActionResult Consanguinidad()
        {
            try
            {
                ViewBag.Feligreses = sql.feligreses.ToList();
                ViewBag.Parroquias = sql.parroquias.ToList();
                return View("~/Views/Shared/Consanguinidad.cshtml");
            }
            catch (Exception)
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult ImprimirConsanguinidad(Consanguinidad modelo)
        {
            string fecha = DateTime.Now.ToString("dd/MMMM/yyyy");
            string hora = DateTime.Now.TimeOfDay.ToString();
            return new ViewAsPdf("ImprimirConsanguinidad", modelo)
            {
                FileName = $"Consanguinidad-{fecha}-{hora}.pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }


        [HttpPost]
        public IActionResult ImprimirReligionMixta(MixtaReligion modelo)
        {
            string fecha = DateTime.Now.ToString("dd/MMMM/yyyy");
            string hora = DateTime.Now.TimeOfDay.ToString();
            return new ViewAsPdf("ImprimirReporteMixtaReligion", modelo)
            {
                FileName = $"Religion-Mixta-{fecha}-{hora}.pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }

        [HttpPost]
        public IActionResult ImprimirDisponibilidadCultos(DisponibilidadCultos modelo)
        {
            string fecha = DateTime.Now.ToString("dd/MMMM/yyyy");
            string hora = DateTime.Now.TimeOfDay.ToString();
            return new ViewAsPdf("ImprimirReporteCultos", modelo)
            {
                FileName = $"Cultos-{fecha}-{hora}.pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }


        public IActionResult ReligionMixta()
        {
            try
            {
                ViewBag.Feligreses = sql.feligreses.ToList();
                ViewBag.Parroquias = sql.parroquias.ToList();
                return View("~/Views/Shared/ReligionMixtaView.cshtml");
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        public IActionResult Cultos()
        {
            try
            {
                ViewBag.Feligreses = sql.feligreses.ToList();
                ViewBag.Parroquias = sql.parroquias.ToList();
                return View("~/Views/Shared/CultosView.cshtml");
            }
            catch (Exception ex)
            {
                return View();
            }
        }
    }
}
