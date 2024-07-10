using Microsoft.EntityFrameworkCore;
using Dominio;



public class Contexto : DbContext
{
    private readonly DbContextOptions _options;

    public Contexto()
    {
    }
    public Contexto(DbContextOptions options) : base(options)
    {
        _options = options;
    }

    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_options == null)
            optionsBuilder.UseSqlServer("Server=DESKTOP-58UEHOH;Database=portal_db;Trusted_Connection=True;TrustServerCertificate=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UsuarioConfiguracoes());
    }
}