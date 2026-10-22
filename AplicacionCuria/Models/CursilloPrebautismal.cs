using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AplicacionCuria.Models
{
    public class CursilloPrebautismal
    {
        public int id { get; set; }

        [Required(ErrorMessage = "El campo 'Nombre' es obligatorio.")]
        [StringLength(255, ErrorMessage = "El nombre no puede superar los 255 caracteres.")]
        public string nombrecompletonino { get; set; }

        // Datos del padre, madre, padrino y madrina
        [Required(ErrorMessage = "El nombre del padre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre del padre no puede superar los 100 caracteres.")]
        public string nombrepadre { get; set; }

        [Required(ErrorMessage = "El nombre de la madre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre de la madre no puede superar los 100 caracteres.")]
        public string nombremadre { get; set; }

        [Required(ErrorMessage = "El nombre del padrino es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre del padrino no puede superar los 100 caracteres.")]
        public string nombrepadrino { get; set; }

        [Required(ErrorMessage = "El nombre de la madrina es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre de la madrina no puede superar los 100 caracteres.")]
        public string nombremadrina { get; set; }

        // Foreign Key
        [Range(1, int.MaxValue, ErrorMessage = "La parroquia es obligatoria.")]
        [ForeignKey("Parroquia")]
        public int parroquia { get; set; }


        // Propiedad de navegación
        public Parroquia Parroquia { get; set; }


        // Quien inscribe y contactos
        public string quieninscribe { get; set; }
        [Required(ErrorMessage = "El campo 'Teléfono' es obligatorio.")]
        [RegularExpression(@"^\d{8,12}$", ErrorMessage = "El 'Teléfono' debe tener entre 8 y 12 dígitos.")]
        public string telefono { get; set; }
        [Required(ErrorMessage = "El campo 'Celular' es obligatorio.")]
        [RegularExpression(@"^\d{8,12}$", ErrorMessage = "El 'Celular' debe tener entre 8 y 12 dígitos.")]
        public string celular { get; set; }

        [Required(ErrorMessage = "La fecha de bautizo es obligatoria.")]
        public DateTime fechabautizo { get; set; }

        [Required(ErrorMessage = "La fecha del cursillo es obligatoria.")]
        public DateTime fechacursillo { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [StringLength(50, ErrorMessage = "El estado no puede superar los 50 caracteres.")]
        public string estado { get; set; } = "PENDIENTE";        

        [Required(ErrorMessage = "Las observaciones son obligatorias.")]
        public string observaciones { get; set; }
    }
}