using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBot.Database.Entities;

namespace DiscordBot.Services.Interfaces;

public interface IMemberManagerService
{
    Task<bool> TryAddMemberAsync(ulong guildId, ulong memberId, string? callSign, string? rankName, string? statusName);
    Task<bool> TryRemoveMemberAsync(ulong guildId, ulong memberId);

    Task<bool> TryChangeMemberAsync(ulong guildId, ulong memberId, string? newCallsign, string? newRankName,
        string? newStatusName);

    Task<List<MemberEntity>> GetAllMembersAsync(ulong guildId);

    Task<bool> TryAddStatusAsync(ulong guildId, string statusName);
    Task<bool> TryRemoveStatusAsync(ulong guildId, string statusName);
    Task<bool> TryChangeStatusAsync(ulong guildId, string statusName, string? newStatusName);
    Task<List<MemberStatusEntity>> GetAllStatusesAsync(ulong guildId);

    Task<bool> TryAddRankAsync(ulong guildId, string rankName, int order);
    Task<bool> TryRemoveRankAsync(ulong guildId, string rankName);
    Task<bool> TryChangeRankAsync(ulong guildId, string rankName, string? newRankName, int? newOrder);
    Task<List<MemberRankEntity>> GetAllRanksAsync(ulong guildId);
}