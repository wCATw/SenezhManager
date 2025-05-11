using DiscordBot.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Database;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<MemberRankEntity> RanksEntities => Set<MemberRankEntity>();
    public DbSet<MemberStatusEntity> StatusesEntities => Set<MemberStatusEntity>();
    public DbSet<SettingsEntity> SettingsEntities => Set<SettingsEntity>();
    public DbSet<MemberEntity> MemberEntities => Set<MemberEntity>();
    public DbSet<EventEntity> EventEntities => Set<EventEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventEntity>(entity =>
        {
            entity.HasKey(e => new { e.GuildId, e.Id });
            entity.HasIndex(e => new { e.GuildId, e.Id }).IsUnique();
        });

        modelBuilder.Entity<MemberRankEntity>(entity =>
        {
            entity.HasKey(r => new { r.GuildId, r.Name });
            entity.HasIndex(r => new { r.GuildId, r.Name }).IsUnique();
            entity.HasIndex(r => new { r.GuildId, r.Order });

#if DEBUG
            entity.HasData(
                new MemberRankEntity { GuildId = 478923127120723969, Name = "Новобранец", Order = 0 },
                new MemberRankEntity { GuildId = 478923127120723969, Name = "Рядовой", Order = 1 },
                new MemberRankEntity { GuildId = 478923127120723969, Name = "Капрал", Order = 2 },
                new MemberRankEntity { GuildId = 478923127120723969, Name = "Мл Сержант", Order = 3 },
                new MemberRankEntity { GuildId = 478923127120723969, Name = "Сержант", Order = 4 },
                new MemberRankEntity { GuildId = 478923127120723969, Name = "Ст. Сержант", Order = 5 },
                new MemberRankEntity { GuildId = 478923127120723969, Name = "Старшина", Order = 6 },
                new MemberRankEntity { GuildId = 478923127120723969, Name = "Прапорщик", Order = 7 },
                new MemberRankEntity { GuildId = 478923127120723969, Name = "Ст. Прапорщик", Order = 8 },
                new MemberRankEntity { GuildId = 478923127120723969, Name = "Мл. Лейтенант", Order = 9 },
                new MemberRankEntity { GuildId = 478923127120723969, Name = "Лейтенант", Order = 10 },
                new MemberRankEntity { GuildId = 478923127120723969, Name = "Ст. Лейтенант", Order = 11 },
                new MemberRankEntity { GuildId = 478923127120723969, Name = "Капитан", Order = 12 },
                new MemberRankEntity { GuildId = 478923127120723969, Name = "Майор", Order = 13 },
                new MemberRankEntity { GuildId = 478923127120723969, Name = "Подполковник", Order = 14 },
                new MemberRankEntity { GuildId = 478923127120723969, Name = "Полковник", Order = 15 });
#endif
        });

        modelBuilder.Entity<MemberStatusEntity>(entity =>
        {
            entity.HasKey(s => new { s.GuildId, s.Name });
            entity.HasIndex(s => new { s.GuildId, s.Name }).IsUnique();

#if DEBUG
            entity.HasData(
                new MemberStatusEntity { GuildId = 478923127120723969, Name = "Неизвестно" },
                new MemberStatusEntity { GuildId = 478923127120723969, Name = "Активен" },
                new MemberStatusEntity { GuildId = 478923127120723969, Name = "В отпуске" },
                new MemberStatusEntity { GuildId = 478923127120723969, Name = "В резерве" },
                new MemberStatusEntity { GuildId = 478923127120723969, Name = "Выбыл" }
            );
#endif
        });

        modelBuilder.Entity<SettingsEntity>(entity =>
        {
            entity.HasKey(s => s.GuildId);
            entity.HasIndex(s => s.GuildId).IsUnique();

#if DEBUG
            entity.HasData(
                new SettingsEntity { GuildId = 478923127120723969, RegistrationChannelId = 1201393133292896286 }
            );
#endif
        });

        modelBuilder.Entity<MemberEntity>(entity =>
        {
            entity.HasKey(m => new { m.GuildId, m.MemberId });
            entity.HasIndex(m => new { m.GuildId, m.MemberId }).IsUnique();

            entity.HasOne(m => m.Status)
                .WithMany()
                .HasForeignKey(m => new { m.GuildId, m.StatusName })
                .HasPrincipalKey(s => new { s.GuildId, s.Name })
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.Rank)
                .WithMany()
                .HasForeignKey(m => new { m.GuildId, m.RankName })
                .HasPrincipalKey(r => new { r.GuildId, r.Name })
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}