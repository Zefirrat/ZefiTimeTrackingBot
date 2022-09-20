using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Zefi.Telegram.Bot.TimeTracking.Db;

public static class ServiceCollectionExtensions
{
    const string DbTypeEnvironmentName = "DbType";
    const string DbNameEnvironmentName = "DbName";
    const string DbPortEnvironmentName = "DbPort";
    const string DbServerEnvironmentName = "DbServer";
    const string DbUserNameEnvironmentName = "DbUserName";
    const string DbPasswordEnvironmentName = "DbPassword";

    private static IDictionary<string, Action<DbContextOptionsBuilder, string, string, string, string, string>>
        _dbConfiguratorDictionary =
            new Dictionary<string, Action<DbContextOptionsBuilder, string, string, string, string, string>>()
            {
                { "postgres", _usePostgres }, { "mssql", _useMssql }, { "inmemory", _useInMemory }
            };

    public static IServiceCollection AddDatabase(
        this IServiceCollection serviceCollection)
    {
        var configuration = serviceCollection.BuildServiceProvider()
            .GetRequiredService<IConfiguration>();
        serviceCollection.AddDbContextFactory<TTDbContext>(opt =>
        {
            var dbTypeOption = configuration.GetValue<string>(DbTypeEnvironmentName);
            if (dbTypeOption != null)
            {
                var dbName = configuration.GetValue<string>(DbNameEnvironmentName);
                var dbPort = configuration.GetValue<string>(DbPortEnvironmentName);
                var dbServer = configuration.GetValue<string>(DbServerEnvironmentName);
                var dbUserName = configuration.GetValue<string>(DbUserNameEnvironmentName);
                var dbPassword = configuration.GetValue<string>(DbPasswordEnvironmentName);
                _dbConfiguratorDictionary[dbTypeOption](opt, dbName, dbPort, dbServer, dbUserName, dbPassword);
            }
            else
            {
                throw new NoNullAllowedException(DbTypeEnvironmentName);
            }
        });

        return serviceCollection;
    }

    private static void _usePostgres(
        DbContextOptionsBuilder optionsBuilder,
        string dbName,
        string dbPort,
        string dbServer,
        string dbUserName,
        string dbPassword)
    {
        var connectionString = $"Server={dbServer}:{dbPort};Database={dbName};User={dbUserName};Password={dbPassword};";
        optionsBuilder.UseNpgsql(connectionString, dbOpt=>dbOpt.EnableRetryOnFailure());
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
        optionsBuilder.UseSqlServer(connectionString, dbOpt=>dbOpt.EnableRetryOnFailure());
    }

    private static void _useInMemory(
        DbContextOptionsBuilder optionsBuilder,
        string dbName,
        string dbPort,
        string dbServer,
        string dbUserName,
        string dbPassword)
    {
        optionsBuilder.UseInMemoryDatabase(dbName);
    }
}