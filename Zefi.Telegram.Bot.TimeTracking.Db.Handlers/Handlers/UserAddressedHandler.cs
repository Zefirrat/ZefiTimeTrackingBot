using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zefi.Telegram.Bot.TimeTracking.Db.Models;
using Zefi.Telegram.Bot.TimeTracking.Mediatr.Models.Notifications;

namespace Zefi.Telegram.Bot.TimeTracking.Db.Handlers.Handlers;

public class UserAddressedHandler : INotificationHandler<UserAddressed>
{
    private readonly ILogger<UserAddressedHandler> _logger;
    private readonly TTDbContext _dbContext;

    public UserAddressedHandler(
        ILogger<UserAddressedHandler> logger,
        TTDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Handle(
        UserAddressed notification,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug($"{nameof(UserAddressedHandler)} triggered.");
        var userExistInDb = await _dbContext.TelegramUsers.AnyAsync(t => t.UserId == notification.UserId,
            cancellationToken: cancellationToken);
        if (userExistInDb)
        {
            return;
        }
        else
        {
            _dbContext.TelegramUsers.Add(
                new TelegramUser(notification.UserId, notification.ChatId.GetValueOrDefault()));
        }
    }
}