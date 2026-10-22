using System.ComponentModel.DataAnnotations;

namespace AplicacionCuria.Models
{
    public class Parroquia
    {
        public int id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(255)]
        public string nombre { get; set; }
        [Required(ErrorMessage = "La dirección es obligatoria")]
        public string direccion { get; set; }
        [Required(ErrorMessage = "El número de telefono es obligatorio")]
        public string telefono { get; set; }

        [StringLength(255)]
        [EmailAddress(ErrorMessage = "Correo electrónico no válido")]
        public string email { get; set; }

        public int? sacerdote_id { get; set; }

        [StringLength(20)]
        public string estado { get; set; } = "ACTIVO";

        [Required(ErrorMessage = "Las observaciones son obligatorias")]
        public string observaciones { get; set; }
    }
}