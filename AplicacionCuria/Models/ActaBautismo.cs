﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace AplicacionCuria.Models
{
    public class ActaBautismo
    {
        public int id { get; set; }

        // Información personal del bautizado
        [StringLength(50, ErrorMessage = "El Nombre' no puede exceder los 50 caracteres.")]
        public string? nombrecompleto { get; set; }
        [StringLength(250, ErrorMessage = "El 'Lugar de nacimiento' no puede exceder los 250 caracteres.")]
        public string? lugarnacimiento { get; set; }
        [RegularExpression(@"^\d{10}$", ErrorMessage = "La 'Cédula' no cumple con el formato.")]
        public string? cedula { get; set; }
        public string? sexo { get; set; }
        public DateTime fechanacimiento { get; set; }

        [Required]
        public DateTime fechabautizo { get; set; }
        [RegularExpression(@"^\d{10}$", ErrorMessage = "La 'Cédula Padre' no cumple con el formato.")]
        public string? cedulapadre { get; set; }
        [RegularExpression(@"^\d{10}$", ErrorMessage = "La 'Cédula Madre' no cumple con el formato.")]
        public string? cedulamadre { get; set; }
        [RegularExpression(@"^\d{10}$", ErrorMessage = "La 'Cédula Padrino' no cumple con el formato.")]
        public string? cedulapadrino1 { get; set; }
        [RegularExpression(@"^\d{10}$", ErrorMessage = "La 'Cédula Padrino 2' no cumple con el formato.")]
        public string? cedulapadrino2 { get; set; }

        [RegularExpression(@"^(\d{10})?$", ErrorMessage = "La 'Cédula Padrino 3' no cumple con el formato.")]
        public string? cedulapadrino3 { get; set; }

        [RegularExpression(@"^(\d{10})?$", ErrorMessage = "La 'Cédula Padrino 4' no cumple con el formato.")]
        public string? cedulapadrino4 { get; set; }

        [RegularExpression(@"^(\d{10})?$", ErrorMessage = "La 'Cédula Padrino 5' no cumple con el formato.")]
        public string? cedulapadrino5 { get; set; }


        // Datos del acta
        [StringLength(50, ErrorMessage = "El 'Página' no puede exceder los 50 caracteres.")]
        public string? pagina { get; set; }
        [StringLength(50, ErrorMessage = "El 'Tomo' no puede exceder los 50 caracteres.")]
        public string? tomo { get; set; }
        [StringLength(50, ErrorMessage = "El 'Libro' no puede exceder los 50 caracteres.")]
        public string? acta { get; set; }

        // Registro civil
        [StringLength(4, ErrorMessage = "El 'Año de Registro Civil' no puede exceder los 4 caracteres.")]
        public string? registrocivilano { get; set; }
        public string? registrociviltomo { get; set; }
        public string? registrocivilfolio { get; set; }
        public string? registrocivilacta { get; set; }
        public DateTime registrocivilfecha { get; set; }

        // Observaciones adicionales
        public string observaciones { get; set; } = "";

        [Required ]
        public int ministro_id { get; set; }

        [Required]
        public int sacerdote_id { get; set; }

        [Required]
        public int iglesia_id { get; set; }

        [Required]
        public int parroquia_id { get; set; }

        [Required(ErrorMessage = "Seleccione un cursillo prebautismal.")]
        public int cursillo_id { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [StringLength(20, ErrorMessage = "El 'Estado' no puede exceder los 20 caracteres.")]
        public string estado { get; set; } = "ACTIVO";
    }
}