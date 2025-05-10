using DiscordBot.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Database;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<MemberRank> RanksEntities { get; set; }
    public DbSet<MemberStatus> StatusesEntities { get; set; }
    public DbSet<SettingsEntity> SettingsEntities { get; set; }
    public DbSet<MemberEntity> MemberEntities { get; set; }
    public DbSet<EventEntity> EventEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SettingsEntity>()
            .HasKey(x => x.GuildId);

        modelBuilder.Entity<MemberStatus>(entity =>
        {
            entity.HasKey(s => new { s.GuildId, s.Id });

            entity.Property(s => s.GuildId).IsRequired();
            entity.Property(s => s.Id).IsRequired();
            entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
#if DEBUG
            entity.HasData(
                new MemberStatus { GuildId = 478923127120723969, Id = 0, Name = "Неизвестно" },
                new MemberStatus { GuildId = 478923127120723969, Id = 1, Name = "Активен" },
                new MemberStatus { GuildId = 478923127120723969, Id = 2, Name = "В отпуске" },
                new MemberStatus { GuildId = 478923127120723969, Id = 3, Name = "В резерве" },
                new MemberStatus { GuildId = 478923127120723969, Id = 4, Name = "Выбыл" }
            );
#endif
        });

        modelBuilder.Entity<MemberRank>(entity =>
        {
            entity.HasKey(r => new { r.GuildId, r.Id });

            entity.Property(r => r.GuildId).IsRequired();
            entity.Property(r => r.Id).IsRequired();
            entity.Property(r => r.Name).IsRequired().HasMaxLength(100);
            entity.Property(r => r.Order).IsRequired();
#if DEBUG
            entity.HasData(
                new MemberRank { GuildId = 478923127120723969, Id = 0, Name = "Новобранец", Order = 0 },
                new MemberRank { GuildId = 478923127120723969, Id = 1, Name = "Рядовой", Order = 1 },
                new MemberRank { GuildId = 478923127120723969, Id = 2, Name = "Капрал", Order = 2 },
                new MemberRank { GuildId = 478923127120723969, Id = 3, Name = "Мл Сержант", Order = 3 },
                new MemberRank { GuildId = 478923127120723969, Id = 4, Name = "Сержант", Order = 4 },
                new MemberRank { GuildId = 478923127120723969, Id = 5, Name = "Ст. Сержант", Order = 5 },
                new MemberRank { GuildId = 478923127120723969, Id = 6, Name = "Старшина", Order = 6 },
                new MemberRank { GuildId = 478923127120723969, Id = 7, Name = "Прапорщик", Order = 7 },
                new MemberRank { GuildId = 478923127120723969, Id = 8, Name = "Ст. Прапорщик", Order = 8 },
                new MemberRank { GuildId = 478923127120723969, Id = 9, Name = "Мл. Лейтенант", Order = 9 },
                new MemberRank { GuildId = 478923127120723969, Id = 10, Name = "Лейтенант", Order = 10 },
                new MemberRank { GuildId = 478923127120723969, Id = 11, Name = "Ст. Лейтенант", Order = 11 },
                new MemberRank { GuildId = 478923127120723969, Id = 12, Name = "Капитан", Order = 12 },
                new MemberRank { GuildId = 478923127120723969, Id = 13, Name = "Майор", Order = 13 },
                new MemberRank { GuildId = 478923127120723969, Id = 14, Name = "Подполковник", Order = 14 },
                new MemberRank { GuildId = 478923127120723969, Id = 15, Name = "Полковник", Order = 15 }
            );
#endif
        });

        modelBuilder.Entity<MemberEntity>(entity =>
        {
            entity.HasKey(m => new { m.GuildId, m.MemberId });

            entity.Property(m => m.GuildId).IsRequired();
            entity.Property(m => m.MemberId).IsRequired();

            entity.HasOne(m => m.Status)
                .WithMany()
                .HasForeignKey(m => new { m.GuildId, m.StatusId })
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.Rank)
                .WithMany()
                .HasForeignKey(m => new { m.GuildId, m.RankId })
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(m => m.CallSign).HasMaxLength(50);

            modelBuilder.Entity<EventEntity>()
                .HasKey(x => new { x.GuildId, x.Id });

            base.OnModelCreating(modelBuilder);
        });
    }
}