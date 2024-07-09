using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data;

public interface IDomainDataContext
{
    DbSet<Catalog> Catalogs { get; set; }
    DbSet<CatalogObject> Objects { get; set; }
}

public class DomainDataContext : DbContext, IDomainDataContext
{
    private const string SQLSERVER_MIGRATIONSASSEMBLY = "Migrations.SqlServer";
    private const string SQLITE_MIGRATIONSASSEMBLY = "Migrations.Sqlite";

    //private const string SQLSERVER_MIGRATIONSASSEMBLY = "Data";
    //private const string SQLITE_MIGRATIONSASSEMBLY = "Data";

    private readonly ILogger<DomainDataContext> _logger;
    private readonly ISettings _settings;

    public DomainDataContext(ILogger<DomainDataContext> logger, ISettings settings)
    {
        _logger = logger;
        _settings = settings;
    }

    // Use for EF Migrations or when 'AddDbContext' is configured on the application service provider
    public DomainDataContext(DbContextOptions<DomainDataContext> options, ILogger<DomainDataContext> logger) : base(options)
    {
        _logger = logger;
    }

    public DomainDataContext(DbContextOptions<DomainDataContext> options, ILogger<DomainDataContext> logger, ISettings settings) : base(options)
    {
        _logger = logger;
        _settings = settings;
    }

    public DbSet<Catalog> Catalogs { get; set; }
    public DbSet<CatalogObject> Objects { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
            return;
        string connectionString = string.Empty;

        switch (_settings.DatabaseProvider)
        {
            case "SqlServer":
                connectionString = _settings.GetSqlServerConnectionString();
                optionsBuilder.UseSqlServer(connectionString, x => x.MigrationsAssembly(SQLSERVER_MIGRATIONSASSEMBLY));
                break;
            case "Sqlite":
                connectionString = _settings.GetSqliteConnectionString();
                optionsBuilder.UseSqlite(connectionString, x => x.MigrationsAssembly(SQLITE_MIGRATIONSASSEMBLY));
                break;
        }


        //if (_logger != null)
        //{
        //    optionsBuilder.EnableSensitiveDataLogging();
        //    optionsBuilder.LogTo(
        //        FilterLog,
        //        (eventdata) => _logger.Log(eventdata.LogLevel, eventdata.EventId, message: eventdata.ToString().ReplaceLineEndings("  ")));
        //}

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Catalog>()
            .ToTable("Catalogs")
            .HasMany(catalog => catalog.Objects)
            .WithOne(catalogObject => catalogObject.Catalog)
            .OnDelete(DeleteBehavior.Cascade)
            .HasPrincipalKey(catalog => catalog.Id);

        modelBuilder.Entity<CatalogObject>()
            .ToTable("Objects")
            .HasKey(catalogObject => catalogObject.Id);

        base.OnModelCreating(modelBuilder);
    }


}