using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;
using Zefi.Telegram.Bot.Services;
using Zefi.Telegram.Bot.TimeTracking.Mediatr.Models.RequestsResponses;

namespace Zefi.Telegram.Bot.TimeTracking.Actions;

public class InlineQueryProcess : ProcessAction
{
    private readonly ILogger<InlineQueryProcess> _logger;
    private readonly IMediator _mediator;
    private readonly QueriesStore _queriesStore;
    private readonly TelegramBotClient _botClient;

    public InlineQueryProcess(
        ILogger<InlineQueryProcess> logger, IMediator mediator, QueriesStore queriesStore)
    {
        _logger = logger;
        _mediator = mediator;
        _queriesStore = queriesStore;
    }

    public override async Task PerformOperation(
        Update update,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug(update.InlineQuery.Query);
        var fromId = (uint)update.InlineQuery.From.Id;
        var suggestInlineResponse = await _mediator.Send(new SuggestInlineRequest(fromId, update.InlineQuery.Query, update.InlineQuery.Id));
        if (suggestInlineResponse.Success && !string.IsNullOrEmpty(suggestInlineResponse.HelloQueryId))
        {
            _queriesStore.AddHello(fromId, suggestInlineResponse.HelloQueryId);
        }
    }
}