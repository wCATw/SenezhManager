using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.Database;
using DiscordBot.Database.Entities;
using DiscordBot.Services.Scoped.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Services.Scoped;

public class DbManagerService(AppDbContext dbContext) : IDbManagerService
{
    public async Task<T?> GetGuildBaseAsync<T>(ulong guildId, bool asNoTracking = true)
        where T : GuildBaseEntity
    {
        var query = dbContext.Set<T>().AsQueryable();

        if (asNoTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(e => e.GuildId == guildId);
    }

    public async Task<T?> GetGuildAndIdBaseAsync<T>(ulong guildId, int id, bool asNoTracking = true)
        where T : GuildAndIdBaseEntity
    {
        var query = dbContext.Set<T>().AsQueryable();

        if (asNoTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(e => e.GuildId == guildId && e.Id == id);
    }

    public async Task<T?> GetGuildAndUserBaseAsync<T>(ulong guildId, ulong userId, bool asNoTracking = true)
        where T : GuildAndUserBaseEntity
    {
        var query = dbContext.Set<T>().AsQueryable();

        if (asNoTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(e => e.GuildId == guildId && e.UserId == userId);
    }

    public async Task<List<T>?> GetAllGuildBaseAsync<T>(ulong guildId, bool asNoTracking = true)
        where T : GuildBaseEntity
    {
        var query = dbContext.Set<T>().AsQueryable();

        if (asNoTracking)
            query = query.AsNoTracking();

        return await query.Where(e => e.GuildId == guildId).ToListAsync();
    }

    public async Task<T?> AddAsync<T>(T entity)
        where T : class
    {
        var entry = await dbContext.Set<T>().AddAsync(entity);
        await dbContext.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<T?> UpdateAsync<T>(T entity)
        where T : class
    {
        dbContext.Set<T>().Update(entity);
        await dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<T?> DeleteAsync<T>(T entity)
        where T : class
    {
        dbContext.Set<T>().Remove(entity);
        await dbContext.SaveChangesAsync();
        return entity;
    }
}