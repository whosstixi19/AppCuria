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

        [StringLength(20)]
        public string cedula { get; set; }

        [StringLength(20)]
        public string telefono { get; set; }

        [StringLength(255)]
        [EmailAddress(ErrorMessage = "Correo electrónico no válido")]
        public string email { get; set; }

        [StringLength(20)]
        public string estado { get; set; } = "ACTIVO";

        public byte[]? firma { get; set; }  // campo para almacenar la firma en binario

        [Required(ErrorMessage = "La observación es obligatoria")]
        public string observaciones { get; set; }
    }
}