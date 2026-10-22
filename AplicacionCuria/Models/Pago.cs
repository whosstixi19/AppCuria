using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AplicacionCuria.Models
{
    public class Pago
    {
        public int id { get; set; }

        [Required]
        [StringLength(50)]
        [Column("tipo_pago")]
        public string tipo_pago { get; set; }

        [Required]
        [StringLength(50)]
        public string concepto { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string nombres { get; set; }

        [Required]
        [StringLength(255)]
        public string apellidos { get; set; }

        [Required]
        public string direccion { get; set; }

        [Required]
        [StringLength(20)]
        public string cedula { get; set; }

        [Required]
        [StringLength(20)]
        public string telefono { get; set; }

        [Required]
        public decimal valor { get; set; }

        [Required]
        [StringLength(50)]
        [Column("tipopago")]
        public string tipoPago { get; set; }

        [Required]
        [Column("fechapago")]
        public DateTime fechaPago { get; set; }

        public int? referencia_id { get; set; }

        [StringLength(50)]
        public string? referencia_tabla { get; set; }

        [Required]
        [StringLength(20)]
        public string estado { get; set; } = "PAGADO";

        public string? observaciones { get; set; }  // <-- CAMBIO CLAVE

        public int? usuario_registro_id { get; set; } // <-- Opcional si no siempre se registra

        [Column("fecha_registro")]
        public DateTime fecha_registro { get; set; } = DateTime.Now;
    }
}
