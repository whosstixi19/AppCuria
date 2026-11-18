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

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [StringLength(20)]
        public string telefono { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [StringLength(255)]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        public string email { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un sacerdote (registre uno primero si no existe)")]
        public int? sacerdote_id { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [StringLength(20)]
        public string estado { get; set; } = "ACTIVO";

        public string? observaciones { get; set; }

        public string? logo_base64 { get; set; }
    }
}