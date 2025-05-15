using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Database;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<SettingsEntity> SettingsEntities => Set<SettingsEntity>();
    public DbSet<EventEntity> EventEntities => Set<EventEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SettingsEntity>(entity =>
        {
            entity.HasKey(x => new { x.GuildId });
            entity.HasIndex(x => new { x.GuildId }).IsUnique();
        });
    }
}

public abstract class GuildBaseEntity
{
    public ulong? GuildId { get; set; }
}

public abstract class GuildAndIdBaseEntity : GuildBaseEntity
{
    public ulong? Id { get; set; }
}

public abstract class GuildAndUserBaseEntity : GuildBaseEntity
{
    public ulong? UserId { get; set; }
}

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

[Table("settings")]
public class SettingsEntity : GuildBaseEntity
{
    public byte? ExpireEventMessageDelayMinutes { get; set; }
}