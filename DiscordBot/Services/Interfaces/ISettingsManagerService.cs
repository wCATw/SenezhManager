using System;
using System.Threading.Tasks;
using DiscordBot.Database;

namespace DiscordBot.Services.Interfaces;

/// <summary>
/// 
/// </summary>
public interface ISettingsManagerService : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="asNoTracking"></param>
    /// <returns></returns>
    Task<SettingsEntity> GetSettingsAsync(ulong guildId, bool asNoTracking = true);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="settingsEnt"></param>
    /// <returns></returns>
    Task<bool> TryUpdateSettingsAsync(SettingsEntity settingsEnt);
}