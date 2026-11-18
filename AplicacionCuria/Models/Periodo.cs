using System;
using System.ComponentModel.DataAnnotations;

namespace AplicacionCuria.Models
{
    public class Periodo
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "El nombre del periodo es obligatorio")]
        [StringLength(255)]
        public string nombre { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        public DateTime fecha_inicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        public DateTime fecha_fin { get; set; }

        [StringLength(20)]
        public string estado { get; set; } = "ACTIVO";

        public string? observaciones { get; set; }
    }
}