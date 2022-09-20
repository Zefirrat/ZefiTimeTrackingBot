using System.ComponentModel.DataAnnotations;

namespace Zefi.Telegram.Bot.TimeTracking.Db.Models;

public class MessageTemplate
{
    private MessageTemplate()
    {
    }

    public MessageTemplate(
        string template, TemplateType type)
    {
        Template = template;
        Type = type;
    }

    [Key] public Guid         Id       { get; private set; }
    public       string       Template { get; internal set; }
    public       TemplateType Type     { get; internal set; }
}