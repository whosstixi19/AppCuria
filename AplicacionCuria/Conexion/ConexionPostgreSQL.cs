using AplicacionCuria.Models;
using Microsoft.EntityFrameworkCore;

namespace AplicacionCuria.Conexion
{
    public class ConexionPostgreSQL : DbContext
    {
        public ConexionPostgreSQL(DbContextOptions<ConexionPostgreSQL> options) : base(options)
        {

        }

        public DbSet<Usuario> usuarios { get; set; }
        public DbSet<ActaBautismo> actasbautismo { get; set; }
        public DbSet<Antiguos> bautismosantiguos { get; set; }
        public DbSet<ActaDeMatrimonio> actasdematrimonio { get; set; }
        public DbSet<ActaPreMatrimonial> actasprematrimoniales { get; set; }
        public DbSet<AlumnoCatequesis> alumnoscatequesis { get; set; }
        //public DbSet<AyudaSocial> ayudasocial { get; set; }
        public DbSet<Barrio> barrios { get; set; }
        public DbSet<Confirmacion> confirmaciones { get; set; }
        public DbSet<CursilloPrebautismal> cursillosprebautismales { get; set; }
        public DbSet<CursilloPreMatrimonial> cursillosprematrimoniales { get; set; }
        //public DbSet<DisponibilidadCultos> disponibilidadcultos { get; set; }
        public DbSet<Falta> faltas { get; set; }
        public DbSet<Feligres> feligreses { get; set; }
        public DbSet<Iglesia> iglesias { get; set; }
        public DbSet<MatrimoniosAntiguos> matrimoniosantiguos { get; set; }
        public DbSet<Ministro> ministros { get; set; }
        //public DbSet<MixtaReligion> mixtareligion { get; set; }
        public DbSet<Pago> pagos { get; set; }
        public DbSet<Parroquia> parroquias { get; set; }
        public DbSet<PrimeraComunion> primerascomuniones { get; set; }
        public DbSet<Agenda> agendas { get; set; }
        public DbSet<Sacerdote> sacerdotes { get; set; }
        public DbSet<Periodo> periodos { get; set; }
        public DbSet<Catequista> catequistas { get; set; }
        public DbSet<Misa> misas { get; set; }
        public DbSet<PersonasEvento> personas_evento { get; set; }

    }
}
