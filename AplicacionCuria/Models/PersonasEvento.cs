using System.ComponentModel.DataAnnotations;

namespace AplicacionCuria.Models
{
    public class PersonasEvento
    {
        [Key]
        public int id { get; set; }

        [Required]
        [StringLength(100)]
        public string nombre { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string email { get; set; }

        [StringLength(20)]
        public string telefono { get; set; }

        [StringLength(50)]
        public string rol { get; set; }
    }
}
