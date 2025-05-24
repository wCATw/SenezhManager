using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.Database;
using DiscordBot.Database.Entities;
using DiscordBot.Services.Interfaces;
using DiscordBot.Services.Structs;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Services;

public class DbManagerService(AppDbContext dbContext) : IDbManagerService
{
    public async Task<DbResult<T?>> GetGuildBasedAsync<T>(ulong guildId, bool asNoTracking = true)
        where T : GuildBaseEntity
    {
        try
        {
            var query = dbContext.Set<T>().AsQueryable();
            if (asNoTracking) query = query.AsNoTracking();

            var entity = await query.FirstOrDefaultAsync(x => x.GuildId == guildId);
            return new DbResult<T?>(entity);
        }
        catch (Exception ex)
        {
            return new DbResult<T?>(ex);
        }
    }

    public async Task<DbResult<T?>> GetGuildAndIdBasedAsync<T>(ulong guildId, ulong id, bool asNoTracking = true)
        where T : GuildAndIdBaseEntity
    {
        try
        {
            var query = dbContext.Set<T>().AsQueryable();
            if (asNoTracking) query = query.AsNoTracking();

            var entity = await query.FirstOrDefaultAsync(x => x.GuildId == guildId && x.Id == id);
            return new DbResult<T?>(entity);
        }
        catch (Exception ex)
        {
            return new DbResult<T?>(ex);
        }
    }

    public async Task<DbResult<T?>> GetGuildAndUserBasedAsync<T>(ulong guildId, ulong userId,
        bool asNoTracking = true)
        where T : GuildAndUserBaseEntity
    {
        try
        {
            var query = dbContext.Set<T>().AsQueryable();
            if (asNoTracking) query = query.AsNoTracking();

            var entity = await query.FirstOrDefaultAsync(x => x.GuildId == guildId && x.UserId == userId);
            return new DbResult<T?>(entity);
        }
        catch (Exception ex)
        {
            return new DbResult<T?>(ex);
        }
    }

    public async Task<DbResult<List<T>?>> GetAllGuildBasedAsync<T>(ulong guildId, bool asNoTracking = true)
        where T : GuildBaseEntity
    {
        try
        {
            var query = dbContext.Set<T>().AsQueryable();
            if (asNoTracking) query = query.AsNoTracking();

            var list = await query.Where(x => x.GuildId == guildId).ToListAsync();
            return new DbResult<List<T>?>(list);
        }
        catch (Exception ex)
        {
            return new DbResult<List<T>?>(ex);
        }
    }

    public async Task<DbResult<bool>> AddOrUpdateAsync<T>(T entity) where T : class
    {
        try
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
            return new DbResult<bool>(true);
        }
        catch (Exception ex)
        {
            return new DbResult<bool>(ex);
        }
    }
}