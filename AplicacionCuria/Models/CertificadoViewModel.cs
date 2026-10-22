namespace AplicacionCuria.Models
{
    public class CertificadoViewModel
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Nivel { get; set; }
        public string Paralelo { get; set; }

        public string Catequista1 { get; set; }   // Primer catequista
        public string Catequista2 { get; set; }   // Segundo catequista (opcional)

        public string Sacerdote { get; set; }  // Nombre completo del sacerdote
        public byte[] FirmaSacerdote { get; set; }  // Firma (opcional)

        public string CatequistasCompletos
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Catequista2))
                    return Catequista1;
                return $"{Catequista1} y {Catequista2}";
            }
        }

        // NUEVA PROPIEDAD
        public string TituloCatequista
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Catequista2))
                {
                    return "la/el catequista";
                }
                else
                {
                    return "las/los catequistas";
                }
            }
        }
    }
}
