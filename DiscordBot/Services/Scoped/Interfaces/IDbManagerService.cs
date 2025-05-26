using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBot.Database.Entities;

namespace DiscordBot.Services.Scoped.Interfaces;

public interface IDbManagerService
{
    Task<T?> GetGuildBaseAsync<T>(ulong guildId, bool asNoTracking = true) where T : GuildBaseEntity;

    Task<T?> GetGuildAndIdBaseAsync<T>(ulong guildId, int id, bool asNoTracking = true)
        where T : GuildAndIdBaseEntity;

    Task<T?> GetGuildAndUserBaseAsync<T>(ulong guildId, ulong userId, bool asNoTracking = true)
        where T : GuildAndUserBaseEntity;

    Task<List<T>?> GetAllGuildBaseAsync<T>(ulong guildId, bool asNoTracking = true)
        where T : GuildBaseEntity;

    Task<T?> AddAsync<T>(T entity) where T : class;

    Task<T?> UpdateAsync<T>(T entity) where T : class;

    Task<T?> DeleteAsync<T>(T entity) where T : class;
}