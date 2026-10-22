using System.ComponentModel.DataAnnotations;

namespace AplicacionCuria.Models
{
    public class ActaDeMatrimonio
    {
        public int id { get; set; }

        [Required(ErrorMessage = "El nombre del Padrino 1 es obligatorio.")]
        public string padrino1 { get; set; }

        [Required(ErrorMessage = "El nombre del Padrino 2 es obligatorio.")]
        public string padrino2 { get; set; }

        public string? padrino3 { get; set; }
        public string? padrino4 { get; set; }

        public DateTime fecha_matrimonio { get; set; }

        [Required(ErrorMessage = "La fecha de registro es obligatoria.")]

        public DateTime? fecharegistro { get; set; }

        public int acta_prematrimonial_id { get; set; }
        public int iglesia_id { get; set; }
        public int ministro_id { get; set; }
        public string acta { get; set; }
        public string tomo { get; set; }
        public string pagina { get; set; }
        
        public string? observaciones { get; set; }
    }
}
