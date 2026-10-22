using AplicacionCuria.Conexion;
using AplicacionCuria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;

namespace AplicacionCuria.Controllers
{
    [Authorize]
    public class CertificadosController(ConexionPostgreSQL sql) : Controller
    {
        [HttpGet]
        public IActionResult DescargarPdf(string fileName)
        {
            var filePath = Path.Combine(Path.GetTempPath(), fileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound("Archivo no encontrado.");

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/pdf", fileName);
        }

        #region pasedenivel

        public async Task<IActionResult> Imprimir(int? alumnoid, int? sacerdoteId = null, bool guardar = false)
        {
            if (alumnoid == null)
                return BadRequest(new { success = false, message = "No se ha seleccionado un alumno." });

            var alumno = await sql.alumnoscatequesis.FindAsync(alumnoid);
            if (alumno == null)
                return NotFound(new { success = false, message = "No se ha encontrado al alumno." });

            // Obtener catequistas
            var catequista1 = await sql.catequistas.FindAsync(alumno.catequista_id);
            var catequista2 = alumno.GetType().GetProperty("catequista_id2")?.GetValue(alumno) is int id2
                ? await sql.catequistas.FindAsync(id2)
                : null;

            // Obtener sacerdote (si fue seleccionado)
            Sacerdote? sacerdote = null;
            if (sacerdoteId.HasValue && sacerdoteId.Value > 0)
            {
                sacerdote = await sql.sacerdotes.FindAsync(sacerdoteId.Value);
                if (sacerdote == null)
                {
                    return NotFound(new { success = false, message = "El sacerdote seleccionado no existe." });
                }
            }

            // Crear ViewModel
            var model = new CertificadoViewModel
            {
                Nombre = alumno.nombre,
                Apellido = alumno.apellido,
                Nivel = alumno.nivel,
                Paralelo = alumno.GetType().GetProperty("paralelo")?.GetValue(alumno, null)?.ToString() ?? "N/A",
                Catequista1 = catequista1 != null ? $"{catequista1.nombre} {catequista1.apellido}" : "No asignado",
                Catequista2 = catequista2 != null ? $"{catequista2.nombre} {catequista2.apellido}" : "",
                Sacerdote = sacerdote != null ? $"{sacerdote.nombre} {sacerdote.apellido}" : "_________________",
                FirmaSacerdote = sacerdote?.firma
            };

            // Log para depuración (opcional)
            Console.WriteLine($"Sacerdote seleccionado: {model.Sacerdote}, Firma: {(model.FirmaSacerdote != null ? "Sí" : "No")}");

            // Generar PDF
            var pdf = new ViewAsPdf("CertificadoPaseDeNivel", model)
            {
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };

            var pdfBytes = await pdf.BuildFile(ControllerContext);

            // Guardar PDF temporalmente
            var pdfName = $"Certificado_PaseDeNivel_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            var pdfPath = Path.Combine(Path.GetTempPath(), pdfName);
            await System.IO.File.WriteAllBytesAsync(pdfPath, pdfBytes);

            // Enviar a la impresora
            var printer = new Services.PrintService();
            printer.PrintPdf(pdfPath);

            return Ok(new { success = true, message = "Certificado enviado a la impresora.", file = pdfName });
        }


        #region listados
        public async Task<IActionResult> Search()
        {
            var resultados = await sql.alumnoscatequesis.ToListAsync();
            if (resultados.Count > 0)
                return Json(resultados); // Envía los resultados a la vista

            return Json(resultados);
        }
        [HttpGet]
        public IActionResult SearchByName(string nombrecompleto)
        {
            var resultados = sql.alumnoscatequesis.Where(a => a.nombre.ToLower().Contains(nombrecompleto.ToLower()) || a.apellido.ToLower().Contains(nombrecompleto.ToLower())).ToList();

            return Json(resultados);
        }
        #endregion

        [HttpGet]
        public async Task<IActionResult> SearchByNamePaseNivel(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return Json(new List<object>());

            nombre = nombre.ToLower();

            var resultados = await (from a in sql.alumnoscatequesis
                                    where a.estado == "ACTIVO" &&
                                        (a.nombre.ToLower().Contains(nombre) || a.apellido.ToLower().Contains(nombre))
                                    select new
                                    {
                                        a.id,
                                        a.nombre,
                                        a.apellido,
                                        a.cedula,
                                        a.nivel,
                                        a.telefono,
                                        a.fechanacimiento,
                                        a.direccion,
                                        a.foto
                                    }).ToListAsync();

            return Json(resultados);
        }

        #endregion

        #region primeracomunion

        public async Task<IActionResult> ImprimirPrimeraComunion(int? alumnoid, bool guardar = false)
        {
            if (alumnoid == null)
                return BadRequest(new { success = false, message = "No se ha seleccionado un alumno." });

            // 1. Buscar datos del alumno
            PrimeraComunion? result = await sql.primerascomuniones.FindAsync(alumnoid);
            if (result == null)
                return NotFound(new { success = false, message = "No se ha encontrado al alumno." });

            // 2. Buscar sacerdote asociado
            Sacerdote? sacerdote = await sql.sacerdotes.FindAsync(result.sacerdote_id);
            string nombreSacerdote = sacerdote != null ? $"{sacerdote.nombre} {sacerdote.apellido}" : "Sacerdote no registrado";
            string firmaSacerdote = sacerdote?.firma != null ? Convert.ToBase64String(sacerdote.firma) : null;

            // 3. Crear modelo combinado
            var modelo = new PrimeraComunionImprimir
            {
                Alumno = result,
                NombreSacerdote = nombreSacerdote,
                FirmaSacerdote = firmaSacerdote
            };

            // 4. Generar el PDF con Rotativa
            var pdf = new ViewAsPdf("CertificadoPrimeraComunion", modelo)
            {
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };

            var pdfBytes = await pdf.BuildFile(ControllerContext);

            // 5. Guardar temporalmente el archivo PDF
            var pdfName = $"Certificado_PrimeraComunion_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            var pdfPath = Path.Combine(Path.GetTempPath(), pdfName);
            await System.IO.File.WriteAllBytesAsync(pdfPath, pdfBytes);

            // 6. Enviar a la impresora
            var printer = new Services.PrintService();
            printer.PrintPdf(pdfPath);

            // 7. Responder con JSON (igual que Pase de Nivel)
            return Ok(new { success = true, message = "Certificado de Primera Comunión enviado a la impresora.", file = pdfName });
        }

        #region listados
        [HttpGet]
        public async Task<IActionResult> SearchPrimeraComunion(string? nombre)
        {
            var query = from pc in sql.primerascomuniones
                        join p in sql.parroquias on pc.parroquia_id equals p.id
                        select new
                        {
                            pc.id,
                            pc.nombre,
                            pc.apellido,
                            pc.telefono,
                            pc.fechanacimiento,
                            parroquia = p.nombre,
                            pc.cedula,
                            pc.direccion
                        };

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                nombre = nombre.ToLower();
                query = query.Where(x => x.nombre.ToLower().Contains(nombre) || x.apellido.ToLower().Contains(nombre));
            }

            var resultados = await query.ToListAsync();
            return Json(resultados);
        }

