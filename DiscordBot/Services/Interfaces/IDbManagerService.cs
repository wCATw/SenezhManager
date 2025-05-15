using System.Threading.Tasks;
using DiscordBot.Database;

namespace DiscordBot.Services.Interfaces;

public interface IDbManagerService
{
    /// <summary>
    ///     Асинхронно получает первую сущность типа <typeparamref name="T" />,
    ///     наследуемую от <see cref="GuildBaseEntity" />, по идентификатору гильдии.
    /// </summary>
    /// <param name="guildId">Идентификатор гильдии.</param>
    /// <param name="asNoTracking">Определяет, нужно ли использовать AsNoTracking.</param>
    /// <returns>Сущность типа <typeparamref name="T" /> или null, если не найдена.</returns>
    Task<T?> GetGuildBaseEntityAsync<T>(ulong guildId, bool asNoTracking = true) where T : GuildBaseEntity;

    /// <summary>
    ///     Асинхронно получает первую сущность типа <typeparamref name="T" />,
    ///     наследуемую от <see cref="GuildAndIdBaseEntity" />, по идентификатору гильдии и сущности.
    /// </summary>
    /// <param name="guildId">Идентификатор гильдии.</param>
    /// <param name="id">Идентификатор сущности.</param>
    /// <param name="asNoTracking">Определяет, нужно ли использовать AsNoTracking.</param>
    /// <returns>Сущность типа <typeparamref name="T" /> или null, если не найдена.</returns>
    Task<T?> GetGuildAndIdBaseEntityAsync<T>(ulong guildId, ulong id, bool asNoTracking = true)
        where T : GuildAndIdBaseEntity;

    /// <summary>
    ///     Асинхронно получает первую сущность типа <typeparamref name="T" />,
    ///     наследуемую от <see cref="GuildAndUserBaseEntity" />, по идентификатору гильдии и пользователя.
    /// </summary>
    /// <param name="guildId">Идентификатор гильдии.</param>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="asNoTracking">Определяет, нужно ли использовать AsNoTracking.</param>
    /// <returns>Сущность типа <typeparamref name="T" /> или null, если не найдена.</returns>
    Task<T?> GetGuildAndUserBaseEntityAsync<T>(ulong guildId, ulong userId, bool asNoTracking = true)
        where T : GuildAndUserBaseEntity;

    /// <summary>
    ///     Добавляет или обновляет сущность в базе данных.
    /// </summary>
    /// <typeparam name="T">Тип сущности.</typeparam>
    /// <param name="entity">Сущность для добавления или обновления.</param>
    /// <returns>Задача.</returns>
    Task AddOrUpdateAsync<T>(T entity) where T : class;
}