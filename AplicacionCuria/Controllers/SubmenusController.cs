using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    public class SubmenusController : Controller
    {

        /// <summary>
        /// Ventadas del menu de Sacramentos
        /// </summary>
        /// <returns></returns>
        public IActionResult BautizadosView()
        {
            return View("BautizadosView");
        }

        public IActionResult ConfirmacionView()
        {
            return View();
        }

        public IActionResult MatrimoniosView()
        {
            return View();
        }

        public IActionResult PrimeraComunionView()
        {
            return View();
        }

        /// <summary>
        /// Ventanas de el menu de Certificados
        /// </summary>
        /// <returns></returns>

        public IActionResult PaseDeNivelView()
        {
            return View("PaseDeNivel");
        }
        public IActionResult PrimeraComunionCView()
        {
            return View();
        }
        public IActionResult ConfirmacionCView()
        {
            return View();
        }
        public IActionResult DispensasCView()
        {
            return View();
        }
        public IActionResult BautizosCView()
        {
            return View();
        }
        public IActionResult MatrimoniosCView()
        {
            return View();
        }

        // Menu Registros
        public IActionResult MinistrosCatequistasView()
        {
            return View("MinistrosCatequistasView");
        }
        public IActionResult PeriodosView()
        {
            return View("PeriodosView");
        }
        public IActionResult SacerdoteView()
        {
            return View("SacerdoteView");
        }
        public IActionResult ParroquiaView()
        {
            return View("ParroquiaView");
        }
        public IActionResult IglesiaView()
        {
            return View("IglesiaView");
        }
        public IActionResult MisaView()
        {
            return View("MisaView");
        }

        public IActionResult FeligresesView()
        {
            ViewData["ActiveMenu"] = "Feligreses";
            return View();
        }

        public IActionResult EncargadosView()
        {
            ViewData["ActiveMenu"] = "Encargados";
            return View();
        }

    }

}
