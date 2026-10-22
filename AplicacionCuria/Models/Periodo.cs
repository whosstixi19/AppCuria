using System;
using System.ComponentModel.DataAnnotations;

namespace AplicacionCuria.Models
{
    public class Periodo
    {
        [Key]
        public int id { get; set; }

        [Required]
        [StringLength(255)]
        public string nombre { get; set; }

        [Required]
        public DateTime fecha_inicio { get; set; }

        [Required]
        public DateTime fecha_fin { get; set; }

        [StringLength(20)]
        public string estado { get; set; } = "ACTIVO";

        public string observaciones { get; set; }
    }
}