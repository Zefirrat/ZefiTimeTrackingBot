using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zefi.Telegram.Bot.Constants;

namespace Zefi.Telegram.Bot.TimeTracking.Db.Extensions;

public static class ServiceCollectionExtensions
{
    private const string PostgresType = "postgres";
    private const string MssqlType = "mssql";
    private const string InmemoryType = "inmemory";

    public static IDictionary<string, Action<DbContextOptionsBuilder, string, string, string, string, string>>
        DbConfiguratorDictionary =
            new Dictionary<string, Action<DbContextOptionsBuilder, string, string, string, string, string>>()
            {
                { PostgresType, _usePostgres }, { MssqlType, _useMssql }, { InmemoryType, _useInMemory }
            };

    public static IServiceCollection AddDatabase(
        this IServiceCollection serviceCollection)
    {
        var configuration = serviceCollection.BuildServiceProvider()
            .GetRequiredService<IConfiguration>();
        var dbTypeOption = configuration.GetValue<string>(EnvironmentConstants.DbTypeEnvironmentName);

        serviceCollection.AddDbContext<TTDbContext>(opt => { ConfigureDb(dbTypeOption, configuration, opt); },
            ServiceLifetime.Scoped);

        var dbContext = serviceCollection.BuildServiceProvider()
            .GetRequiredService<TTDbContext>();

        if (dbTypeOption != InmemoryType)
        {
            dbContext.Database.Migrate();
        }

        return serviceCollection;
    }

    public static void ConfigureDb(string? dbTypeOption, IConfiguration configuration, DbContextOptionsBuilder opt)
    {
        if (dbTypeOption != null)
        {
            var dbName = configuration.GetValue<string>(EnvironmentConstants.DbNameEnvironmentName);
            var dbPort = configuration.GetValue<string>(EnvironmentConstants.DbPortEnvironmentName);
            var dbServer = configuration.GetValue<string>(EnvironmentConstants.DbServerEnvironmentName);
            var dbUserName = configuration.GetValue<string>(EnvironmentConstants.DbUserNameEnvironmentName);
            var dbPassword = configuration.GetValue<string>(EnvironmentConstants.DbPasswordEnvironmentName);
            DbConfiguratorDictionary[dbTypeOption](opt, dbName, dbPort, dbServer, dbUserName, dbPassword);
        }
        else
        {
            throw new NoNullAllowedException(EnvironmentConstants.DbTypeEnvironmentName);
        }
    }

    private static void _usePostgres(
        DbContextOptionsBuilder optionsBuilder,
        string dbName,
        string dbPort,
        string dbServer,
        string dbUserName,
        string dbPassword)
    {
        var connectionString = $"Host={dbServer};Port={dbPort};Database={dbName};User ID={dbUserName};Password={dbPassword};Pooling=true;Connection Lifetime=0;";
        optionsBuilder.UseNpgsql(connectionString, dbOpt => dbOpt.EnableRetryOnFailure());
    }

    private static void _useMssql(
        DbContextOptionsBuilder optionsBuilder,
        string dbName,
        string dbPort,
        string dbServer,
        string dbUserName,
        string dbPassword)
    {
        var connectionString = $"Server={dbServer}:{dbPort};Database={dbName};User={dbUserName};Password={dbPassword};";
        optionsBuilder.UseSqlServer(connectionString, dbOpt => dbOpt.EnableRetryOnFailure());
    }

    private static void _useInMemory(
        DbContextOptionsBuilder optionsBuilder,
        string dbName,
        string dbPort,
        string dbServer,
        string dbUserName,
        string dbPassword)
    {
        optionsBuilder.UseInMemoryDatabase(dbName, new InMemoryDatabaseRoot());
    }
}