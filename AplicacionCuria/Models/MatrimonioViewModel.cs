using System.ComponentModel.DataAnnotations;

namespace AplicacionCuria.Models
{
    public class MatrimonioViewModel
    {
        [Key]
        public int Id { get; set; }

        // Datos del esposo
        [Required(ErrorMessage = "El nombre del esposo es obligatorio.")]
        public string NombreEsposo { get; set; }

        [Required(ErrorMessage = "La cédula o pasaporte del esposo es obligatorio.")]
        public string DniEsposo { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento del esposo es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimientoEsposo { get; set; }

        // Datos de la esposa
        [Required(ErrorMessage = "El nombre de la esposa es obligatorio.")]
        public string NombreEsposa { get; set; }

        [Required(ErrorMessage = "La cédula o pasaporte de la esposa es obligatorio.")]
        public string DniEsposa { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento de la esposa es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimientoEsposa { get; set; }

        // Información del matrimonio
        [Required(ErrorMessage = "La fecha del matrimonio es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaMatrimonio { get; set; }

        [Required(ErrorMessage = "El lugar del matrimonio es obligatorio.")]
        public string LugarMatrimonio { get; set; }

        // Testigos
        [Required(ErrorMessage = "El nombre del testigo 1 es obligatorio.")]
        public string Testigo1 { get; set; }

        [Required(ErrorMessage = "El nombre del testigo 2 es obligatorio.")]
        public string Testigo2 { get; set; }

    }
}
