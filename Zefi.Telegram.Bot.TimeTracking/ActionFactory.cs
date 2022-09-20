using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Zefi.Telegram.Bot.TimeTracking.Actions;
using Zefi.Telegram.Bot.TimeTracking.Mediatr.Models.Notifications;

namespace Zefi.Telegram.Bot.TimeTracking;

public class ActionFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMediator _mediator;

    public ActionFactory (IServiceProvider serviceProvider, IMediator mediator)
    {
        _serviceProvider = serviceProvider;
        _mediator = mediator;
    }

    public async Task<ProcessAction?> CreateAction(
        Update update,
        CancellationToken cancellationToken)
    {
        switch (update.Type)
        {
            case UpdateType.Unknown:
                break;
            case UpdateType.Message:
                break;
            case UpdateType.InlineQuery:
                await _mediator.Publish(new UserAddressed(update.InlineQuery.From.Id, null), cancellationToken);
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