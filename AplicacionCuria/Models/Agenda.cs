using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AplicacionCuria.Models
{
    public class Agenda
    {
        public int id { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        public DateTime fechaagenda { get; set; }

        public TimeSpan? hora { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public string descripcion { get; set; }

        [StringLength(255, ErrorMessage = "El lugar no puede superar los 255 caracteres.")]
        public string lugar { get; set; }

        public string responsables_ids { get; set; }  // Guardará "1,3,5"


        [StringLength(20)]
        public string estado { get; set; } = "ACTIVO";

        [StringLength(50)]
        public string tipo_evento { get; set; }   

        [StringLength(100)]
        public string titulo { get; set; }        

        public bool notificar { get; set; } = true;  

        [Range(0, 30, ErrorMessage = "Los días de anticipación deben estar entre 0 y 30.")]
        public int recordatorio_anticipacion { get; set; } = 1;  

    }
}
