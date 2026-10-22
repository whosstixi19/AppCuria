using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AplicacionCuria.Models
{
    public class CursilloPreMatrimonial
    {
        public int id { get; set; }

        [Required, StringLength(255)]
        public string nombrenovio { get; set; }

        [Required, RegularExpression(@"^\d{10}$")]
        public string cedulanovio { get; set; }

        [Required, RegularExpression(@"^\d{10}$")]
        public string telefononovio { get; set; }

        [Required]
        public DateTime fechanacimientonovio { get; set; }

        [Required, StringLength(255)]
        public string nombrenovia { get; set; }

        [Required, RegularExpression(@"^\d{10}$")]
        public string cedulanovia { get; set; }

        [Required, RegularExpression(@"^\d{10}$")]
        public string telefononovia { get; set; }

        [Required]
        public DateTime fechanacimientonovia { get; set; }

        [Required]
        public DateTime fechacursillo { get; set; }

        [Required]
        public DateTime fechamatrimonio { get; set; }

        [Required, StringLength(50)]
        public string estado { get; set; } = "PENDIENTE";

        // Foreign Key
        [ForeignKey("Parroquia")]
        public int parroquia { get; set; }

        // Propiedad de navegación
        public Parroquia Parroquia { get; set; }

        public string? observaciones { get; set; }
    }
}
