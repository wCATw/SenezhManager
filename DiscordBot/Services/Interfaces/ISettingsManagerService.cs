using System.Threading.Tasks;
using DiscordBot.Database.Entities;

namespace DiscordBot.Services.Interfaces;

public interface ISettingsManagerService
{
    /// <summary>
    /// Получает из базы данных запись настроек для указанного сервера.
    /// Если запись не найдена — создаёт новую и возвращает её.
    /// </summary>
    /// <param name="guildId">ID сервера (гильдии).</param>
    /// <param name="asNoTracking">Если true — возвращает сущность без трекинга изменений (read-only).</param>
    /// <returns>Сущность настроек сервера.</returns>
    Task<SettingsEntity?> GetSettingsAsync(ulong guildId, bool asNoTracking = false);

    /// <summary>
    /// Обновляет существующую запись настроек для указанного сервера.
    /// </summary>
    /// <param name="guildId">ID сервера (гильдии).</param>
    /// <param name="settingsEnt">Сущность с новыми значениями полей.</param>
    /// <returns>True, если обновление прошло успешно.</returns>
    Task<bool> TryUpdateSettingsAsync(ulong guildId, SettingsEntity settingsEnt);
}