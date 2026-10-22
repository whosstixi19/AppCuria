using PdfiumPrinter;
using System;
using System.IO;
using System.Linq;
using System.Drawing.Printing;

namespace AplicacionCuria.Services
{
    public class PrintService
    {
        public void PrintPdf(string pdfPath, string printerName = null)
        {
            if (!File.Exists(pdfPath))
                throw new FileNotFoundException($"No se encontró el archivo: {pdfPath}");

            try
            {
                // Comprobar si existe la impresora especificada
                bool impresoraDisponible = false;
                if (!string.IsNullOrEmpty(printerName))
                {
                    impresoraDisponible = PrinterSettings.InstalledPrinters
                        .Cast<string>()
                        .Any(p => p.Equals(printerName, StringComparison.OrdinalIgnoreCase));
                }

                if (impresoraDisponible)
                {
                    var printer = new PdfPrinter();
                    printer.Settings.PrinterName = printerName;
                    printer.Print(pdfPath);

                    Console.WriteLine($"PDF enviado a la impresora '{printerName}'");
                }
                else
                {
                    Console.WriteLine($"⚠ No se encontró la impresora '{printerName}'. Mostrando el PDF...");
                    AbrirPdf(pdfPath); // Si no hay impresora, abre el PDF
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al intentar imprimir: {ex.Message}");
                AbrirPdf(pdfPath); // Fallback en caso de error
            }
        }

        private void AbrirPdf(string pdfPath)
        {
            try
            {
                // Abre el PDF con el visor por defecto del sistema
                var process = new System.Diagnostics.Process();
                process.StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = pdfPath,
                    UseShellExecute = true
                };
                process.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ No se pudo abrir el PDF: {ex.Message}");
            }
        }
    }
}