        [HttpGet]
        public IActionResult SearchPByName(string nombrecompleto)
        {
            var resultados = sql.primerascomuniones.Where(a => a.nombre.ToLower().Contains(nombrecompleto.ToLower()) || a.apellido.ToLower().Contains(nombrecompleto.ToLower())).ToList();

            return Json(resultados);
        }
        #endregion

        #endregion

        #region confirmacion

       public async Task<IActionResult> ImprimirConfirmacion(int? alumnoid)
        {
            if (alumnoid == null)
                return BadRequest(new { success = false, message = "No se ha seleccionado un alumno." });

            // 1. Buscar el registro de confirmación
            Confirmacion? result = await sql.confirmaciones.FindAsync(alumnoid);
            if (result == null)
                return NotFound(new { success = false, message = "No se ha encontrado al alumno." });

            // 2. Buscar el sacerdote asociado
            var sacerdote = await sql.sacerdotes.FindAsync(result.sacerdote_id);
            string nombreSacerdote = sacerdote != null ? $"{sacerdote.nombre} {sacerdote.apellido}" : "Sacerdote no registrado";
            string firmaSacerdote = sacerdote?.firma != null ? Convert.ToBase64String(sacerdote.firma) : null;

            // 3. Crear modelo de impresión
            var modelo = new ConfirmacionImprimir
            {
                Alumno = result,
                NombreSacerdote = nombreSacerdote,
                FirmaSacerdote = firmaSacerdote
            };

            // 4. Generar el PDF con Rotativa
            var pdf = new ViewAsPdf("CertificadoConfirmacion", modelo)
            {
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };

            var pdfBytes = await pdf.BuildFile(ControllerContext);

            // 5. Guardar PDF temporalmente
            var pdfName = $"Certificado_Confirmacion_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            var pdfPath = Path.Combine(Path.GetTempPath(), pdfName);
            await System.IO.File.WriteAllBytesAsync(pdfPath, pdfBytes);

            // 6. Enviar a la impresora (o simulación)
            var printer = new Services.PrintService();
            printer.PrintPdf(pdfPath);

            // 7. Retornar JSON
            return Ok(new { success = true, message = "Certificado de Confirmación enviado a la impresora.", file = pdfName });
        }


