using System.ComponentModel.DataAnnotations;

namespace AplicacionCuria.Models
{
    public class Sacerdote
    {
        public int id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(255)]
        public string nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(255)]
        public string apellido { get; set; }

        [Required(ErrorMessage = "La cédula es obligatoria")]
        [StringLength(20)]
        public string cedula { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [StringLength(20)]
        public string telefono { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [StringLength(255)]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        public string email { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [StringLength(20)]
        public string estado { get; set; } = "ACTIVO";

        public byte[]? firma { get; set; }  // campo para almacenar la firma en binario

        public string? observaciones { get; set; }
    }
}