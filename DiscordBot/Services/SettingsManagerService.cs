using System.Threading.Tasks;
using DiscordBot.Database;
using DiscordBot.Services.Interfaces;

namespace DiscordBot.Services;

public class SettingsManagerService(IDbManagerService dbManager) : ISettingsManagerService
{
    public async Task<SettingsEntity?> GetSettingsAsync(ulong guildId, bool asNoTracking = true)
    {
        var entity = await dbManager.GetGuildBaseEntityAsync<SettingsEntity>(guildId, asNoTracking);

        if (entity == null)
        {
            entity = new SettingsEntity
            {
                GuildId = guildId
            };

            await dbManager.AddOrUpdateAsync(entity);
        }

        return entity;
    }

    public async Task<bool> TryUpdateSettingsAsync(SettingsEntity settingsEnt)
    {
        if (settingsEnt.GuildId == null)
            return false;

        var entity = await GetSettingsAsync(settingsEnt.GuildId.Value, false);
        if (entity == null)
            return false;

        var properties = typeof(SettingsEntity).GetProperties();
        foreach (var prop in properties)
        {
            if (prop.Name == nameof(SettingsEntity.GuildId)) continue;

            var newValue = prop.GetValue(settingsEnt);
            if (newValue != null)
                prop.SetValue(entity, newValue);
        }

        await dbManager.AddOrUpdateAsync(entity);
        return true;
    }
}