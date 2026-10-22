using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    public class AyudaController(ConexionPostgreSQL sql) : Controller
    {

        [HttpPost]
        public async Task<IActionResult> Create(AyudaSocial model, IFormFile foto)
        {
            try
            {
                if (model.especificardiscapacidad == null || model.especificardiscapacidad == "")
                {
                    model.especificardiscapacidad = "Ninguna";
                }

                if (model.porcentajediscapacidad == null)
                {
                    model.porcentajediscapacidad = 0;
                }

                if (model.carnetdiscapacidad == "false")
                {
                    model.carnetdiscapacidad = "No";
                }
                else
                {
                    model.carnetdiscapacidad = "Si";
                }


                if (model.discapacidad == "false")
                {
                    model.discapacidad = "No";
                }
                else
                {
                    model.discapacidad = "Si";
                }

                if (model.recibebdh == "false")
                {
                    model.recibebdh = "No";
                }
                else
                {
                    model.recibebdh = "Si";
                }

                if (model.jubilacion == "false")
                {
                    model.jubilacion = "No";
                }
                else
                {
                    model.jubilacion = "Si";
                }


                model.fechanacimiento = DateTime.SpecifyKind(model.fechanacimiento, DateTimeKind.Utc);
                model.fechavisita = DateTime.SpecifyKind(model.fechavisita, DateTimeKind.Utc);
                if (foto != null && foto.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await foto.CopyToAsync(ms);
                        model.foto = ms.ToArray();
                    }
                }

                if (model.foto != null && model.foto.Length > 0)
                {
                    model.FotoBase64 = Convert.ToBase64String(model.foto);
                }
                string fecha = DateTime.Now.ToString("dd/MMMM/yyyy");
                string hora = DateTime.Now.TimeOfDay.ToString();


                //Se debera conversar si es que se debe guardar el reporte en la base de datos
                //sql.Add(model);
                //await sql.SaveChangesAsync();
                return new ViewAsPdf("ReporteDatosBeneficiario", model)
                {
                    FileName = $"Reporte Datos Beneficiario-{fecha}-{hora}.pdf",
                    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait, // ← aquí el cambio
                    PageSize = Rotativa.AspNetCore.Options.Size.A4,
                    PageMargins = new Rotativa.AspNetCore.Options.Margins(10, 10, 10, 10) // opcional
                };

            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });

            }

        }
    }
}
