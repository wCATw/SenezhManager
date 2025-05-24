using DiscordBot.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Database;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<SettingsEntity> SettingsEntities => Set<SettingsEntity>();
    public DbSet<EventEntity> EventsEntities => Set<EventEntity>();
    public DbSet<EventRepeatabilityEntity> EventRepeatabilityEntities => Set<EventRepeatabilityEntity>();
    public DbSet<EventTemplateEntity> EventTemplateEntities => Set<EventTemplateEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SettingsEntity>(entity =>
        {
            entity.HasKey(x => new { x.GuildId });
            entity.HasIndex(x => new { x.GuildId }).IsUnique();
        });

        modelBuilder.Entity<EventEntity>(entity =>
        {
            entity.HasKey(x => new { x.GuildId, x.Id });
            entity.HasIndex(x => new { x.GuildId, x.Id }).IsUnique();
            entity.HasIndex(x => new { x.GuildId, x.ChannelId, x.MessageId }).IsUnique();
        });

        modelBuilder.Entity<EventTemplateEntity>(entity =>
        {
            entity.HasKey(x => new { x.GuildId, x.Id });
            entity.HasIndex(x => new { x.GuildId, x.Id }).IsUnique();
        });

        modelBuilder.Entity<EventRepeatabilityEntity>(entity =>
        {
            entity.HasKey(x => new { x.GuildId, x.Id });
            entity.HasIndex(x => new { x.GuildId, x.Id }).IsUnique();

            entity.HasOne(x => x.EventEntity)
                .WithOne(x => x.RepeatabilityEntity)
                .HasForeignKey<EventRepeatabilityEntity>(x => new { x.GuildId, x.Id })
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}