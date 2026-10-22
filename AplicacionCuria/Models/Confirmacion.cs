using System.ComponentModel.DataAnnotations;

namespace AplicacionCuria.Models
{
    public class Confirmacion
    {
        public int id { get; set; }

        [Required]
        public int alumno_id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(255, ErrorMessage = "El nombre no puede exceder los 255 caracteres.")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(255, ErrorMessage = "El apellido no puede exceder los 255 caracteres.")]
        public string apellido { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        public DateTime fechanacimiento { get; set; }

        [Required(ErrorMessage = "La fecha de confirmación es obligatoria.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "La 'Cédula' no cumple con el formato.")]
        public string cedula { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(255, ErrorMessage = "La dirección no puede exceder los 255 caracteres.")]
        public string direccion { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "El 'Teléfono' debe tener 10 dígitos.")]
        public string telefono { get; set; }

        [Required(ErrorMessage = "La fecha de confirmación es obligatoria.")]
        public DateTime fechaconfirmacion { get; set; }

        [Required(ErrorMessage = "La parroquia es obligatoria.")]
        public int parroquia_id { get; set; }

        [Required]
        public int sacerdote_id { get; set; }

        [Required]
        [StringLength(20)]
        public string estado { get; set; } = "ACTIVO";

        public string? observaciones { get; set; }
    }
}