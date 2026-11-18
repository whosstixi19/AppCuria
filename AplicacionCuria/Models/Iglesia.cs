using System.ComponentModel.DataAnnotations;

namespace AplicacionCuria.Models
{
    public class Iglesia
    {
        public int id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(255)]
        public string nombre { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(255)]
        public string direccion { get; set; }

        [Required(ErrorMessage = "La ciudad es obligatoria")]
        [StringLength(255)]
        public string ciudad { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [StringLength(20)]
        public string telefono { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [StringLength(255)]
        [EmailAddress(ErrorMessage = "Correo electrónico no válido")]
        public string email { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un ministro")]
        public int? ministro_id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una parroquia")]
        public int? parroquia_id { get; set; }

        [Required(ErrorMessage = "La denominación es obligatoria")]
        [StringLength(255)]
        public string denominacion { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [StringLength(20)]
        public string estado { get; set; } = "ACTIVO";

        public string? observaciones { get; set; }
    }
}