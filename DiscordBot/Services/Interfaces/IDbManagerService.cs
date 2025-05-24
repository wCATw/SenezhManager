using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBot.Database.Entities;
using DiscordBot.Services.Structs;

namespace DiscordBot.Services.Interfaces;

public interface IDbManagerService
{
    /// <summary>
    ///     Асинхронно получает первую сущность типа <typeparamref name="T" />,
    ///     наследуемую от <see cref="GuildBaseEntity" />, по идентификатору гильдии.
    /// </summary>
    /// <param name="guildId">Идентификатор гильдии.</param>
    /// <param name="asNoTracking">Определяет, нужно ли использовать AsNoTracking.</param>
    /// <typeparam name="T">Тип сущности.</typeparam>
    /// <returns>Сущность типа <typeparamref name="T" /> или null, если не найдена.</returns>
    Task<DbResult<T?>> GetGuildBasedAsync<T>(ulong guildId, bool asNoTracking = true) where T : GuildBaseEntity;

    /// <summary>
    ///     Асинхронно получает первую сущность типа <typeparamref name="T" />,
    ///     наследуемую от <see cref="GuildAndIdBaseEntity" />, по идентификатору гильдии и сущности.
    /// </summary>
    /// <param name="guildId">Идентификатор гильдии.</param>
    /// <param name="id">Идентификатор сущности.</param>
    /// <param name="asNoTracking">Определяет, нужно ли использовать AsNoTracking.</param>
    /// <typeparam name="T">Тип сущности.</typeparam>
    /// <returns>Сущность типа <typeparamref name="T" /> или null, если не найдена.</returns>
    Task<DbResult<T?>> GetGuildAndIdBasedAsync<T>(ulong guildId, ulong id, bool asNoTracking = true)
        where T : GuildAndIdBaseEntity;

    /// <summary>
    ///     Асинхронно получает первую сущность типа <typeparamref name="T" />,
    ///     наследуемую от <see cref="GuildAndUserBaseEntity" />, по идентификатору гильдии и пользователя.
    /// </summary>
    /// <param name="guildId">Идентификатор гильдии.</param>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="asNoTracking">Определяет, нужно ли использовать AsNoTracking.</param>
    /// <typeparam name="T">Тип сущности.</typeparam>
    /// <returns>Сущность типа <typeparamref name="T" /> или null, если не найдена.</returns>
    Task<DbResult<T?>> GetGuildAndUserBasedAsync<T>(ulong guildId, ulong userId, bool asNoTracking = true)
        where T : GuildAndUserBaseEntity;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId">Идентификатор гильдии.</param>
    /// <param name="asNoTracking">Определяет, нужно ли использовать AsNoTracking.</param>
    /// <typeparam name="T">Тип сущности.</typeparam>
    /// <returns></returns>
    Task<DbResult<List<T>?>> GetAllGuildBasedAsync<T>(ulong guildId, bool asNoTracking = true)
        where T : GuildBaseEntity;

    /// <summary>
    ///     Добавляет или обновляет сущность в базе данных.
    /// </summary>
    /// <param name="entity">Сущность для добавления или обновления.</param>
    /// <typeparam name="T">Тип сущности.</typeparam>
    /// <returns></returns>
    Task<DbResult<bool>> AddOrUpdateAsync<T>(T entity) where T : class;
}