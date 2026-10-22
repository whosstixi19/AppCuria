using System.ComponentModel.DataAnnotations;

namespace AplicacionCuria.Models
{
    public class Feligres
    {
        public int id { get; set; }

        [Required]
        [StringLength(255)]
        public string nombre { get; set; }

        [Required]
        [StringLength(255)]
        public string apellido { get; set; }

        [Required]
        [StringLength(20)]
        public string cedula { get; set; }

        [Required]
        public DateTime fechanacimiento { get; set; }

        [Required]
        [StringLength(255)]
        public string direccion { get; set; }

        [Required]
        [StringLength(20)]
        public string telefono { get; set; }

        [Required]
        [StringLength(255)]
        public string email { get; set; }

        [Required]
        [StringLength(20)]
        public string estado { get; set; } = "ACTIVO";

        [Required]
        public string observaciones { get; set; }
    }
}