﻿// See https://aka.ms/new-console-template for more information

using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Telegram.Bot;
using Zefi.Telegram.Bot.TimeTracking;

const string EnvironmentPrefix = "ZefiBot_";

var loggerConfiguration = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console();

var logger = loggerConfiguration.CreateLogger();

logger.Information("Logger configured.");

try
{
    var host = Host.CreateDefaultBuilder();
    host.ConfigureAppConfiguration((
        hb,
        cb) =>
    {
        cb.AddUserSecrets(typeof(Program).Assembly, true);
        cb.AddEnvironmentVariables(EnvironmentPrefix);
    });

    host.ConfigureServices(services =>
    {
        services.AddLogging(l => { l.AddSerilog(logger); });
        services.AddSingleton<TelegramBotStarter>();
        services.AddTransient<ActionFactory>();
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