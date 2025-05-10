using System.Threading.Tasks;
using DiscordBot.Database;
using DiscordBot.Database.Entities;
using DiscordBot.Services.Interfaces;

namespace DiscordBot.Services;

public class SettingsManagerService(AppDbContext dbContext) : ISettingsManagerService
{
    public Task<SettingsEntity?> GetSettings(ulong guildId)
    {
        throw new System.NotImplementedException();
    }

    public Task UpdateSettings(ulong guildId, SettingsEntity? settingsEnt = null)
    {
        throw new System.NotImplementedException();
    }
}