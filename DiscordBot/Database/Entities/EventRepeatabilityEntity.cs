using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiscordBot.Database.Entities;

[Table("event_repeatability")]
public class EventRepeatabilityEntity : GuildAndIdBaseEntity
{
    public DayOfWeek? DayOfWeek { get; set; }
    public TimeSpan? Time { get; set; }
    public EventTemplateEntity? EventEntity { get; set; }
}