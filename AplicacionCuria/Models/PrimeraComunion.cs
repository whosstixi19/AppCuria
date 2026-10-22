using System.ComponentModel.DataAnnotations;

namespace AplicacionCuria.Models
{
    public class PrimeraComunion
    {
        public int id { get; set; }
        [Required(ErrorMessage = "El campo 'Nombre' es obligatorio.")]
        public string nombre { get; set; }
        [Required(ErrorMessage = "El campo 'Apellido' es obligatorio.")]
        public string apellido { get; set; }
        [Required(ErrorMessage = "El campo 'Fecha de Nacimiento' es obligatorio.")]
        public DateTime fechanacimiento { get; set; }
        [Required(ErrorMessage = "El campo 'Cédula' es obligatorio.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "La 'Cédula' no cumple con el formato.")]
        public string cedula { get; set; }

        [Required(ErrorMessage = "El campo 'Dirección' es obligatorio.")]
        [StringLength(250, ErrorMessage = "La 'Dirección' no puede exceder los 100 caracteres.")]
        public string direccion { get; set; }
        [Required(ErrorMessage = "El campo 'Teléfono' es obligatorio.")]
        [RegularExpression(@"^\d{8,12}$", ErrorMessage = "El 'Teléfono' debe tener entre 8 y 12 dígitos.")]
        public string telefono { get; set; }
        [Required(ErrorMessage = "El campo 'Fecha Primera Comunion' es obligatorio.")]
        public DateTime fechaprimeracomunion { get; set; }

        [Required]
        public int parroquia_id { get; set; }

        [Required]
        public int sacerdote_id { get; set; }

        [Required]
        [StringLength(20)]
        public string estado { get; set; } = "ACTIVO";

        [Required]
        public string observaciones { get; set; }
        
        [Required]
        public int alumno_id { get; set; }

    }
}