        [HttpGet]
        public async Task<IActionResult> SearchConfirmacion(string? nombre)
        {
            var query = from c in sql.confirmaciones
                        join p in sql.parroquias on c.parroquia_id equals p.id
                        select new
                        {
                            c.id,
                            c.nombre,
                            c.apellido,
                            c.telefono,
                            c.fechanacimiento,
                            parroquia = p.nombre,
                            c.cedula
                        };

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                nombre = nombre.ToLower();
                query = query.Where(x =>
                    x.nombre.ToLower().Contains(nombre) ||
                    x.apellido.ToLower().Contains(nombre));
            }

            var resultados = await query.ToListAsync();
            return Json(resultados);
        }

        #region listados
        public async Task<IActionResult> SearchConfirmacion()
        {
            var resultados = await sql.confirmaciones.ToListAsync();
            if (resultados.Count > 0)
                return Json(resultados); // Envía los resultados a la vista

            return Json(resultados);
        }
        [HttpGet]
        public IActionResult SearchCByName(string nombrecompleto)
        {
            var resultados = sql.confirmaciones.Where(a => a.nombre.ToLower().Contains(nombrecompleto.ToLower()) || a.apellido.ToLower().Contains(nombrecompleto.ToLower())).ToList();

            return Json(resultados);
        }
        #endregion


        #endregion

        #region dispensas

       public async Task<IActionResult> ImprimirCertificadoSolteria(Solteria s, bool guardar = false)
        {
            if (s == null || string.IsNullOrEmpty(s.parroquia))
                return BadRequest(new { success = false, message = "Datos del certificado de soltería inválidos." });

            // 1. Obtener datos de la parroquia
            Parroquia parroquia = await sql.parroquias.FindAsync(int.Parse(s.parroquia));
            if (parroquia == null)
                return NotFound(new { success = false, message = "No se ha encontrado la parroquia." });

            s.parroquia = parroquia.nombre;

            // 2. Generar PDF con Rotativa
            var pdf = new ViewAsPdf("CertificadoSolteria", s)
            {
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };

            var pdfBytes = await pdf.BuildFile(ControllerContext);

            // 3. Guardar PDF temporalmente
            var pdfName = $"Certificado_Solteria_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            var pdfPath = Path.Combine(Path.GetTempPath(), pdfName);
            await System.IO.File.WriteAllBytesAsync(pdfPath, pdfBytes);

            // 4. Enviar a la impresora (o simulación)
            var printer = new Services.PrintService();
            printer.PrintPdf(pdfPath);

            // 5. Devolver confirmación en JSON
            return Ok(new { success = true, message = "Certificado de soltería enviado a la impresora.", file = pdfName });
        }

