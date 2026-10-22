using System.ComponentModel.DataAnnotations;

namespace AplicacionCuria.Models
{
    public class ActaPreMatrimonial
    {
        public int id { get; set; }

        [Required]
        [StringLength(255)]
        public string nombrenovio { get; set; }

        [Required]
        [StringLength(255)]
        public string lugarbautizonovio { get; set; }

        [Required]
        [StringLength(20)]
        public string nombrepadrenovio { get; set; }

        [Required]
        [StringLength(20)]
        public string nombremadrenovio { get; set; }

        [Required]
        public DateTime fechanacimientonovio { get; set; }

        [Required]
        public DateTime fechabautizonovio { get; set; }

        [Required]
        [StringLength(255)]
        public string nombrenovia { get; set; }

        [Required]
        [StringLength(255)]
        public string lugarbautizonovia { get; set; }

        [Required]
        [StringLength(20)]
        public string nombrepadrenovia { get; set; }

        [Required]
        [StringLength(20)]
        public string nombremadrenovia { get; set; }

        [Required]
        public DateTime fechanacimientonovia { get; set; }

        [Required]
        public DateTime fechabautizonovia { get; set; }

        [Required]
        public DateTime fechamatrimonio { get; set; }
        
        [Required]
        public int iglesia_id { get; set; }

        [Required]
        public int ministro_id { get; set; }


        [Required]
        public DateTime fecharegistrocivil { get; set; }

        [Required]
        [StringLength(255)]
        public string lugarregistrocivil { get; set; }

        [Required]
        [StringLength(50)]
        public string numeroactaregistrocivil { get; set; }

        [Required]
        [StringLength(50)]
        public string numeropaginaregistrocivil { get; set; }

        [Required]
        [StringLength(50)]
        public string numertomoregistrocivil { get; set; }


        /////////////////
        [Required]
        [StringLength(50)]
        public string tomo { get; set; }

        [Required]
        [StringLength(50)]
        public string pagina { get; set; }

        [Required]
        [StringLength(50)]
        public string acta { get; set; }

        [Required]
        public DateTime fecharegistro { get; set; }

        [Required]
        public string observaciones { get; set; }

        [Required]
        [StringLength(20)]
        public string testigo1 { get; set; }

        [Required]
        [StringLength(20)]
        public string testigo2 { get; set; }

        [StringLength(20)]
        public string? testigo3 { get; set; }

        [StringLength(20)]
        public string? testigo4 { get; set; }

        [Required]
        public int cursillo_id { get; set; }

        [Required]
        [StringLength(20)]
        public string estado { get; set; } = "ACTIVO";
    }
}