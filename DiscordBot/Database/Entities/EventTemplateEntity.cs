using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiscordBot.Database.Entities;

[Table("event_templates")]
public class EventTemplateEntity : GuildAndIdBaseEntity, IEventData
{
    public EventRepeatabilityEntity? RepeatabilityEntity { get; set; }

    [MaxLength(255)] public string? Title { get; set; }

    [MaxLength(4000)] public string? Description { get; set; }

    public DateTime? CreationDateTime { get; set; }
}