using Telegram.Bot.Types;

namespace Zefi.Telegram.Bot.TimeTracking.Actions;

public abstract class ProcessAction
{
    public ProcessAction() {}

    public abstract Task PerformOperation(
        Update update,
        CancellationToken cancellationToken);
}