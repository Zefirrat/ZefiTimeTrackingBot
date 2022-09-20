using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;

namespace Zefi.Telegram.Bot.TimeTracking.Actions;

public class InlineQueryProcess : ProcessAction
{
    private readonly ILogger<InlineQueryProcess> _logger;
    private readonly TelegramBotClient _botClient;

    public InlineQueryProcess(
        ILogger<InlineQueryProcess> logger,
        TelegramBotClient botClient)
    {
        _logger = logger;
        _botClient = botClient;
    }

    public override async Task PerformOperation(
        Update update)
    {
        _logger.LogDebug(update.InlineQuery.Query);
        await _botClient.AnswerInlineQueryAsync(update.InlineQuery.Id,
            new List<InlineQueryResult>()
            {
                new InlineQueryResultArticle(update.InlineQuery.Id,
                    $"{update.InlineQuery.Query}\n{DateTime.Now:t}",
                    new InputTextMessageContent($"{update.InlineQuery.Query}\n{DateTime.Now:t}"))
            },
            isPersonal: false);
    }
}