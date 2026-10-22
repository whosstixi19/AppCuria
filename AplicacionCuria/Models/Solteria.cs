namespace AplicacionCuria.Models
{
    public class Solteria
    {
        // Datos del Novio/a
        public string nombreNovio { get; set; }
        public string cedulaNovio { get; set; }
        public string domicilioNovio { get; set; }

        // Datos de la Pareja
        public string nombrePareja { get; set; }

        // Datos de los testigos
        public string testigo1 { get; set; }
        public string direccion1 { get; set; }
        public string cedula1 { get; set; }
        public string telefono1 { get; set; }

        public string testigo2 { get; set; }
        public string direccion2 { get; set; }
        public string cedula2 { get; set; }
        public string telefono2 { get; set; }

        // Parroquia y feligrés
        public string feligres { get; set; }
        public string parroquia { get; set; }

        // Impedimento (SI o NO)
        public string impedimento { get; set; }

        // Observación
        public string observacion { get; set; }
    }
}
