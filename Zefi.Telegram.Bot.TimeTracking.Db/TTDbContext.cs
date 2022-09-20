using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Zefi.Telegram.Bot.Constants;
using Zefi.Telegram.Bot.TimeTracking.Db.Extensions;
using Zefi.Telegram.Bot.TimeTracking.Db.Models;

namespace Zefi.Telegram.Bot.TimeTracking.Db;

public class TTDbContext : DbContext
{
    public TTDbContext(
        DbContextOptions<TTDbContext> context) :
        base(context)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddUserSecrets(typeof(TTDbContext).Assembly, true)
                .AddEnvironmentVariables(EnvironmentConstants.EnvironmentPrefix)
                .Build();
            var dbTypeOption = configuration.GetValue<string>(EnvironmentConstants.DbTypeEnvironmentName);
            ServiceCollectionExtensions.ConfigureDb(dbTypeOption, configuration, optionsBuilder);
        }
    }

    public DbSet<MessageTemplate> MessageTemplates { get; private set; }
    public DbSet<TelegramUser> TelegramUsers { get; private set; }
    public DbSet<TimeTrackingInfo> TimeTrackingInfos { get; private set; }
}