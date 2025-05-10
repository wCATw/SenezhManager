using System.Threading.Tasks;
using DiscordBot.Database;
using DiscordBot.Database.Entities;
using DiscordBot.Services.Interfaces;

namespace DiscordBot.Services;

public class MemberManagerService(AppDbContext dbContext) : IMemberManagerService
{
    public Task<MemberEntity?> GetMemberById(ulong guildId, ulong memberId)
    {
        throw new System.NotImplementedException();
    }

    public Task<ulong> CreateMember(ulong guildId, MemberEntity? memberEnt = null)
    {
        throw new System.NotImplementedException();
    }

    public Task UpdateMember(ulong guildId, ulong memberId, MemberEntity? memberEnt = null)
    {
        throw new System.NotImplementedException();
    }
}