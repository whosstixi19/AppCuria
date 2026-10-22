using System.ComponentModel.DataAnnotations;

namespace AplicacionCuria.Models
{
    public class Falta
    {
        public int id { get; set; }

        [Required(ErrorMessage = "El alumno es obligatorio.")]
        public int alumno_id { get; set; }

        [Required(ErrorMessage = "La fecha de la falta es obligatoria.")]
        public DateTime fechafalta { get; set; }

        [Required(ErrorMessage = "La observación es obligatoria.")]
        [StringLength(500, ErrorMessage = "La observación no puede superar los 500 caracteres.")]
        public string observacion { get; set; }

        [StringLength(20)]
        public string estado { get; set; } = "ACTIVO";
    }
}