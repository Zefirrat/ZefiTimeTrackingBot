// See https://aka.ms/new-console-template for more information

using System.Data;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Telegram.Bot;
using Zefi.Telegram.Bot.Constants;
using Zefi.Telegram.Bot.TimeTracking;
using Zefi.Telegram.Bot.TimeTracking.Db;
using Zefi.Telegram.Bot.TimeTracking.Db.Extensions;
using Zefi.Telegram.Bot.TimeTracking.Db.Handlers.Handlers;
using Zefi.Telegram.Bot.TimeTracking.Mediatr.Models.Notifications;


var loggerConfiguration = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console();

var logger = loggerConfiguration.CreateLogger();

logger.Information("Logger configured.");

try
{
    var host = Host.CreateDefaultBuilder();
    host.ConfigureHostConfiguration((cb) =>
    {
        cb.AddUserSecrets(typeof(Program).Assembly, true);
        cb.AddEnvironmentVariables(EnvironmentConstants.EnvironmentPrefix);
    });

    host.ConfigureServices(services =>
    {
        services.AddLogging(l => { l.AddSerilog(logger); });
        services.AddSingleton<TelegramBotStarter>();
        services.AddTransient<ActionFactory>();
        services.AddMediatR(typeof(Program), typeof(UserAddressedHandler), typeof(UserAddressed), typeof(TTDbContext));
        services.AddDatabase();
    });


    host.ConfigureServices((
        hs,
        services) =>
    {
        var configuration = hs.Configuration;
        var botToken = configuration.GetValue<string>("BotToken");
        if (string.IsNullOrEmpty(botToken))
        {
            throw new NoNullAllowedException(nameof(botToken));
        }

        var botClient = new TelegramBotClient(botToken);
        services.AddSingleton(botClient);
    });

    var buildHost = host.Build();

    var botStarter = buildHost.Services.GetRequiredService<TelegramBotStarter>();
    botStarter.StartListen();

    await buildHost.RunAsync();
}
catch (Exception e)
{
    logger.Fatal("Error occured at application.\nException: {0}\nStacktrace: {1}", e.Message, e.StackTrace);
    throw;
}


Console.WriteLine("Stopping...");