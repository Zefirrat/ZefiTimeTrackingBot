using System.Text.RegularExpressions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.InlineQueryResults;
using Zefi.Telegram.Bot.TimeTracking.Mediatr.Models.RequestsResponses;

namespace Zefi.Telegram.Bot.TimeTracking.Db.Handlers.Handlers;

public class SuggestInlineHandler : IRequestHandler<SuggestInlineRequest, SuggestInlineResponse>
{
    private readonly TTDbContext _dbContext;
    private readonly TelegramBotClient _botClient;
    private readonly ILogger<SuggestInlineHandler> _logger;

    public SuggestInlineHandler(TTDbContext dbContext,
        TelegramBotClient botClient,
        ILogger<SuggestInlineHandler> logger)
    {
        _dbContext = dbContext;
        _botClient = botClient;
        _logger = logger;
    }

    public async Task<SuggestInlineResponse> Handle(SuggestInlineRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var inlineQueryResults = await GetInlineResults(request.Message, request.TelegramUserId);
            await _botClient.AnswerInlineQueryAsync(request.QueryId,
                inlineQueryResults.Item1,
                isPersonal: false, cacheTime: (int?)TimeSpan.FromSeconds(10).TotalSeconds,
                cancellationToken: cancellationToken, switchPmParameter: "sdasdasd");
            return new SuggestInlineResponse(true, inlineQueryResults.Item2);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Exception when processing {nameof(SuggestInlineHandler)}.");
        }

            return new SuggestInlineResponse(false, string.Empty);
    }

    private async Task<Tuple<List<InlineQueryResult>, string>> GetInlineResults(string message, uint telegramUserId)
    {
        const string regexToCutMessageFromCommand = @"(?>(^\/hello)|(^\/bye))[ ]{0,1}(?<message>.*)";
        var regex = new Regex(regexToCutMessageFromCommand);
        var cuttedMessage = regex.Matches(message).FirstOrDefault(m => m.Groups.ContainsKey("message"))
            ?.Groups["message"].Value ?? message;

        var utcNowToLocal = DateTime.UtcNow.ToLocalTime();
        var helloMessageText = $"{cuttedMessage}\nВремя начала: {utcNowToLocal:hh:mm:ss tt zz}";
        var helloMessage = new InputTextMessageContent(helloMessageText);

        var lastHello= await _dbContext.TelegramUsers.Where(t=>t.UserId == telegramUserId).Select(t=>t.TimeTrackingInfo.LastHelloSend).SingleAsync();
        var timeSpan = lastHello != default ? DateTime.UtcNow.Subtract(lastHello) : TimeSpan.Zero;

        var byeMessageText = $"{cuttedMessage}\nДлительность: {timeSpan:hh':'mm}\nВремя окончания: {utcNowToLocal:hh:mm:ss tt zz}";
        var byeMessage = new InputTextMessageContent(byeMessageText);

        if (message.StartsWith("/hello"))
        {
            var inlineQueryHello = new InlineQueryResultArticle(new { helloMessage, telegramUserId }.GetHashCode().ToString(),
                helloMessageText,
                helloMessage);
            return new Tuple<List<InlineQueryResult>, string>(new List<InlineQueryResult>()
            {
                inlineQueryHello
            }, inlineQueryHello.Id);
        }
        
        if (message.StartsWith("/bye"))
        {
            var inlineQueryBye = new InlineQueryResultArticle(new { byeMessage, telegramUserId }.GetHashCode().ToString(),
                byeMessageText,
                byeMessage);
            return new Tuple<List<InlineQueryResult>, string>(new List<InlineQueryResult>()
            {
                inlineQueryBye
            }, null);
        }

        var inlineHelloQuery = new InlineQueryResultArticle(new { helloMessage, telegramUserId }.GetHashCode().ToString(), "/hello",
            helloMessage);
        return new Tuple<List<InlineQueryResult>, string>(new List<InlineQueryResult>()
        {
            inlineHelloQuery,
            new InlineQueryResultArticle(new { byeMessage, telegramUserId }.GetHashCode().ToString(), "/bye",
                byeMessage)
        }, inlineHelloQuery.Id);
    }
}