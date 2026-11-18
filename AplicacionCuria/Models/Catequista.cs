using System;
using System.ComponentModel.DataAnnotations;

namespace AplicacionCuria.Models
{
    public class Catequista
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(255)]
        public string nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(255)]
        public string apellido { get; set; }

        [Required(ErrorMessage = "La cédula es obligatoria")]
        [StringLength(20)]
        public string cedula { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [StringLength(20)]
        public string telefono { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        public string direccion { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [StringLength(255)]
        public string email { get; set; }

        public DateTime? fecha_nacimiento { get; set; }

        [StringLength(20)]
        public string estado { get; set; } = "ACTIVO";

        public string? observaciones { get; set; }
    }
}