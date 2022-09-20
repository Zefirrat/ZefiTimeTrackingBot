using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Zefi.Telegram.Bot.TimeTracking.Actions;

namespace Zefi.Telegram.Bot.TimeTracking;

public class ActionFactory
{
    private readonly IServiceProvider _serviceProvider;
    public ActionFactory (IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ProcessAction CreateAction(Update update)
    {
        switch (update.Type)
        {
            case UpdateType.Unknown:
                break;
            case UpdateType.Message:
                break;
            case UpdateType.InlineQuery:
                return ActivatorUtilities.CreateInstance<InlineQueryProcess>(_serviceProvider);
                break;
            case UpdateType.ChosenInlineResult:
                break;
            case UpdateType.CallbackQuery:
                break;
            case UpdateType.EditedMessage:
                break;
            case UpdateType.ChannelPost:
                break;
            case UpdateType.EditedChannelPost:
                break;
            case UpdateType.ShippingQuery:
                break;
            case UpdateType.PreCheckoutQuery:
                break;
            case UpdateType.Poll:
                break;
            case UpdateType.PollAnswer:
                break;
            case UpdateType.MyChatMember:
                break;
            case UpdateType.ChatMember:
                break;
            case UpdateType.ChatJoinRequest:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return null;
    }
}