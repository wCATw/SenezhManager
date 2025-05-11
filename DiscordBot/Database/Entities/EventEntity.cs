using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiscordBot.Database.Entities;

[Table("events")]
public class EventEntity
{
    public ulong? GuildId { get; set; }
    public ulong? Id { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }
    public TimeSpan? StartTimeSpan { get; set; }
}