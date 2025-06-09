using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DiscordBot.Database;

namespace DiscordBot.Services.Interfaces;

/// <summary>
///     Интерфейс для управления базовыми сущностями в базе данных.
///     Обеспечивает получение, добавление, обновление и удаление сущностей с поддержкой включения зависимостей.
/// </summary>
public interface IDbManagerService : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// 
    /// </summary>
    AppDbContext DbContext { get; }

    /// <summary>
    ///     Получает сущность по внутреннему идентификатору.
    /// </summary>
    /// <typeparam name="T">Тип сущности, производной от <see cref="BaseEntity" />.</typeparam>
    /// <param name="internalId">Внутренний идентификатор сущности.</param>
    /// <param name="asNoTracking">Если true — сущность будет загружена без отслеживания изменений.</param>
    /// <param name="includes">Свойства для загрузки связанных данных.</param>
    /// <returns>Сущность типа <typeparamref name="T" /> или null, если не найдена.</returns>
    Task<T?> GetBaseEntityAsync<T>(int internalId, bool asNoTracking = true, params Expression<Func<T, object>>[] includes) where T : BaseEntity;

    /// <summary>
    ///     Получает сущность по идентификатору сервера (guild).
    /// </summary>
    /// <typeparam name="T">Тип сущности, производной от <see cref="GuildBaseEntity" />.</typeparam>
    /// <param name="guildId">Идентификатор сервера Discord.</param>
    /// <param name="asNoTracking">Если true — сущность будет загружена без отслеживания изменений.</param>
    /// <param name="includes">Свойства для загрузки связанных данных.</param>
    /// <returns>Сущность типа <typeparamref name="T" /> или null, если не найдена.</returns>
    Task<T?> GetGuildBaseEntityAsync<T>(ulong guildId, bool asNoTracking = true, params Expression<Func<T, object>>[] includes) where T : GuildBaseEntity;

    /// <summary>
    ///     Получает сущность по идентификатору сервера и внутреннему ID.
    /// </summary>
    /// <typeparam name="T">Тип сущности, производной от <see cref="GuildAndIdBaseEntity" />.</typeparam>
    /// <param name="guildId">Идентификатор сервера Discord.</param>
    /// <param name="id">Внутренний идентификатор сущности.</param>
    /// <param name="asNoTracking">Если true — сущность будет загружена без отслеживания изменений.</param>
    /// <param name="includes">Свойства для загрузки связанных данных.</param>
    /// <returns>Сущность типа <typeparamref name="T" /> или null, если не найдена.</returns>
    Task<T?> GetGuildAndIdBaseEntityAsync<T>(ulong guildId, int id, bool asNoTracking = true, params Expression<Func<T, object>>[] includes) where T : GuildAndIdBaseEntity;

    /// <summary>
    ///     Получает сущность по идентификатору сервера и пользователя.
    /// </summary>
    /// <typeparam name="T">Тип сущности, производной от <see cref="GuildAndUserBaseEntity" />.</typeparam>
    /// <param name="guildId">Идентификатор сервера Discord.</param>
    /// <param name="userId">Идентификатор пользователя Discord.</param>
    /// <param name="asNoTracking">Если true — сущность будет загружена без отслеживания изменений.</param>
    /// <param name="includes">Свойства для загрузки связанных данных.</param>
    /// <returns>Сущность типа <typeparamref name="T" /> или null, если не найдена.</returns>
    Task<T?> GetGuildAndUserBaseEntityAsync<T>(ulong guildId, ulong userId, bool asNoTracking = true, params Expression<Func<T, object>>[] includes)
        where T : GuildAndUserBaseEntity;

    /// <summary>
    ///     Получает список всех сущностей, связанных с определённым сервером (guild).
    /// </summary>
    /// <typeparam name="T">Тип сущности, производной от <see cref="GuildBaseEntity" />.</typeparam>
    /// <param name="guildId">Идентификатор сервера Discord.</param>
    /// <param name="asNoTracking">Если true — сущности будут загружены без отслеживания изменений.</param>
    /// <param name="includes">Свойства для загрузки связанных данных.</param>
    /// <returns>Список сущностей типа <typeparamref name="T" />.</returns>
    Task<List<T>> GetAllGuildBaseEntitiesAsync<T>(ulong guildId, bool asNoTracking = true, params Expression<Func<T, object>>[] includes) where T : GuildBaseEntity;

    /// <summary>
    ///     Добавляет новую сущность в базу данных.
    /// </summary>
    /// <typeparam name="T">Тип сущности, производной от <see cref="BaseEntity" />.</typeparam>
    /// <param name="entity">Сущность для добавления.</param>
    /// <returns>Добавленная сущность.</returns>
    Task<T?> AddAsync<T>(T entity) where T : BaseEntity;

    /// <summary>
    ///     Обновляет существующую сущность в базе данных.
    /// </summary>
    /// <typeparam name="T">Тип сущности, производной от <see cref="BaseEntity" />.</typeparam>
    /// <param name="entity">Сущность с новыми значениями.</param>
    /// <returns>Обновлённая сущность.</returns>
    Task<T?> UpdateAsync<T>(T entity) where T : BaseEntity;

    /// <summary>
    ///     Удаляет сущность по внутреннему идентификатору.
    /// </summary>
    /// <typeparam name="T">Тип сущности, производной от <see cref="BaseEntity" />.</typeparam>
    /// <param name="internalId">Внутренний идентификатор сущности.</param>
    /// <returns>True, если сущность была успешно удалена; иначе — false.</returns>
    Task<bool> DeleteAsync<T>(int internalId) where T : BaseEntity;
}