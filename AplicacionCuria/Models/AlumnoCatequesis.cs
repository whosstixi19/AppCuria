using System.ComponentModel.DataAnnotations;

namespace AplicacionCuria.Models
{
    public class AlumnoCatequesis
    {
        public int id { get; set; }

        [StringLength(250, ErrorMessage = "El 'Nombre' no puede exceder los 50 caracteres.")]
        [Required(ErrorMessage = "El campo 'Nombre' es obligatorio.")]
        public string nombre { get; set; }

        [StringLength(250, ErrorMessage = "El 'Apellido' no puede exceder los 50 caracteres.")]
        [Required(ErrorMessage = "El campo 'Apellido' es obligatorio.")]
        public string apellido { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        public DateTime fechanacimiento { get; set; }

        [Required(ErrorMessage = "La fecha de bautizo es obligatoria.")]
        public DateTime fechabautizo { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "La 'Cédula' no cumple con el formato.")]
        public string cedula { get; set; }

        [StringLength(50, ErrorMessage = "La 'Dirección' no puede exceder los 50 caracteres.")]
        [Required(ErrorMessage = "El campo 'Dirección' es obligatorio.")]
        public string direccion { get; set; }

        [StringLength(50, ErrorMessage = "El 'Teléfono' no puede exceder los 50 caracteres.")]
        [Required(ErrorMessage = "El campo 'Teléfono' es obligatorio.")]
        public string telefono { get; set; }

        public byte[]? foto { get; set; }

        [StringLength(50, ErrorMessage = "La 'Nacionalidad' no puede exceder los 50 caracteres.")]
        [Required(ErrorMessage = "El campo 'Nacionalidad' es obligatorio.")]
        public string nacionalidad { get; set; }

        [StringLength(50, ErrorMessage = "El 'Estado Civil' no puede exceder los 50 caracteres.")]
        [Required(ErrorMessage = "El campo 'Estado Civil' es obligatorio.")]
        public string estadocivil { get; set; }

        [EmailAddress(ErrorMessage = "El 'Correo' no es válido.")]
        public string correo { get; set; }

        // Datos representante
        [StringLength(250, ErrorMessage = "El 'Nombre' no puede exceder los 250 caracteres.")]
        [Required(ErrorMessage = "El campo 'Nombre' es obligatorio.")]
        public string nombrerepresentante { get; set; }

        [StringLength(50, ErrorMessage = "El 'Teléfono de representante' no puede exceder los 50 caracteres.")]
        [Required(ErrorMessage = "El campo 'Teléfono de representante' es obligatorio.")]
        public string telefonorepresentante { get; set; }

        [StringLength(50, ErrorMessage = "El 'Parentesco' no puede exceder los 50 caracteres.")]
        [Required(ErrorMessage = "El campo 'Parentesco' es obligatorio.")]
        public string relacionconelestudiante { get; set; }

        [Required(ErrorMessage = "El nivel es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nivel no puede superar los 50 caracteres.")]
        public string nivel { get; set; }

        [Required(ErrorMessage = "El catequista es obligatorio.")]
        public int catequista_id { get; set; }

        // Segundo catequista opcional
        public int? catequista_id2 { get; set; }

        // NUEVO CAMPO PARA EL PARALELO
        [StringLength(10, ErrorMessage = "El paralelo no puede superar los 10 caracteres.")]
        public string paralelo { get; set; }

        [Required(ErrorMessage = "El periodo es obligatorio.")]
        public int periodo_id { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [StringLength(20)]
        public string estado { get; set; } = "ACTIVO";

        [Required(ErrorMessage = "Las observaciones son obligatorias.")]
        public string observaciones { get; set; }
    }
}
