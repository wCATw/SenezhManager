using System;
using System.Threading.Tasks;
using DiscordBot.Database;
using DiscordBot.Database.Entities;
using DiscordBot.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Services;

public class MemberManagerService(AppDbContext dbContext) : IMemberManagerService
{
    public async Task<MemberEntity?> GetMemberById(ulong guildId, ulong memberId)
    {
        return await dbContext.MemberEntities
            .Include(m => m.Status)
            .Include(m => m.Rank)
            .FirstOrDefaultAsync(m => m.GuildId == guildId && m.MemberId == memberId);
    }

    public async Task<ulong> CreateMember(ulong guildId, MemberEntity? memberEnt = null)
    {
        var entity = memberEnt ?? new MemberEntity();
        entity.GuildId ??= guildId;

        if (entity.MemberId == null)
            throw new ArgumentException("MemberId must be set on the entity or provided externally.");

        dbContext.MemberEntities.Add(entity);
        await dbContext.SaveChangesAsync();
        return entity.MemberId.Value;
    }

    public async Task UpdateMember(ulong guildId, ulong memberId, MemberEntity? memberEnt = null)
    {
        var existing = await dbContext.MemberEntities
            .FirstOrDefaultAsync(m => m.GuildId == guildId && m.MemberId == memberId);

        if (existing == null) throw new InvalidOperationException("Member not found.");

        var properties = typeof(MemberEntity).GetProperties();
        foreach (var prop in properties)
        {
            if (prop.Name is nameof(MemberEntity.GuildId) or nameof(MemberEntity.MemberId)) continue;

            var newValue = prop.GetValue(memberEnt);
            if (newValue != null)
                prop.SetValue(existing, newValue);
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task<MemberStatus?> GetStatusByName(ulong guildId, string name)
    {
        return await dbContext.StatusesEntities.FirstOrDefaultAsync(s =>
            s.GuildId == guildId && s.Name!.ToLower() == name.ToLower());
    }

    public async Task<MemberRank?> GetRankByName(ulong guildId, string name)
    {
        return await dbContext.RanksEntities.FirstOrDefaultAsync(r =>
            r.GuildId == guildId && r.Name!.ToLower() == name.ToLower());
    }
}