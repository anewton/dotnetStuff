using ConsoleApp;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args).UseConsoleLifetime();
        string environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.Sources.Clear();
            config.AddJsonFile("appsettings.json");
            config.AddEnvironmentVariables();
            string appsettingsFileName = $"appsettings.{environmentName}.json";
            config.AddJsonFile(appsettingsFileName, false);
        });


        // Logging configuration with Serilog
        string debuggingOutputTemplate = "[{Timestamp:HH:mm:ss.fff} {Level:u3} {Environment}] {Message:lj} {NewLine}{Exception}";

        LoggerConfiguration logger = new LoggerConfiguration()
            .WriteTo.Debug(
                restrictedToMinimumLevel: LogEventLevel.Verbose,
                outputTemplate: debuggingOutputTemplate)
            .WriteTo.Console(
                restrictedToMinimumLevel: LogEventLevel.Verbose,
                outputTemplate: debuggingOutputTemplate)
            .MinimumLevel.Is(LogEventLevel.Verbose)
            .MinimumLevel.Override(DbLoggerCategory.Query.Name, LogEventLevel.Debug)
            .MinimumLevel.Override(DbLoggerCategory.Database.Command.Name, LogEventLevel.Warning)
            .MinimumLevel.Override(DbLoggerCategory.Database.Transaction.Name, LogEventLevel.Warning)
            .MinimumLevel.Override(DbLoggerCategory.ChangeTracking.Name, LogEventLevel.Warning)
            .MinimumLevel.Override(DbLoggerCategory.Update.Name, LogEventLevel.Warning)
            .MinimumLevel.Override(DbLoggerCategory.Migrations.Name, LogEventLevel.Warning)
            .MinimumLevel.Override(DbLoggerCategory.Infrastructure.Name, LogEventLevel.Warning)
            .MinimumLevel.Override(DbLoggerCategory.Model.Name, LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Environment", environmentName);
        Log.Logger = logger.CreateLogger();

        builder.UseSerilog();

        builder.ConfigureLogging((context, logging) =>
        {
            logging.AddConsole();
            logging.AddDebug();
            logging.AddSerilog();
        });

        builder.ConfigureServices((context, services) =>
        {
            ISettings settings = new Settings(context.Configuration);
            services.AddSingleton(settings);
            services.AddScoped<IApplicationRunner, ApplicationRunner>();
            services.AddDbContext<IDomainDataContext, DomainDataContext>();
            services.AddScoped<IMigrationsService, MigrationsService>();
        });

        using var host = builder.Build();
        await host.StartAsync();
        var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

        using IServiceScope scope = host.Services.CreateScope();

        // Skip when running from EF design-time tool
        if (!EF.IsDesignTime)
        {
            IMigrationsService migrationsService = scope.ServiceProvider.GetRequiredService<IMigrationsService>();
            migrationsService.RunMigrations_ProviderSpecific();
        }

        var applicationRunner = scope.ServiceProvider.GetService<IApplicationRunner>();
        applicationRunner.RunApplication();

        Log.CloseAndFlush();
        lifetime.StopApplication();
        await host.WaitForShutdownAsync();
    }
}