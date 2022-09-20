using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;
using Zefi.Telegram.Bot.TimeTracking.Mediatr.Models.RequestsResponses;

namespace Zefi.Telegram.Bot.TimeTracking.Actions;

public class InlineQueryProcess : ProcessAction
{
    private readonly ILogger<InlineQueryProcess> _logger;
    private readonly IMediator _mediator;
    private readonly TelegramBotClient _botClient;

    public InlineQueryProcess(
        ILogger<InlineQueryProcess> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public override async Task PerformOperation(
        Update update,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug(update.InlineQuery.Query);
        await _mediator.Send(new SuggestInlineRequest((uint)update.InlineQuery.From.Id, update.InlineQuery.Query, update.InlineQuery.Id));
    }
}