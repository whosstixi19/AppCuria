using System.ComponentModel.DataAnnotations;

namespace AplicacionCuria.Models
{
    public class Barrio
    {
        public int id { get; set; }

        [Required]
        public string nombre { get; set; }

        [Required]
        public int ministro_id { get; set; }

        [Required]
        public int iglesia_id { get; set; }

        [Required]
        public int misa_id { get; set; }

        [Required]
        public string estado { get; set; }

        [Required]
        public string observaciones { get; set; }
    }
}
