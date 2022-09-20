using MediatR;

namespace Zefi.Telegram.Bot.TimeTracking.Mediatr.Models.Notifications;

public class UserAddressed : INotification
{
    public UserAddressed(
        long userId,
        long chatId)
    {
        UserId = userId;
        ChatId = chatId;
    }

    public long UserId { get; private set; }
    public long ChatId { get; private set; }
}