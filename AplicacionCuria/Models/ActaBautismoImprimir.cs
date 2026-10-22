using AplicacionCuria.Models;
using System;

namespace AplicacionCuria.Models
{
    public class ActaBautismoImprimir
    {
        public ActaBautismo Acta { get; set; }
        public string NombreSacerdote { get; set; }
        public string FirmaSacerdote { get; set; } // Base64 de la firma
        public string NombreIglesia { get; set; }

        // Nuevos campos para mostrar nombres en vez de cédulas
        public string NombrePadre { get; set; }
        public string NombreMadre { get; set; }
        public string NombrePadrino1 { get; set; }
        public string NombrePadrino2 { get; set; }
    }
}
