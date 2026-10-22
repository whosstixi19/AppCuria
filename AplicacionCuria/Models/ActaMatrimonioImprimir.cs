using System;

namespace AplicacionCuria.Models
{
    public class ActaMatrimonioImprimir
    {
        public ActaPreMatrimonial Acta { get; set; }
        public ActaDeMatrimonio Actamatrimonio { get; set; }

        public string NombreSacerdote { get; set; }

        public string FirmaSacerdote { get; set; }  // Base64 de la firma

        public string NombreParroquia { get; set; }  // Nombre de la iglesia/parroquia
    }
}
