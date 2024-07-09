using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace Data;

public interface ISettings
{
    string SqliteDatabaseName { get; }
    string DatabaseProvider { get; }

    string GetSqliteConnectionString();
    string GetSqlServerConnectionString();
}

public class Settings(IConfiguration configuration) : ISettings
{
    private readonly IConfiguration _configuration = configuration;

    public string SqliteDatabaseName => _configuration["AppSettings:SqliteDatabaseName"];

    public string DatabaseProvider => _configuration["AppSettings:DatabaseProvider"];

    public string GetSqlServerConnectionString() => _configuration.GetConnectionString("ConsoleApp");

    public string GetSqliteConnectionString()
    {
        string connectionString = string.Empty;
        string directory = AppContext.BaseDirectory;
        string databaseFileName = SqliteDatabaseName + ".sqlite";
        if (!string.IsNullOrWhiteSpace(directory) && !string.IsNullOrWhiteSpace(databaseFileName))
        {
            string filePath = Path.Combine(directory, databaseFileName);
            connectionString = new SqliteConnectionStringBuilder() { DataSource = filePath }.ConnectionString;
        }
        return connectionString;
    }
}

