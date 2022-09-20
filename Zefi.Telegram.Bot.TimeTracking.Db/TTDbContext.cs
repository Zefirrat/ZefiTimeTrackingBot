using Microsoft.EntityFrameworkCore;
using Zefi.Telegram.Bot.TimeTracking.Db.Models;

namespace Zefi.Telegram.Bot.TimeTracking.Db;

public class TTDbContext : DbContext
{
    public TTDbContext(
        DbContextOptions<TTDbContext> context) :
        base(context)
    {
    }

    public DbSet<MessageTemplate>  MessageTemplates  { get; private set; }
    public DbSet<TelegramUser>     TelegramUsers     { get; private set; }
    public DbSet<TimeTrackingInfo> TimeTrackingInfos { get; private set; }
}