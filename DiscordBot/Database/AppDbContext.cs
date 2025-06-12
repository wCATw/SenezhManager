using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscordBot.Database;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<SettingsEntity> SettingsEntities => Set<SettingsEntity>();
    public DbSet<EventEntity> EventsEntities => Set<EventEntity>();
    public DbSet<EventRepeatabilityEntity> EventRepeatabilityEntities => Set<EventRepeatabilityEntity>();
    public DbSet<EventTemplateEntity> EventTemplateEntities => Set<EventTemplateEntity>();

    private void GuildBaseEntityConfiguration<T>(EntityTypeBuilder<T> entity) where T : GuildBaseEntity
    {
        entity.HasKey(x => x.InternalId);
        entity.HasIndex(x => x.GuildId).IsUnique();
    }

    private void GuildAndIdBaseEntityConfiguration<T>(EntityTypeBuilder<T> entity) where T : GuildAndIdBaseEntity
    {
        entity.HasKey(x => x.InternalId);
        entity.HasIndex(x => new { x.GuildId, x.Id }).IsUnique();
    }

    private void GuildAndUserBaseEntityConfiguration<T>(EntityTypeBuilder<T> entity) where T : GuildAndUserBaseEntity
    {
        entity.HasKey(x => x.InternalId);
        entity.HasIndex(x => new { x.GuildId, x.UserId }).IsUnique();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SettingsEntity>(GuildBaseEntityConfiguration);

        modelBuilder.Entity<EventEntity>(GuildAndIdBaseEntityConfiguration);

        modelBuilder.Entity<EventRepeatabilityEntity>(entity =>
        {
            GuildAndIdBaseEntityConfiguration(entity);
            entity.HasOne(e => e.EventTemplateEntity)
                  .WithOne(t => t.EventRepeatabilityEntity)
                  .HasForeignKey<EventRepeatabilityEntity>(e => e.Id)
                  .HasPrincipalKey<EventTemplateEntity>(t => t.Id);
        });

        modelBuilder.Entity<EventTemplateEntity>(GuildAndIdBaseEntityConfiguration);

        base.OnModelCreating(modelBuilder);
    }
}

#region Base

public abstract class BaseEntity
{
    [Display(Name = "SYSTEM")] public int? InternalId { get; set; }
}

public abstract class GuildBaseEntity : BaseEntity
{
    [Display(Name = "SYSTEM")] public ulong? GuildId { get; set; }
}

public abstract class GuildAndIdBaseEntity : GuildBaseEntity
{
    [Display(Name = "SYSTEM")] public int? Id { get; set; }
}

public abstract class GuildAndUserBaseEntity : GuildBaseEntity
{
    [Display(Name = "SYSTEM")] public ulong? UserId { get; set; }
}

public interface IEventData
{
    [MaxLength(255)] public string? Title { get; set; }

    [MaxLength(4000)] public string? Description { get; set; }
    public DateTime? CreationDateTime { get; set; }
}

#endregion

#region Events

[Table("events")]
public class EventEntity : GuildAndIdBaseEntity, IEventData
{
    public DateTime? EventDateTime { get; set; }
    public ulong? MessageId { get; set; }
    public bool? Notified { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? CreationDateTime { get; set; }
}

[Table("event_repeatability")]
public class EventRepeatabilityEntity : GuildAndIdBaseEntity
{
    public DayOfWeek? DayOfWeek { get; set; }
    public TimeSpan? Time { get; set; }
    public EventTemplateEntity? EventTemplateEntity { get; set; }
}

[Table("event_templates")]
public class EventTemplateEntity : GuildAndIdBaseEntity, IEventData
{
    public EventRepeatabilityEntity? EventRepeatabilityEntity { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? CreationDateTime { get; set; }
}

#endregion

#region Settings

[Table("settings")]
public class SettingsEntity : GuildBaseEntity
{
    [Display(Name = "ID Канала оповещений и расписания событий")]
    public ulong? EventNotificationChannelId { get; set; }

    [Display(Name = "HIDDEN")]
    public ulong? ScheduleMessageId { get; set; }
}

#endregion