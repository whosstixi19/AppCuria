using System;
using System.ComponentModel.DataAnnotations;

namespace AplicacionCuria.Models
{
    public class Misa
    {
        public int id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(255)]
        public string nombre { get; set; }

        [Required(ErrorMessage = "La hora de inicio es obligatoria")]
        [DataType(DataType.Time)]
        public TimeSpan hora_inicio { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan? hora_fin { get; set; }

        [StringLength(20)]
        public string dia_semana { get; set; }

        public int? iglesia_id { get; set; }

        public int? sacerdote_id { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string descripcion { get; set; }

        [StringLength(20)]
        public string estado { get; set; } = "ACTIVO";
    }
}