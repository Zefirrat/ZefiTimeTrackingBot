using System.ComponentModel.DataAnnotations;

namespace Zefi.Telegram.Bot.TimeTracking.Db.Models;

public class TimeTrackingInfo
{
    public TimeTrackingInfo(){}
    
    [Key]
    public Guid Id { get; private set; }
    public DateTime LastHelloSend { get; internal set; }

    public void RefreshLastHelloSend()
    {
        LastHelloSend = DateTime.UtcNow;
    }
}