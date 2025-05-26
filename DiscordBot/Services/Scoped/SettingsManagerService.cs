using System.Threading.Tasks;
using DiscordBot.Database.Entities;
using DiscordBot.Services.Scoped.Interfaces;

namespace DiscordBot.Services.Scoped;

public class SettingsManagerService(IDbManagerService dbManager) : ISettingsManagerService
{
    public async Task<SettingsEntity> GetSettingsAsync(ulong guildId, bool asNoTracking = true)
    {
        var entity = await dbManager.GetGuildBaseAsync<SettingsEntity>(guildId, asNoTracking);

        if (entity == null)
        {
            entity = new SettingsEntity { GuildId = guildId };
            await dbManager.AddAsync(entity);
        }

        return entity;
    }

    public async Task<bool> TryUpdateSettingsAsync(SettingsEntity settingsEnt)
    {
        if (settingsEnt.GuildId == null)
            return false;

        var entity = await GetSettingsAsync(settingsEnt.GuildId.Value, false);

        var properties = typeof(SettingsEntity).GetProperties();
        foreach (var prop in properties)
        {
            if (prop.Name == nameof(SettingsEntity.GuildId)) continue;

            var newValue = prop.GetValue(settingsEnt);
            if (newValue != null)
                prop.SetValue(entity, newValue);
        }

        var result = await dbManager.UpdateAsync(entity);

        return result != null;
    }
}