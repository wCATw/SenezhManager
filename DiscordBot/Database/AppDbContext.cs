using DiscordBot.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Database;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SettingsEntity>()
            .HasKey(x => x.GuildId);
        
        modelBuilder.Entity<MemberEntity>()
            .HasKey(x => new { x.GuildId, x.MemberId});
        
        modelBuilder.Entity<EventEntity>()
            .HasKey(x => new { x.GuildId, x.Id});
        
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<SettingsEntity> SettingsEntities { get; set; }
    public DbSet<MemberEntity> MemberEntities { get; set; }
    public DbSet<EventEntity> EventEntities { get; set; }
    
}