using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AplicacionCuria.Models
{
    public class Antiguos
    {
        public int id { get; set; }

        // Información personal del bautizado
        [StringLength(50, ErrorMessage = "El Nombre' no puede exceder los 50 caracteres.")]
        [Required(ErrorMessage = "El campo 'Nombre' es obligatorio.")]
        [Display(Name = "Nombre Completo")]
        public string nombrecompleto { get; set; }
        
        [Display(Name = "Lugar de Nacimiento")]
        [Required(ErrorMessage = "El campo 'Lugar de Nacimiento' es obligatorio.")]
        [StringLength(250, ErrorMessage = "El 'Lugar de Nacimiento' no puede exceder los 250 caracteres.")]
        public string lugarnacimiento { get; set; }
        [Display(Name = "Cédula")]
        [Required(ErrorMessage = "El campo 'Cédula' es obligatorio.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "La 'Cédula' no cumple con el formato.")]
        public string cedula { get; set; }
        [Display(Name = "Sexo")]
        [Required(ErrorMessage = "El campo 'Sexo' es obligatorio.")]
        [RegularExpression(@"^(Masculino|Femenino)$", ErrorMessage = "El 'Sexo' debe ser 'Masculino' o 'Femenino'.")]
        public string sexo { get; set; }

        [Required]
        public DateTime fechanacimiento { get; set; }

        [Required]
        public DateTime fechabautizo { get; set; }

        [Required(ErrorMessage = "El campo 'Nombre del Padre' es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre del padre no puede exceder los 100 caracteres.")]
        public string nombrepadre { get; set; }

        [Required(ErrorMessage = "El campo 'Nombre de la Madre' es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre de la madre no puede exceder los 100 caracteres.")]
        public string nombremadre { get; set; }

        [Required(ErrorMessage = "El campo 'Cédula Padrino' es obligatorio.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "La 'Cédula Padrino' no cumple con el formato.")]
        public string cedulapadrino1 { get; set; }
        [Required(ErrorMessage = "El campo 'Cédula Madrina' es obligatorio.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "La 'Cédula Madrina' no cumple con el formato.")]
        public string cedulapadrino2 { get; set; }
        [RegularExpression(@"^\d{10}$", ErrorMessage = "La 'Cédula Padrino 3' no cumple con el formato.")]
        [DefaultValue("999999999")]
        public string? cedulapadrino3 { get; set; }  = "999999999";
        [RegularExpression(@"^\d{10}$", ErrorMessage = "La 'Cédula Padrino 4' no cumple con el formato.")]
        [DefaultValue("999999999")]
        public string? cedulapadrino4 { get; set; }  = "999999999";
        [RegularExpression(@"^\d{10}$", ErrorMessage = "La 'Cédula Padrino 5' no cumple con el formato.")]
        [DefaultValue("999999999")]
        public string? cedulapadrino5 { get; set; }  = "999999999";
      // Datos del acta
        [StringLength(50, ErrorMessage = "El 'Página' no puede exceder los 50 caracteres.")]
        public string pagina { get; set; }
        [StringLength(50, ErrorMessage = "El 'Tomo' no puede exceder los 50 caracteres.")]
        public string tomo { get; set; }
        [StringLength(50, ErrorMessage = "El 'Libro' no puede exceder los 50 caracteres.")]
        public string acta { get; set; }

        // Información religiosa
        //public int ministro { get; set; }  // Sacerdote o ministro
        //public int iglesia { get; set; }
        //public int feligreses { get; set; }

        // Registro civil
        [StringLength(4, ErrorMessage = "El 'Año de Registro Civil' no puede exceder los 4 caracteres.")]
        public string registrocivilano { get; set; }

        [Required]
        [StringLength(50)]
        public string registrociviltomo { get; set; }

        [Required]
        [StringLength(50)]
        public string registrocivilfolio { get; set; }

        [Required]
        [StringLength(50)]
        public string registrocivilacta { get; set; }

        [Required]
        public DateTime registrocivilfecha { get; set; }

        [Required]
        public string observaciones { get; set; }

        [Required]
        public int ministro_id { get; set; }

        [Required]
        public int iglesia_id { get; set; }

        [Required]
        public int parroquia_id { get; set; }

        [Required]
        public int sacerdote_id { get; set; }

        [Required]
        [StringLength(20)]
        public string estado { get; set; } = "ACTIVO";
    }
}