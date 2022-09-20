using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Zefi.Telegram.Bot.Services;
using Zefi.Telegram.Bot.TimeTracking.Db;

namespace Zefi.Telegram.Bot.TimeTracking.Actions;

public class ChoosenInlineResultProcess :ProcessAction
{
    private ILogger<ChoosenInlineResultProcess> _logger;
    private readonly TTDbContext _dbContext;
    private readonly QueriesStore _queriesStore;

    public ChoosenInlineResultProcess(ILogger<ChoosenInlineResultProcess> logger, TTDbContext dbContext, QueriesStore queriesStore)
    {
        _logger = logger;
        _dbContext = dbContext;
        _queriesStore = queriesStore;
    }

    public override async Task PerformOperation(Update update, CancellationToken cancellationToken)
    {
        var fromId = update.ChosenInlineResult.From.Id;
        _logger.LogDebug($"Shipping query from {0}, with id {1}", fromId, update.ChosenInlineResult.ResultId);

        if (_queriesStore.IsHelloQuery(update.ChosenInlineResult.ResultId))
        {
            var user = await _dbContext.TelegramUsers.Include(t => t.TimeTrackingInfo)
                .SingleAsync(t => t.UserId == fromId, cancellationToken: cancellationToken);
            user.TimeTrackingInfo.RefreshLastHelloSend();
        }
    }
}