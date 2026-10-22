using System;

namespace AplicacionCuria.Models
{
    public class Usuario
    {
        public int id { get; set; }
        public string correo { get; set; }
        public string pass { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string rol { get; set; }
        public string estado { get; set; }
        public DateTime? ultimo_acceso { get; set; }
        public DateTime fecha_creacion { get; set; }
        public string salt { get; set; }
        public int iterations { get; set; }
    }
}
