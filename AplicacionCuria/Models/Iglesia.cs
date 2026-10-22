using System.ComponentModel.DataAnnotations;

namespace AplicacionCuria.Models
{
    public class Iglesia
    {
        public int id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(255)]
        public string nombre { get; set; }

        [StringLength(255)]
        public string direccion { get; set; }

        [StringLength(255)]
        public string ciudad { get; set; }

        [StringLength(20)]
        public string telefono { get; set; }

        [StringLength(255)]
        [EmailAddress(ErrorMessage = "Correo electrónico no válido")]
        public string email { get; set; }

        public int? ministro_id { get; set; }

        public int? parroquia_id { get; set; }

        [StringLength(255)]
        public string denominacion { get; set; }

        [StringLength(20)]
        public string estado { get; set; } = "ACTIVO";

        public string observaciones { get; set; }
    }
}