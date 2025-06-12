using System;
using System.Threading.Tasks;
using DiscordBot.Database;
using DiscordBot.Services.Interfaces;
using DiscordBot.Utils;

namespace DiscordBot.Services;

public class SettingsManagerService(IDbManagerService dbManager) : ISettingsManagerService
{
    private bool _disposed;

    public async Task<SettingsEntity> GetSettingsAsync(ulong guildId, bool asNoTracking = true)
    {
        var entity = await dbManager.GetGuildBaseEntityAsync<SettingsEntity>(guildId, asNoTracking);

        if (entity != null)
            return entity;

        entity = new SettingsEntity { GuildId = guildId };
        await dbManager.AddAsync(entity);

        return entity;
    }

    public async Task<bool> TryUpdateSettingsAsync(SettingsEntity settingsEnt)
    {
        if (settingsEnt.GuildId == null)
            throw new ArgumentException("GuildId is null!");

        var entity = await GetSettingsAsync(settingsEnt.GuildId.Value, false);

        ServicesHelper.PatchEntity(settingsEnt, ref entity);

        var result = await dbManager.UpdateAsync(entity);

        return result != null;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        dbManager.Dispose();
        _disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        await dbManager.DisposeAsync();
        _disposed = true;
    }
}