        #endregion

        #region bautizos

       [HttpGet("Certificados/ImprimirCertificadoBautismo")]
        public async Task<IActionResult> ImprimirCertificadoBautismo(int? alumnoid, int? sacerdoteId, string tipo = "padres")
        {
            if (alumnoid == null)
                return BadRequest(new { success = false, message = "No se ha seleccionado un alumno." });

            if (sacerdoteId == null)
                return BadRequest(new { success = false, message = "No se ha seleccionado un sacerdote." });

            // 1. Buscar sacerdote
            var sacerdote = await sql.sacerdotes.FindAsync(sacerdoteId);
            if (sacerdote == null)
                return NotFound(new { success = false, message = "No se ha encontrado el sacerdote." });

            // 2. Buscar cursillo de bautismo
            var cursillo = await sql.cursillosprebautismales
                .Include(c => c.Parroquia)
                .FirstOrDefaultAsync(c => c.id == alumnoid);

            if (cursillo == null)
                return NotFound(new { success = false, message = "No se ha encontrado el cursillo de bautismo." });

            // 3. Obtener nombres desde Feligreses (Padres o Padrinos)
            string nombre1 = "";
            string nombre2 = "";

            if (tipo.ToLower() == "padrinos")
            {
                nombre1 = await GetNombreFeligresPorCedula(cursillo.nombrepadrino);
                nombre2 = await GetNombreFeligresPorCedula(cursillo.nombremadrina);

                // Si no existe en feligreses, usar el valor tal cual
                nombre1 = string.IsNullOrEmpty(nombre1) ? cursillo.nombrepadrino : nombre1;
                nombre2 = string.IsNullOrEmpty(nombre2) ? cursillo.nombremadrina : nombre2;
            }
            else
            {
                nombre1 = await GetNombreFeligresPorCedula(cursillo.nombrepadre);
                nombre2 = await GetNombreFeligresPorCedula(cursillo.nombremadre);

                // Si no existe en feligreses, usar el valor tal cual
                nombre1 = string.IsNullOrEmpty(nombre1) ? cursillo.nombrepadre : nombre1;
                nombre2 = string.IsNullOrEmpty(nombre2) ? cursillo.nombremadre : nombre2;
            }

            // 4. Crear el modelo de impresión
            var nuevoCursillo = new CursilloPrebautismalImprimir
            {
                celular = cursillo.celular,
                estado = cursillo.estado,
                fechabautizo = cursillo.fechabautizo,
                fechacursillo = cursillo.fechacursillo,
                id = cursillo.id,
                nombrecompletonino = cursillo.nombrecompletonino,
                quieninscribe = cursillo.quieninscribe,
                telefono = cursillo.telefono,
                nombremadre = nombre2,
                nombrepadre = nombre1,
                parroquia = cursillo.Parroquia?.nombre ?? "Sin Parroquia",
                nombreSacerdote = $"{sacerdote.nombre} {sacerdote.apellido}",
                firmaSacerdote = sacerdote.firma != null ? Convert.ToBase64String(sacerdote.firma) : null,
                TipoRelacion = tipo.ToLower() == "padrinos" ? "padrino y madrina" : "padre y madre",
                EtiquetaPadre = tipo.ToLower() == "padrinos" ? "Padrino" : "Padre",
                EtiquetaMadre = tipo.ToLower() == "padrinos" ? "Madrina" : "Madre"
            };


            // 5. Según el tipo de certificado, asignar relación
            if (tipo.ToLower() == "padrinos")
            {
                nuevoCursillo.TipoRelacion = "padrino y madrina";
            }
            else
            {
                nuevoCursillo.TipoRelacion = "padre y madre";
            }

            // 5. Generar el PDF
            var pdf = new ViewAsPdf("CertificadoBautizmo", nuevoCursillo)
            {
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };

            var pdfBytes = await pdf.BuildFile(ControllerContext);

            // 6. Guardar temporalmente el PDF
            var pdfName = $"Certificado_Bautismo_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            var pdfPath = Path.Combine(Path.GetTempPath(), pdfName);
            await System.IO.File.WriteAllBytesAsync(pdfPath, pdfBytes);

            // 7. Enviar a la impresora
            var printer = new Services.PrintService();
            printer.PrintPdf(pdfPath);

            return Ok(new { success = true, message = $"Certificado de Bautismo ({tipo}) enviado a la impresora.", file = pdfName });
        }

