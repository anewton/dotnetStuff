### Single Project with Multiple Provider Migrations Issue - Entity Framework

With Entity Framework Core, it is possible to maintain multiple database provider migration projects.  Which is useful when a single database context might be deployed with a SqlServer for one customer, or a Sqlite database for another customer.  The same database context code can be used with a minor switch in the configuration method.

Documentation for this capability can be found here, [Microsoft - Migrations with Multiple Providers](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/providers?tabs=dotnet-core-cli), and here, [Using a Separate Migrations Project](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/projects?tabs=dotnet-core-cli).

This project attempts to illustrate an edge case that makes separate class library projects a requirement when multiple database providers are used.  There seems to be an error in an internal Migrate method that ignores any target migration id when supplied.  Noting that this is all undocumented and not supported.  It would be very nice to have support for the scenario described here.

<b><u>Proposed solution</u></b>: Make a public method to allow running a migration by a specific target migration id.  Or, fix the internal method that already has this parameter but seems not to work as expected.  Based on the method name and parameter name.

This is a console application that loads csv data for some astronomy Messier objects into a database.  The MigrationsService.cs code file contains a class that is used in application startup to run any pending migrations.  There are 2 methods.  One is a simple and more typical use case.  The other represents the edge use case.

In the namespace, Microsoft.EntityFrameworkCore.Migrations, there is an interface called, IMigrator.  This can be instantiated from the database context service provider.  This IMigrator interface contains a method called Migrate that has a parameter for the migration target id.  

This console application project illustrates use of the IMigrator interface to specifically run a migration that is found to be pending, based on the migration id.  

The goal would be to allow creating a separate namespace, or folder, within the same project that contains the database context.  Where each folder, or namespace, represents the target database provider.  For example, in this solution there could be in the Data project a folder for Sqlite and for SqlServer.  These could contain multiple separate database migrations based on the provider.  

The migration commands below show how to create working migrations based on a provider switch.  This puts migrations into separate class library projects.  



#### Entity Framework Initial Migration Commands - Separate Projects

Steps to use

1. Set the appsettings.Development.json config value AppSettings:DatabaseProvider to "Sqlite"
1. Open a terminal at the root folder of the solution.  (Same folder that the solution file is in).
1. Copy/paste the Sqlite command below and run it.  An EF migration will be generated in the Migrations.Sqlite project.
1.<b>IMPORTANT STEP</b>:  Now change the appsettings.Development.json config value AppSettings:DatdabaseProvider to "SqlServer"
1. In the terminal previously opened, copy/paste the SqlServer command below and run it.  An EF migration will be generated in the Migrations.SqlServer project.
    - <b>NOTE</b>: For SqlServer, it is assumed that there is a SQL Server instance available and running where the configuration connection string is pointed to.  See the appsettings.Development.json config value ConnectionStrings:ConsoleApp to be sure there is SQL Server connection that will work.
1. After the commands below successfully generate EF migration code, the ConsoleApp can now run automatically any migrations at startup.  For the database provider indicated in the configuration file noted above.

##### Sqlite
```
dotnet-ef migrations add InitialCreate --startup-project ./src/ConsoleApp/ConsoleApp.csproj --project ./src/Migrations.Sqlite/Migrations.Sqlite.csproj --context DomainDataContext --output-dir InitialCreate
```

##### SqlServer
```
dotnet-ef migrations add InitialCreate --startup-project ./src/ConsoleApp/ConsoleApp.csproj --project ./src/Migrations.SqlServer/Migrations.SqlServer.csproj --context DomainDataContext --output-dir InitialCreate
```
<br />
<br />
<br />


The following example migration commands would place those same migrations inside subfolder of the Data project.  While it is possible to create the migrations this way, the method in IMigrator to use a specific target id seems to completely ignore the target id.  Again, only when using migrations for different databse providers that were located inside the same project.



#### Entity Framework Initial Migration Commands - Single Project with Mulitple Migration Providers (DOES NOT WORK)

Steps to use

<b>NOTE</b>: This step is already completed but Migrations were exluded from the Data project.  To skips steps below, simply include the migration code that was excluded and complete step 1 to change the migration assembly constant.

1. Change the code in DomainDataContext.cs in the Data project, so that the migration assembly names match the namespace of the location these migrations will be created in.  As follows:
```
    private const string SQLSERVER_MIGRATIONSASSEMBLY = "Data";
    private const string SQLITE_MIGRATIONSASSEMBLY = "Data";
```
1. Set the appsettings.Development.json config value AppSettings:DatabaseProvider to "Sqlite"
1. Open a terminal at the root folder of the solution.  (Same folder that the solution file is in).
1. Copy/paste the Sqlite command below and run it.  An EF migration will be generated in the Data project in a folder with the path "Migrations/Sqlite/InitialCreate".
1. <b>IMPORTANT STEP</b>:  Now change the appsettings.Development.json config value AppSettings:DatdabaseProvider to "SqlServer"
1. In the terminal previously opened, copy/paste the SqlServer command below and run it.  An EF migration will be generated in the  Data project in a folder with the path "Migrations/SqlServer/InitialCreate".
1. After the commands below successfully generate EF migration code, the ConsoleApp can now run automatically any migrations at startup.  For the database provider indicated in the configuration file noted above.
1. <b>NOTE: This approach does not work!!!</b>  During create of the migrations, the snapshot files must be manually removed and then placed back.  Otherwise the snapshot would consider the database changed from one provider to the next.  Also, the Migrate method when invoked, seems to ignore any target migration id parameter.  All migrations will run, regardless of the configured provider.  Which causes an exception.  When, for example, Sqlite migration code is run against a SqlServer database.

##### Sqlite
```
dotnet-ef migrations add InitialCreate_Sqlite --startup-project ./src/ConsoleApp/ConsoleApp.csproj --project ./src/Data/Data.csproj --context DomainDataContext --output-dir Migrations/Sqlite/InitialCreate
```

##### SqlServer
```
dotnet-ef migrations add InitialCreate_SqlServer --startup-project ./src/ConsoleApp/ConsoleApp.csproj --project ./src/Data/Data.csproj --context DomainDataContext --output-dir Migrations/SqlServer/InitialCreate
```