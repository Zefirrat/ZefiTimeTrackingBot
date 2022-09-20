using System.ComponentModel.DataAnnotations;

namespace Zefi.Telegram.Bot.TimeTracking.Db.Models;

public class TelegramUser
{
    private TelegramUser()
    {
    }

    public TelegramUser(
        long userId,
        long chatId)
    {
        ChatId = (uint)chatId;
        UserId = (uint)userId;
        MessageTemplatesList = new List<MessageTemplate>();
        TimeTrackingInfo = new TimeTrackingInfo();
    }

    [Key] public Guid                         Id                   { get; private set; }
    public       uint                         ChatId               { get; internal set; }
    public       uint                         UserId               { get; internal set; }
    public       IEnumerable<MessageTemplate> MessageTemplatesList { get; internal set; }
    public       TimeTrackingInfo             TimeTrackingInfo     { get; internal set; }
}