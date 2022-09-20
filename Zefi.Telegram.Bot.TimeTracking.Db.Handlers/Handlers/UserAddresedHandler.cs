using MediatR;
using Zefi.Telegram.Bot.TimeTracking.Mediatr.Models.Notifications;

namespace Zefi.Telegram.Bot.TimeTracking.Db.Handlers.Handlers;

public class UserAddresedHandler : INotificationHandler<UserAddressed>
{
    public async Task Handle(
        UserAddressed notification,
        CancellationToken cancellationToken)
    {
        
    }
}