        /// <summary>
        /// Busca el nombre completo de un feligrés por su cédula.
        /// </summary>
        private async Task<string> GetNombreFeligresPorCedula(string cedula)
        {
            if (string.IsNullOrEmpty(cedula))
                return null;

            var feligr = await sql.feligreses.FirstOrDefaultAsync(f => f.cedula == cedula);
            return feligr != null ? $"{feligr.nombre} {feligr.apellido}" : null;
        }


       public async Task<IActionResult> ImprimirCertificadoActaBautismo(int? alumnoid2, int? sacerdoteId)
        {
            if (alumnoid2 == null)
                return BadRequest(new { success = false, message = "No se ha seleccionado un alumno." });

            if (sacerdoteId == null)
                return BadRequest(new { success = false, message = "No se ha seleccionado un sacerdote." });

            // 1. Buscar acta de bautismo
            var acta = await sql.actasbautismo.FindAsync(alumnoid2);
            if (acta == null)
                return NotFound(new { success = false, message = "No se ha encontrado el acta de bautismo." });

            // 2. Buscar sacerdote
            var sacerdote = await sql.sacerdotes.FindAsync(sacerdoteId);
            if (sacerdote == null)
                return NotFound(new { success = false, message = "No se ha encontrado el sacerdote." });

            // 3. Obtener nombre de la iglesia
            var iglesiaNombre = await (from i in sql.iglesias
                                    where i.id == acta.iglesia_id
                                    select i.nombre)
                                    .FirstOrDefaultAsync();

            // 4. Construir modelo para la vista
            var modelo = new ActaBautismoImprimir
            {
                Acta = acta,
                NombreSacerdote = $"{sacerdote.nombre} {sacerdote.apellido}",
                FirmaSacerdote = sacerdote.firma != null ? Convert.ToBase64String(sacerdote.firma) : null,
                NombreIglesia = iglesiaNombre ?? "Iglesia no registrada"
            };

            // 5. Generar PDF con Rotativa
            var pdf = new ViewAsPdf("CertificadoActaBautismo", modelo)
            {
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };

            var pdfBytes = await pdf.BuildFile(ControllerContext);

            // 6. Guardar PDF temporalmente
            var pdfName = $"Certificado_Acta_Bautismo_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            var pdfPath = Path.Combine(Path.GetTempPath(), pdfName);
            await System.IO.File.WriteAllBytesAsync(pdfPath, pdfBytes);

            // 7. Enviar a la impresora
            var printer = new Services.PrintService();
            printer.PrintPdf(pdfPath);

            return Ok(new { success = true, message = "Certificado de Acta de Bautismo enviado a la impresora.", file = pdfName });
        }

        #endregion

        #region matrimonios

