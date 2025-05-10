using System.Threading.Tasks;
using DiscordBot.Database.Entities;

namespace DiscordBot.Services.Interfaces;

public interface ISettingsManagerService
{
    /// <summary>
    ///  Берет из БД запись настроек. Если не существует, создает новую пустую и берет её.
    /// </summary>
    /// <param name="guildId">ID сервера из контекста</param>
    /// <returns>Сущность целевой записи настроек</returns>
    Task<SettingsEntity?> GetSettingsAsync(ulong guildId, bool asNoTracking = false);

    /// <summary>
    ///  Обновляет существующую запись настроек.
    /// </summary>
    /// <param name="guildId">ID сервера из контекста</param>
    /// <param name="settingsEnt">Сущность с данными</param>
    Task<bool> TryUpdateSettingsAsync(ulong guildId, SettingsEntity settingsEnt);
}