using System.ComponentModel.DataAnnotations;

namespace AplicacionCuria.Models
{
    public class Ministro
    {
        public int id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(255)]
        public string nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(255)]
        public string apellido { get; set; }

        [StringLength(255)]
        [EmailAddress(ErrorMessage = "Correo electrónico no válido")]
        public string email { get; set; }

        [StringLength(20)]
        public string telefono { get; set; }

        [DataType(DataType.Date)]
        public DateTime? fechaordenacion { get; set; }

        [StringLength(20)]
        public string estado { get; set; } = "ACTIVO";

        [Required(ErrorMessage = "Las observaciones son obligatorias")]
        public string observaciones { get; set; }
    }
}