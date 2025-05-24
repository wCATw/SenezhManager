using System.Threading.Tasks;
using DiscordBot.Database.Entities;
using DiscordBot.Services.Scoped.Interfaces;

namespace DiscordBot.Services.Scoped;

public class SettingsManagerService(IDbManagerService dbManager) : ISettingsManagerService
{
    public async Task<SettingsEntity> GetSettingsAsync(ulong guildId, bool asNoTracking = true)
    {
        var result = await dbManager.GetGuildBasedAsync<SettingsEntity>(guildId, asNoTracking);
        if (!result.IsSuccess)
            throw result.Exception!;

        var entity = result.Result;

        if (entity == null)
        {
            entity = new SettingsEntity { GuildId = guildId };

            var addResult = await dbManager.AddOrUpdateAsync(entity);
            if (!addResult.IsSuccess)
                throw addResult.Exception!;
        }

        return entity;
    }

    public async Task<bool> TryUpdateSettingsAsync(SettingsEntity settingsEnt)
    {
        if (settingsEnt.GuildId == null)
            return false;

        var result = await GetSettingsAsync(settingsEnt.GuildId.Value, false);
        if (result == null)
            return false;

        var properties = typeof(SettingsEntity).GetProperties();
        foreach (var prop in properties)
        {
            if (prop.Name == nameof(SettingsEntity.GuildId)) continue;

            var newValue = prop.GetValue(settingsEnt);
            if (newValue != null)
                prop.SetValue(result, newValue);
        }

        var updateResult = await dbManager.AddOrUpdateAsync(result);
        return updateResult.IsSuccess;
    }
}