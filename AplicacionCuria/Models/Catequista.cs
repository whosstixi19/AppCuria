using System;
using System.ComponentModel.DataAnnotations;

namespace AplicacionCuria.Models
{
    public class Catequista
    {
        [Key]
        public int id { get; set; }

        [Required]
        [StringLength(255)]
        public string nombre { get; set; }

        [Required]
        [StringLength(255)]
        public string apellido { get; set; }

        [StringLength(20)]
        public string cedula { get; set; }

        [StringLength(20)]
        public string telefono { get; set; }

        public string direccion { get; set; }

        [StringLength(255)]
        public string email { get; set; }

        public DateTime? fecha_nacimiento { get; set; }

        [StringLength(20)]
        public string estado { get; set; } = "ACTIVO";

        public string observaciones { get; set; }
    }
}