        public async Task<IActionResult> ImprimirCertificadoCursilloMatrimonio(int? cursilloid, int? sacerdoteId)
        {
            if (cursilloid == null)
                return BadRequest(new { success = false, message = "No se ha seleccionado un cursillo de matrimonio." });

            if (sacerdoteId == null)
                return BadRequest(new { success = false, message = "No se ha seleccionado un sacerdote." });

            // 1. Traer el cursillo con la parroquia asociada
            var cursillo = await sql.cursillosprematrimoniales
                .Include(c => c.Parroquia)
                .FirstOrDefaultAsync(c => c.id == cursilloid);

            if (cursillo == null)
                return NotFound(new { success = false, message = "No se ha encontrado el cursillo de matrimonio." });

            // 2. Buscar sacerdote
            var sacerdote = await sql.sacerdotes.FindAsync(sacerdoteId);
            if (sacerdote == null)
                return NotFound(new { success = false, message = "No se ha encontrado el sacerdote." });

            // 3. Construir modelo
            var modelo = new CursilloMatrimonioImprimir
            {
                Cursillo = cursillo,
                NombreSacerdote = $"{sacerdote.nombre} {sacerdote.apellido}",
                FirmaSacerdote = sacerdote.firma != null ? Convert.ToBase64String(sacerdote.firma) : null
            };

            // 4. Generar PDF con Rotativa
            var pdf = new ViewAsPdf("CertificadoCursilloMatrimonio", modelo)
            {
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };

            var pdfBytes = await pdf.BuildFile(ControllerContext);

            // 5. Guardar temporalmente el PDF
            var pdfName = $"Certificado_Cursillo_Matrimonio_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            var pdfPath = Path.Combine(Path.GetTempPath(), pdfName);
            await System.IO.File.WriteAllBytesAsync(pdfPath, pdfBytes);

            // 6. Enviar a la impresora (o simular)
            var printer = new Services.PrintService();
            printer.PrintPdf(pdfPath);

            // 7. Responder con JSON
            return Ok(new { success = true, message = "Certificado de cursillo de matrimonio enviado a la impresora.", file = pdfName });
        }


        public async Task<IActionResult> ImprimirCertificadoMatrimonio(int? matid, int? sacerdoteId, int? parroquiaId)
        {
            if (matid == null)
                return BadRequest(new { success = false, message = "No se ha seleccionado un matrimonio." });

            if (sacerdoteId == null)
                return BadRequest(new { success = false, message = "No se ha seleccionado un sacerdote." });

            if (parroquiaId == null)
                return BadRequest(new { success = false, message = "No se ha seleccionado una parroquia." });

            // 1. Buscar acta de matrimonio
            var actaMatrimonio = await sql.actasdematrimonio.FindAsync(matid);
            if (actaMatrimonio == null)
                return NotFound(new { success = false, message = "No se ha encontrado el acta de matrimonio." });

            // 2. Buscar acta pre-matrimonial
            ActaPreMatrimonial? cursillo = await sql.actasprematrimoniales.FindAsync(actaMatrimonio.acta_prematrimonial_id);
            if (cursillo == null)
                return NotFound(new { success = false, message = "No se ha encontrado el acta pre-matrimonial." });

            // 3. Datos de la iglesia
            var iglesiaData = await sql.iglesias
                .Where(i => i.id == cursillo.iglesia_id)
                .Select(i => new { i.nombre, i.direccion, i.ciudad, i.telefono, i.email })
                .FirstOrDefaultAsync();

            cursillo.observaciones = iglesiaData?.nombre ?? "IGLESIA DESCONOCIDA";
            cursillo.estado = iglesiaData != null
                ? $"{iglesiaData.nombre} - {iglesiaData.direccion}, {iglesiaData.ciudad}. " +
                $"Tel: {iglesiaData.telefono}, Email: {iglesiaData.email}"
                : "Datos de la iglesia no disponibles.";

            // 4. Buscar sacerdote
            var sacerdote = await sql.sacerdotes.FindAsync(sacerdoteId);
            if (sacerdote == null)
                return NotFound(new { success = false, message = "No se ha encontrado el sacerdote." });

            // 5. Buscar parroquia
            var parroquia = await sql.parroquias.FindAsync(parroquiaId);
            if (parroquia == null)
                return NotFound(new { success = false, message = "No se ha encontrado la parroquia." });

            // 6. Crear modelo
            var modelo = new CertificadoMatrimonioImprimir
            {
                Acta = cursillo,
                NombreSacerdote = $"{sacerdote.nombre} {sacerdote.apellido}",
                FirmaSacerdote = sacerdote.firma != null ? Convert.ToBase64String(sacerdote.firma) : null,
                NombreParroquia = parroquia.nombre
            };

            // 7. Generar PDF con Rotativa
            var pdf = new ViewAsPdf("CertificadoMatrimonio", modelo)
            {
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };

            var pdfBytes = await pdf.BuildFile(ControllerContext);

            // 8. Guardar PDF temporal
            var pdfName = $"Certificado_Matrimonio_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            var pdfPath = Path.Combine(Path.GetTempPath(), pdfName);
            await System.IO.File.WriteAllBytesAsync(pdfPath, pdfBytes);

            // 9. Imprimir (o simular impresión)
            var printer = new Services.PrintService();
            printer.PrintPdf(pdfPath);

            // 10. Responder
            return Ok(new { success = true, message = "Certificado de matrimonio enviado a la impresora.", file = pdfName });
        }

