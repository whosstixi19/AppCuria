namespace AplicacionCuria.Models
{
    public class MatrimoniosAntiguos
    {
        public int id { get; set; }

        // Información del Novio
        public string nombrenovio { get; set; }
        public string lugarbautizonovio { get; set; }
        public string cedulapadrenovio { get; set; }
        public string cedulamadrenovio { get; set; }
        public DateTime fechanacimientonovio { get; set; }
        public DateTime fechabautizonovio { get; set; }

        // Información de la Novia
        public string nombrenovia { get; set; }
        public string lugarbautizonovia { get; set; }
        public string cedulapadrenovia { get; set; }
        public string cedulamadrenovia { get; set; }
        public DateTime fechanacimientonovia { get; set; }
        public DateTime fechabautizonovia { get; set; }

        // Información del Matrimonio
        public DateTime fechamatrimonio { get; set; }
        //public string lugarmatrimonio { get; set; }
        //public string celebrante { get; set; } // Persona que ofició la ceremonia

        // Información del Registro Civil
        public DateTime fecharegistrocivil { get; set; }
        public string lugarregistrocivil { get; set; }
        public string numeroactaregistrocivil { get; set; }

        //Registro civil
        public string tomo { get; set; }
        public string pagina { get; set; }
        public string acta { get; set; }
        public DateTime fecharegistro { get; set; }

        // Testigos
        public string cedulatestigo1 { get; set; }
        public string cedulatestigo2 { get; set; }
        public string? cedulatestigo3 { get; set; }
        public string? cedulatestigo4 { get; set; }
        public int ministro_id { get; set; }
        public int iglesia_id { get; set; }
        public string estado { get; set; } = "ACTIVO";

        public string observaciones { get; set; }
    }
}
