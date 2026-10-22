namespace AplicacionCuria.Models
{
    public class CertificadoMatrimonioImprimir
    {
        public ActaPreMatrimonial Acta { get; set; }
        public ActaDeMatrimonio Actamatrimonio { get; set; }
        public string NombreSacerdote { get; set; }
        public string FirmaSacerdote { get; set; }

        public string NombreParroquia { get; set; }


    }
}
