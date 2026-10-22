using System.ComponentModel.DataAnnotations;

namespace AplicacionCuria.Models
{
    public class UsuarioRegisterRequest
    {
        public int Id { get; set; }  
        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [MinLength(6)]
        public string? Password { get; set; }

        public string Rol { get; set; }
    }
}
