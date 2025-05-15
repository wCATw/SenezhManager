using System.Linq;
using System.Threading.Tasks;
using DiscordBot.Database;
using DiscordBot.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Services;

public class DbManagerService(AppDbContext dbContext) : IDbManagerService
{
    public async Task<T?> GetGuildBaseEntityAsync<T>(ulong guildId, bool asNoTracking = true) where T : GuildBaseEntity
    {
        var query = dbContext.Set<T>().AsQueryable();

        if (asNoTracking) query = query.AsNoTracking();

        var entity = await query.FirstOrDefaultAsync(x => x.GuildId == guildId);

        return entity;
    }

    public async Task<T?> GetGuildAndIdBaseEntityAsync<T>(ulong guildId, ulong id, bool asNoTracking = true)
        where T : GuildAndIdBaseEntity
    {
        var query = dbContext.Set<T>().AsQueryable();

        if (asNoTracking) query = query.AsNoTracking();

        var entity = await query.FirstOrDefaultAsync(x => x.GuildId == guildId && x.Id == id);

        return entity;
    }

    public async Task<T?> GetGuildAndUserBaseEntityAsync<T>(ulong guildId, ulong userId, bool asNoTracking = true)
        where T : GuildAndUserBaseEntity
    {
        var query = dbContext.Set<T>().AsQueryable();

        if (asNoTracking) query = query.AsNoTracking();

        var entity = await query.FirstOrDefaultAsync(x => x.GuildId == guildId && x.UserId == userId);

        return entity;
    }

    public async Task AddOrUpdateAsync<T>(T entity) where T : class
    {
        var dbSet = dbContext.Set<T>();
        var entry = dbContext.Entry(entity);

        if (entry.State == EntityState.Detached)
        {
            var primaryKey = dbContext.Model
                .FindEntityType(typeof(T))?
                .FindPrimaryKey()?
                .Properties
                .Select(p => p.Name)
                .FirstOrDefault();

            if (primaryKey is not null)
            {
                var keyProperty = typeof(T).GetProperty(primaryKey);
                var keyValue = keyProperty?.GetValue(entity);

                if (keyValue != null)
                {
                    var existingEntity = await dbSet.FindAsync(keyValue);

                    if (existingEntity is null)
                        await dbSet.AddAsync(entity);
                    else
                        dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
                }
                else
                {
                    await dbSet.AddAsync(entity);
                }
            }
            else
            {
                await dbSet.AddAsync(entity);
            }
        }

        await dbContext.SaveChangesAsync();
    }
}