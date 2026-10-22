using System.ComponentModel.DataAnnotations;

namespace AplicacionCuria.Models
{
    public class Consanguinidad
    {
        public int id { get; set; }
        public string reverendo { get; set; }
        public string novio { get; set; }
        public string novia { get; set; }
        public string parroquia { get; set; }
        public string feligres { get; set; }
        public DateTime fecha { get; set; }
    }
}
