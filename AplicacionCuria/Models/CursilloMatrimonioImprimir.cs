using System.ComponentModel.DataAnnotations.Schema;
using AplicacionCuria.Models;

namespace AplicacionCuria.Models
{
    public class CursilloMatrimonioImprimir
    {
        public CursilloPreMatrimonial Cursillo { get; set; }
        public string NombreSacerdote { get; set; }
        public string FirmaSacerdote { get; set; }

    }
}

