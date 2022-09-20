using System.Text.RegularExpressions;
using MediatR;
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
            await _botClient.AnswerInlineQueryAsync(request.QueryId,
                await GetInlineResults(request.Message),
                isPersonal: false,
                cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Exception when processing {nameof(SuggestInlineHandler)}.");
            return new SuggestInlineResponse(false);
        }

        return new SuggestInlineResponse(true);
    }

    private async Task<List<InlineQueryResult>> GetInlineResults(string message)
    {
        const string regexToCutMessageFromCommand = @"(?>(^\/hello)|(^\/bye))[ ]{0,1}(?<message>.*)";
        var regex = new Regex(regexToCutMessageFromCommand);
        var cuttedMessage = regex.Matches(message).FirstOrDefault(m=>m.Groups.ContainsKey("message"))?.Groups["message"].Value ?? message;
        
        var helloMessage = new InputTextMessageContent($"{cuttedMessage}\nВремя начала: {DateTime.Now:t}");
        var byeMessage = new InputTextMessageContent($"{cuttedMessage}\nВремя окончания: {DateTime.Now:t}");
        
        if (message.StartsWith("/hello"))
        {
            var title = $"{cuttedMessage}\n{DateTime.Now:t}";
            return new List<InlineQueryResult>()
            {
                new InlineQueryResultArticle(title.GetHashCode().ToString(),
                    title,
                    helloMessage)
            };
        }

        return new List<InlineQueryResult>()
        {
            new InlineQueryResultArticle(helloMessage.GetHashCode().ToString(), "/hello", helloMessage),
            new InlineQueryResultArticle(byeMessage.GetHashCode().ToString(), "/bye", byeMessage)
        };
    }
}