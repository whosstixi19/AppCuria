namespace AplicacionCuria.Models
{
    public class ComprobanteDonacionViewModel
    {
        public int Id { get; set; }
        public string Donante { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public string MetodoDonacion { get; set; } = string.Empty;
        public string TipoServicio { get; set; } = string.Empty;
        public string Concepto { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
        public DateTime FechaDonacion { get; set; }
        public string NombreSacerdote { get; set; } = string.Empty;
        public string FirmaSacerdote { get; set; } = string.Empty;
        public string Parroquia { get; set; } = string.Empty;
    }
}
