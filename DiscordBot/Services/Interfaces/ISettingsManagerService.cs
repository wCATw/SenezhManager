using System.Threading.Tasks;
using DiscordBot.Database.Entities;

namespace DiscordBot.Services.Interfaces;

public interface ISettingsManagerService
{
    /// <summary>
    /// Асинхронно получает настройки для указанной гильдии. 
    /// Если настроек нет — создаёт новый экземпляр, сохраняет в базу и возвращает его.
    /// </summary>
    /// <param name="guildId">Идентификатор гильдии.</param>
    /// <param name="asNoTracking">Определяет, нужно ли использовать AsNoTracking.</param>
    /// <returns>Настройки гильдии или вновь созданный экземпляр.</returns>
    Task<SettingsEntity> GetSettingsAsync(ulong guildId, bool asNoTracking = true);


    /// <summary>
    /// Асинхронно обновляет свойства настроек гильдии, если они не null.
    /// </summary>
    /// <param name="settingsEnt">Обновлённый экземпляр сущности с новыми значениями.</param>
    /// <returns>True, если обновление прошло успешно; иначе — false.</returns>
    Task<bool> TryUpdateSettingsAsync(SettingsEntity settingsEnt);
}