        [HttpGet("Certificados/ImprimirReportePDF")]
        public async Task<IActionResult> ImprimirReportePDF(int? id, int? sacerdoteId)
        {
            if (id == null)
                return BadRequest(new { success = false, message = "No se ha seleccionado un acta de matrimonio." });

            if (sacerdoteId == null)
                return BadRequest(new { success = false, message = "No se ha seleccionado un sacerdote." });

            // 1. Buscar acta de matrimonio
            var actaMatrimonio = await sql.actasdematrimonio.FindAsync(id);
            if (actaMatrimonio == null)
                return NotFound(new { success = false, message = "No se ha encontrado el acta de matrimonio." });

            // 2. Buscar acta pre-matrimonial
            var cursillo = await sql.actasprematrimoniales.FindAsync(actaMatrimonio.acta_prematrimonial_id);
            if (cursillo == null)
                return NotFound(new { success = false, message = "No se ha encontrado el acta pre-matrimonial." });

            // 3. Datos de la iglesia
            var iglesiaData = await sql.iglesias
                .Where(i => i.id == cursillo.iglesia_id)
                .Select(i => new { i.nombre, i.direccion, i.ciudad, i.telefono, i.email })
                .FirstOrDefaultAsync();

            cursillo.observaciones = iglesiaData?.nombre ?? "IGLESIA DESCONOCIDA";
            cursillo.estado = iglesiaData != null
                ? $"{iglesiaData.nombre} - {iglesiaData.direccion}, {iglesiaData.ciudad}. " +
                $"Tel: {iglesiaData.telefono}, Email: {iglesiaData.email}"
                : "Datos de la iglesia no disponibles.";

            // 4. Buscar sacerdote
            var sacerdote = await sql.sacerdotes.FindAsync(sacerdoteId);
            if (sacerdote == null)
                return NotFound(new { success = false, message = "No se ha encontrado el sacerdote." });

            // 5. Crear modelo
            var modelo = new CertificadoMatrimonioImprimir
            {
                Acta = cursillo,
                Actamatrimonio = actaMatrimonio ?? new ActaDeMatrimonio(),
                NombreSacerdote = $"{sacerdote.nombre} {sacerdote.apellido}",
                FirmaSacerdote = sacerdote.firma != null ? Convert.ToBase64String(sacerdote.firma) : null,
                NombreParroquia = iglesiaData?.nombre ?? "SIN PARROQUIA"
            };

            // 6. Generar PDF con Rotativa
            var pdf = new ViewAsPdf("ReporteMatrimonio", modelo)
            {
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };

            var pdfBytes = await pdf.BuildFile(ControllerContext);

            // 7. Guardar temporalmente el PDF
            var pdfName = $"Reporte_Acta_Matrimonio_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            var pdfPath = Path.Combine(Path.GetTempPath(), pdfName);
            await System.IO.File.WriteAllBytesAsync(pdfPath, pdfBytes);

            // 8. Imprimir (o simular)
            var printer = new Services.PrintService();
            printer.PrintPdf(pdfPath);

            // 9. Responder
            return Ok(new { success = true, message = "Reporte de acta de matrimonio enviado a la impresora.", file = pdfName });
        }

        #endregion
        [HttpGet]
        public IActionResult GetSacerdotes()
        {
            var sacerdotes = sql.sacerdotes
                .Select(s => new
                {
                    id = s.id,
                    nombre = s.nombre + " " + s.apellido
                })
                .ToList();

            return Json(sacerdotes);
        }

    }
}
