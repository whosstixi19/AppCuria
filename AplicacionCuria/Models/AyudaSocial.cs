
namespace AplicacionCuria.Models
{
    public class AyudaSocial
    {
        public int id { get; set; }
        public byte[] foto { get; set; } // Para almacenar imágenes o archivos binarios
        public string FotoBase64 { get; set; }
        public string nombre { get; set; }
        public string apellidos { get; set; }
        public string cedula { get; set; } // La cédula debe ser única
        public int edad { get; set; }
        public string genero { get; set; }
        public string instruccion { get; set; }
        public string estadocivil { get; set; }
        public string religion { get; set; }
        public DateTime fechanacimiento { get; set; }
        public string provincia { get; set; }
        public string canton { get; set; }
        public string parroquia { get; set; }
        public string calleprincipal { get; set; }
        public string numero { get; set; }
        public string callesecundaria { get; set; }
        public string sector { get; set; }
        public string telefono { get; set; }
        public string referencia { get; set; }
        public string discapacidad { get; set; }
        public string especificardiscapacidad { get; set; }
        public int? porcentajediscapacidad { get; set; }
        public string carnetdiscapacidad { get; set; }
        public string recibebdh { get; set; }
        public string jubilacion { get; set; }
        public string trabaja { get; set; }
        public string conquienvive { get; set; }
        public string? especifica_con_quien { get; set; }

        public string viviendaes { get; set; }
        public string tipoconstruccion { get; set; }
        public string descripcion { get; set; }
        public string nombrerepresentante { get; set; }
        public string telefonorepresentante { get; set; }
        public string relacion { get; set; }
        public string antecedentesmedicos { get; set; }
        public DateTime fechavisita { get; set; }
        public string observaciones { get; set; }
    }
}
