using System;
using System.Threading.Tasks;
using DiscordBot.Database;

namespace DiscordBot.Services.Scoped.Interfaces;

public interface ISettingsManagerService : IDisposable, IAsyncDisposable
{
    Task<SettingsEntity> GetSettingsAsync(ulong guildId, bool asNoTracking = true);

    Task<bool> TryUpdateSettingsAsync(SettingsEntity settingsEnt);
}