using System;

namespace AplicacionCuria.Models
{
    public class ConfirmacionImprimir
    {
        // Datos del alumno que hizo la confirmación
        public Confirmacion Alumno { get; set; }

        // Nombre completo del sacerdote
        public string NombreSacerdote { get; set; }

        // Firma del sacerdote en Base64 (opcional)
        public string FirmaSacerdote { get; set; }
    }
}
