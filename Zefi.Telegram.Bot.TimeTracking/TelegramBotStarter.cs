using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;

namespace Zefi.Telegram.Bot.TimeTracking;

public class TelegramBotStarter
{
    private readonly ILogger<TelegramBotStarter> _logger;
    private readonly TelegramBotClient _botClient;
    private IServiceProvider _serviceProvider;

    public TelegramBotStarter(TelegramBotClient botClient, ILogger<TelegramBotStarter> logger,
        IServiceProvider serviceProvider)
    {
        _botClient = botClient;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    public void StartListen()
    {
        _botClient.StartReceiving(UpdateHandler, PollErrorHandle);
    }

    private async Task PollErrorHandle(
        ITelegramBotClient client,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError("Telegram error handled: {0}", exception.Message);
    }

    private async Task UpdateHandler(
        ITelegramBotClient client,
        Update update,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Message received Id: {0}", update.Id);
        var actionFactory = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ActionFactory>();
        var action = actionFactory.CreateAction(update);
        if (action != null)
        {
            await action.PerformOperation(update);
        }
    }
}