using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiscordBot.Database.Entities;

[Table("event_templates")]
public class EventTemplateEntity : GuildAndIdBaseEntity, IEventData
{
    public EventRepeatabilityEntity? RepeatabilityEntity { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? CreationDateTime { get; set; }
}