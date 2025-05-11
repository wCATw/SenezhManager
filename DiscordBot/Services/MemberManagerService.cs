using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.Database;
using DiscordBot.Database.Entities;
using DiscordBot.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Services;

public class MemberManagerService(AppDbContext dbContext) : IMemberManagerService
{
    public async Task<bool> TryChangeMemberAsync(ulong guildId, ulong memberId, string? newCallsign,
        string? newRankName, string? newStatusName)
    {
        try
        {
            var member = await dbContext.MemberEntities
                .FirstOrDefaultAsync(m => m.GuildId == guildId && m.MemberId == memberId);

            if (member == null)
            {
                member = new MemberEntity
                {
                    GuildId = guildId,
                    MemberId = memberId
                };
                dbContext.MemberEntities.Add(member);
            }

            if (!string.IsNullOrWhiteSpace(newCallsign))
                member.CallSign = newCallsign;

            if (!string.IsNullOrWhiteSpace(newStatusName))
            {
                var status = await dbContext.StatusesEntities
                    .FirstOrDefaultAsync(s => s.GuildId == guildId && s.Name == newStatusName);
                if (status is null) return false;

                member.StatusName = newStatusName;
                member.Status = status;
            }

            if (!string.IsNullOrWhiteSpace(newRankName))
            {
                var rank = await dbContext.RanksEntities
                    .FirstOrDefaultAsync(r => r.GuildId == guildId && r.Name == newRankName);
                if (rank is null) return false;

                member.RankName = newRankName;
                member.Rank = rank;
            }

            await dbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> TryAddStatusAsync(ulong guildId, string statusName)
    {
        if (await dbContext.StatusesEntities.AnyAsync(s => s.GuildId == guildId && s.Name == statusName))
            return false;

        dbContext.StatusesEntities.Add(new MemberStatusEntity { GuildId = guildId, Name = statusName });
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TryRemoveStatusAsync(ulong guildId, string statusName)
    {
        var status = await dbContext.StatusesEntities
            .FirstOrDefaultAsync(s => s.GuildId == guildId && s.Name == statusName);
        if (status is null) return false;

        dbContext.StatusesEntities.Remove(status);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TryChangeStatusAsync(ulong guildId, string statusName, string? newStatusName)
    {
        var status = await dbContext.StatusesEntities
            .FirstOrDefaultAsync(s => s.GuildId == guildId && s.Name == statusName);
        if (status is null || string.IsNullOrWhiteSpace(newStatusName)) return false;

        status.Name = newStatusName;
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TryAddRankAsync(ulong guildId, string rankName, int order)
    {
        if (await dbContext.RanksEntities.AnyAsync(r => r.GuildId == guildId && r.Name == rankName))
            return false;

        dbContext.RanksEntities.Add(new MemberRankEntity
        {
            GuildId = guildId,
            Name = rankName,
            Order = order
        });

        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TryRemoveRankAsync(ulong guildId, string rankName)
    {
        var rank = await dbContext.RanksEntities
            .FirstOrDefaultAsync(r => r.GuildId == guildId && r.Name == rankName);
        if (rank is null) return false;

        dbContext.RanksEntities.Remove(rank);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TryChangeRankAsync(ulong guildId, string rankName, string? newRankName, int? newOrder)
    {
        var rank = await dbContext.RanksEntities
            .FirstOrDefaultAsync(r => r.GuildId == guildId && r.Name == rankName);
        if (rank is null) return false;

        if (!string.IsNullOrWhiteSpace(newRankName))
            rank.Name = newRankName;

        if (newOrder.HasValue)
            rank.Order = newOrder.Value;

        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TryAddMemberAsync(ulong guildId, ulong memberId, string? callSign, string? rankName,
        string? statusName)
    {
        if (await dbContext.MemberEntities.AnyAsync(m => m.GuildId == guildId && m.MemberId == memberId))
            return false;

        var member = new MemberEntity
        {
            GuildId = guildId,
            MemberId = memberId,
            CallSign = callSign,
            RankName = rankName,
            StatusName = statusName
        };

        if (!string.IsNullOrWhiteSpace(rankName))
        {
            var rank =
                await dbContext.RanksEntities.FirstOrDefaultAsync(r => r.GuildId == guildId && r.Name == rankName);
            if (rank is null) return false;
            member.Rank = rank;
        }

        if (!string.IsNullOrWhiteSpace(statusName))
        {
            var status =
                await dbContext.StatusesEntities.FirstOrDefaultAsync(s => s.GuildId == guildId && s.Name == statusName);
            if (status is null) return false;
            member.Status = status;
        }

        dbContext.MemberEntities.Add(member);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TryRemoveMemberAsync(ulong guildId, ulong memberId)
    {
        var member =
            await dbContext.MemberEntities.FirstOrDefaultAsync(m => m.GuildId == guildId && m.MemberId == memberId);
        if (member == null) return false;

        dbContext.MemberEntities.Remove(member);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<List<MemberEntity>> GetAllMembersAsync(ulong guildId)
    {
        return await dbContext.MemberEntities
            .Where(m => m.GuildId == guildId)
            .Include(m => m.Rank)
            .Include(m => m.Status)
            .ToListAsync();
    }

    public async Task<List<MemberStatusEntity>> GetAllStatusesAsync(ulong guildId)
    {
        return await dbContext.StatusesEntities.Where(s => s.GuildId == guildId).ToListAsync();
    }

    public async Task<List<MemberRankEntity>> GetAllRanksAsync(ulong guildId)
    {
        return await dbContext.RanksEntities.Where(r => r.GuildId == guildId).OrderBy(r => r.Order).ToListAsync();
    }
}