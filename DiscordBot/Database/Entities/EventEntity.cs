using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiscordBot.Database.Entities;

[Table("events")]
public class EventEntity : GuildAndIdBaseEntity
{
    public ulong? MentionMessageId { get; set; }

    [MaxLength(256)] public string? Heading { get; set; }

    [MaxLength(4096)] public string? Description { get; set; }

    [MaxLength(300)] public string? ImageUrl { get; set; }

    [MaxLength(300)] public string? ThumbnailUrl { get; set; }

    public TimeSpan? StartTime { get; set; }
    public bool? IsRepeatable { get; set; }
    public DayOfWeek? RepeatDayOfWeek { get; set; }
}