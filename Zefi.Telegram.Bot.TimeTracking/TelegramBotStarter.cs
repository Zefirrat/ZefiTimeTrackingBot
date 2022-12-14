using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;
using Zefi.Telegram.Bot.TimeTracking.Db;

namespace Zefi.Telegram.Bot.TimeTracking;

public class TelegramBotStarter
{
    private readonly ILogger<TelegramBotStarter> _logger;
    private readonly TelegramBotClient _botClient;
    private IServiceProvider _serviceProvider;

    public TelegramBotStarter(
        TelegramBotClient botClient,
        ILogger<TelegramBotStarter> logger,
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
        try
        {
            _logger.LogInformation("Message received Id: {0}", update.Id);
            using var serviceScope = _serviceProvider.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<TTDbContext>();
            var actionFactory = serviceScope
                .ServiceProvider.GetRequiredService<ActionFactory>();
            var action = await actionFactory.CreateAction(update, cancellationToken);
            if (action != null)
            {
                await action.PerformOperation(update, cancellationToken);
            }
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured on processing update handler.");
        }
    }
}