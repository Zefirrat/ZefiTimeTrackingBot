using System.Collections.Concurrent;

namespace Zefi.Telegram.Bot.Services;

public class QueriesStore
{
    private ConcurrentBag<QueryData> HelloStore = new ConcurrentBag<QueryData>();

    public QueriesStore()
    {
    }

    public void AddHello(uint telegramUserId, string queryId)
    {
        HelloStore.Add(new QueryData(telegramUserId, queryId));
    }

    public bool IsHelloQuery(string queryId)
    {
        return HelloStore.Any(q => q.QueryId == queryId);
    }

    private class QueryData
    {
        public QueryData(uint telegramUserId, string queryId)
        {
            TelegramUserId = telegramUserId;
            QueryId = queryId;
            CreatedAt = DateTime.Now;
        }

        public DateTime CreatedAt { get; }

        public uint TelegramUserId { get; }
        public string QueryId { get; }
    }
}