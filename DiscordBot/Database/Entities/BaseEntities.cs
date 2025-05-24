using System;
using System.ComponentModel.DataAnnotations;

namespace DiscordBot.Database.Entities;

public abstract class GuildBaseEntity
{
    [Display(Name = "SYSTEM")] public ulong? GuildId { get; set; }
}

public abstract class GuildAndIdBaseEntity : GuildBaseEntity
{
    [Display(Name = "SYSTEM")] public ulong? Id { get; set; }
}

public abstract class GuildAndUserBaseEntity : GuildBaseEntity
{
    [Display(Name = "SYSTEM")] public ulong? UserId { get; set; }
}

public interface IEventData
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? CreationDateTime { get; set; }
}