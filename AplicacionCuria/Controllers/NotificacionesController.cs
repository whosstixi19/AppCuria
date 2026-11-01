using AplicacionCuria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class NotificacionesController(RecordatorioService recordatorios) : Controller
    {
        /// <summary>
        /// Envía recordatorios de agendas que cumplen la fecha actual.
        /// Requiere sesión iniciada.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> EnviarRecordatorios()
        {
            try
            {
                var enviados = await recordatorios.EnviarRecordatoriosHoyAsync();
                return Json(new { enviado = true, total = enviados });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] EnviarRecordatorios: {ex.Message}");
                return Json(new { enviado = false, error = "Error al enviar los recordatorios." });
            }
        }
    }
}
