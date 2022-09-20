using MediatR;

namespace Zefi.Telegram.Bot.TimeTracking.Mediatr.Models.RequestsResponses;

public class SuggestInlineRequest : IRequest<SuggestInlineResponse>
{
    public SuggestInlineRequest(uint telegramUserId, string message, string queryId)
    {
        TelegramUserId = telegramUserId;
        Message = message;
        QueryId = queryId;
    }
    
    public uint TelegramUserId { get; }
    public string QueryId { get; }
    public string Message { get; }
}

public class SuggestInlineResponse
{
    public SuggestInlineResponse(bool success)
    {
        Success = success;
    }

    public bool Success { get; }
}