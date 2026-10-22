using System.ComponentModel.DataAnnotations.Schema;
using AplicacionCuria.Models;


namespace AplicacionCuria.Models
{
    public class CursilloPrebautismalImprimir
    {
        public int id { get; set; }

        // Datos del niño/niña
        public string nombrecompletonino { get; set; }  // Nombres y apellidos combinados

        // Datos del padre, madre, padrino y madrina
        public string nombrepadre { get; set; }
        public string nombremadre { get; set; }
        public string nombrepadrino { get; set; }
        public string nombremadrina { get; set; }

        // Quien inscribe y contactos
        public string quieninscribe { get; set; }
        public string telefono { get; set; }
        public string celular { get; set; }

        // Fechas importantes
        public DateTime fechabautizo { get; set; }
        public DateTime fechacursillo { get; set; }

        public string? nombreSacerdote { get; set; }
        public string? firmaSacerdote { get; set; }

        // Estado del cursillo
        public string estado { get; set; }

        public string parroquia { get; set; }

        public string TipoRelacion { get; set; } = "padre y madre";
        
        public string EtiquetaPadre { get; set; } = "Padre";
        public string EtiquetaMadre { get; set; } = "Madre";


    }


}
