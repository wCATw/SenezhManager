using System.Threading.Tasks;
using DiscordBot.Database;
using DiscordBot.Database.Entities;
using DiscordBot.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Services;

public class SettingsManagerService(AppDbContext dbContext) : ISettingsManagerService
{
    public async Task<SettingsEntity?> GetSettingsAsync(ulong guildId, bool asNoTracking = true)
    {
        var query = dbContext.SettingsEntities.AsQueryable();

        if (asNoTracking) query = query.AsNoTracking();

        var entity = await query.FirstOrDefaultAsync(x => x.GuildId == guildId);

        if (entity == null)
        {
            entity = new SettingsEntity
            {
                GuildId = guildId
            };
            dbContext.SettingsEntities.Add(entity);
            await dbContext.SaveChangesAsync();
        }

        return entity;
    }

    public async Task<bool> TryUpdateSettingsAsync(ulong guildId, SettingsEntity settingsEnt)
    {
        var entity = await GetSettingsAsync(guildId, false);

        var properties = typeof(SettingsEntity).GetProperties();
        foreach (var prop in properties)
        {
            if (prop.Name == nameof(SettingsEntity.GuildId)) continue;

            var newValue = prop.GetValue(settingsEnt);
            if (newValue != null) prop.SetValue(entity, newValue);
        }

        await dbContext.SaveChangesAsync();
        return true;
    }
}