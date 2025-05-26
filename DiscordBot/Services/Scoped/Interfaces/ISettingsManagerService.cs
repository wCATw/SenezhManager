using System.Threading.Tasks;
using DiscordBot.Database.Entities;

namespace DiscordBot.Services.Scoped.Interfaces;

public interface ISettingsManagerService
{
    Task<SettingsEntity> GetSettingsAsync(ulong guildId, bool asNoTracking = true);

    Task<bool> TryUpdateSettingsAsync(SettingsEntity settingsEnt);
}