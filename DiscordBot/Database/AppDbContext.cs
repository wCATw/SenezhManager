using DiscordBot.Database.Entities;
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

        modelBuilder.Entity<EventEntity>(entity =>
        {
            entity.HasKey(x => new { x.GuildId, x.Id });
            entity.HasIndex(x => new { x.GuildId, x.Id }).IsUnique();
        });
    }
}