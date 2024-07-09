using Data;
using Domain;
using Microsoft.Extensions.Logging;

namespace ConsoleApp;

public interface IApplicationRunner
{
    void RunApplication();
}

public class ApplicationRunner : IApplicationRunner
{
    private readonly DomainDataContext _domainDataContext;
    private readonly ILogger<ApplicationRunner> _logger;

    public ApplicationRunner(DomainDataContext domainDataContext, ILogger<ApplicationRunner> logger)
    {
        _domainDataContext = domainDataContext;
        _logger = logger;
    }

    public void RunApplication()
    {
        // Get database catalog data
        _logger.LogInformation("Retrieving any and all catalog data from the database.");
        List<Catalog> databaseCatalogs = _domainDataContext.Catalogs.ToList();

        // Remove database catalog data
        _logger.LogInformation("Removing any and all catalog data from the database.");
        if (databaseCatalogs.Count != 0)
        {
            _domainDataContext.RemoveRange(databaseCatalogs);
            _domainDataContext.SaveChanges();
        }

        // Load catalog from file
        _logger.LogInformation("Loading catalog data from a local file resource.");
        var catalogObjects = CatalogDataImportService.GetCatalogObjects("MessierCatalog.csv");
        var catalog = new Catalog("Messier Catalog", catalogObjects);

        // Insert database catalog data
        _logger.LogInformation("Add catalog data to the database.");
        if (catalog.Objects.Any())
        {
            _domainDataContext.Add(catalog);
            _domainDataContext.SaveChanges();
        }

        // Get database catalog data
        databaseCatalogs = _domainDataContext.Catalogs.ToList();
        _logger.LogInformation("There are {CatalogCount} catalogs stored in the database", databaseCatalogs.Count);
    }
}
