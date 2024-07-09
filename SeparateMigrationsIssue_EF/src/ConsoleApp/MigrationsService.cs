using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace ConsoleApp;

public interface IMigrationsService
{
    void RunMigrations_ProviderSpecific();
    void RunMigrations_Simple();
}

public class MigrationsService(DomainDataContext domainDataContext, ISettings settings, ILogger<MigrationsService> logger) : IMigrationsService
{
    public void RunMigrations_Simple()
    {
        //System.Diagnostics.Debugger.Launch(); // Use this to attach a debugger during EF migration tool use
        try
        {
            IEnumerable<string> pendingMigrations = domainDataContext.Database.GetPendingMigrations();
            if (pendingMigrations.Any())
            {
                domainDataContext.Database.Migrate();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in {Method}. {Message}", nameof(RunMigrations_Simple), ex.Message);
        }
    }

    public void RunMigrations_ProviderSpecific()
    {
        //System.Diagnostics.Debugger.Launch(); // Use this to attach a debugger during EF migration tool use
        try
        {
            IEnumerable<string> pendingMigrations = domainDataContext.Database.GetPendingMigrations();
            if (!pendingMigrations.Any())
            {
                return;
            }

            List<string> migrationIdsToRun = GetMigrationsContainedInPending(pendingMigrations);
            if (migrationIdsToRun.Count == 0)
            {
                return;
            }

            IMigrator migrator = GetMigratorService(domainDataContext);
            if (migrator == null)
            {
                return;
            }

            foreach (string targetMigrationId in migrationIdsToRun)
            {
                migrator.Migrate(targetMigrationId);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in {Method}. {Message}", nameof(RunMigrations_ProviderSpecific), ex.Message);
        }
    }

    private List<string> GetMigrationsContainedInPending(IEnumerable<string> pendingMigrations)
    {
        List<string> migrationIdsToRun = [];
        List<Type> migrationTypes = [];

        // Force the satellite assemblies to load.  To be able to locate migration classes that have a MigrationAttribute.
        _ = new Migrations.Sqlite.MigrationsAssemblyName();
        _ = new Migrations.SqlServer.MigrationsAssemblyName();

        foreach (var assemblyName in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
        {
            Assembly assembly = Assembly.Load(assemblyName);
            List<Type> migrationTypesFound = assembly.GetTypes().Where(t => t.FullName.Contains(settings.DatabaseProvider.ToString()) && t.GetCustomAttributes(true).Any(at => at.GetType() == typeof(MigrationAttribute))).ToList();
            if (migrationTypesFound.Count != 0)
                migrationTypes.AddRange(migrationTypesFound);
        }

        foreach (Type migrationType in migrationTypes)
        {
            MigrationAttribute migrationAttribute = migrationType.GetCustomAttributes(true).Where(at => at.GetType() == typeof(MigrationAttribute)).FirstOrDefault() as MigrationAttribute;
            if (pendingMigrations.Contains(migrationAttribute.Id))
            {
                migrationIdsToRun.Add(migrationAttribute.Id);
            }
        }

        return migrationIdsToRun;
    }

    private static IMigrator GetMigratorService(DbContext dbContext)
    {
        DatabaseFacade databaseFacade = dbContext.Database;
        IInfrastructure<IServiceProvider> serviceProvider = databaseFacade;
        IMigrator service = serviceProvider.Instance.GetService<IMigrator>();
        return service ?? throw new InvalidOperationException(RelationalStrings.RelationalNotInUse);
    }
}
