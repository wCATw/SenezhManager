using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DiscordBot.Database;
using DiscordBot.Services.Interfaces;
using DiscordBot.Utils;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Services;

public class DbManagerService(AppDbContext dbContext) : IDbManagerService
{
    private bool _disposed;

    public AppDbContext DbContext => dbContext;

    public async Task<T?> GetBaseEntityAsync<T>(int internalId, bool asNoTracking = true, params Expression<Func<T, object>>[] includes) where T : BaseEntity
    {
        var query = ServicesHelper.BuildQuery(dbContext, asNoTracking, includes);

        return await query.FirstOrDefaultAsync(e => e.InternalId == internalId);
    }

    public async Task<T?> GetGuildBaseEntityAsync<T>(ulong guildId, bool asNoTracking = true, params Expression<Func<T, object>>[] includes) where T : GuildBaseEntity
    {
        var query = ServicesHelper.BuildQuery(dbContext, asNoTracking, includes);

        return await query.FirstOrDefaultAsync(e => e.GuildId == guildId);
    }

    public async Task<T?> GetGuildAndIdBaseEntityAsync<T>(ulong guildId, int id, bool asNoTracking = true, params Expression<Func<T, object>>[] includes)
        where T : GuildAndIdBaseEntity
    {
        var query = ServicesHelper.BuildQuery(dbContext, asNoTracking, includes);

        return await query.FirstOrDefaultAsync(e => e.GuildId == guildId && e.Id == id);
    }

    public async Task<T?> GetGuildAndUserBaseEntityAsync<T>(ulong guildId, ulong userId, bool asNoTracking = true, params Expression<Func<T, object>>[] includes)
        where T : GuildAndUserBaseEntity
    {
        var query = ServicesHelper.BuildQuery(dbContext, asNoTracking, includes);

        return await query.FirstOrDefaultAsync(e => e.GuildId == guildId && e.UserId == userId);
    }

    public async Task<List<T>> GetAllGuildBaseEntitiesAsync<T>(ulong guildId, bool asNoTracking = true, params Expression<Func<T, object>>[] includes) where T : GuildBaseEntity
    {
        var query = ServicesHelper.BuildQuery(dbContext, asNoTracking, includes);

        return await query.Where(e => e.GuildId == guildId).ToListAsync();
    }

    public async Task<T?> AddAsync<T>(T entity) where T : BaseEntity
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await dbContext.Set<T>().AddAsync(entity);
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            return entity;
        }
        catch
        {
            await transaction.RollbackAsync();
            return null;
        }
    }

    public async Task<T?> UpdateAsync<T>(T entity) where T : BaseEntity
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var existing = await dbContext.Set<T>().FindAsync(entity.InternalId);
            if (existing == null)
                throw new InvalidOperationException($"Entity with InternalId {entity.InternalId} not found");

            dbContext.Entry(existing).CurrentValues.SetValues(entity);
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            return entity;
        }
        catch
        {
            await transaction.RollbackAsync();
            return null;
        }
    }

    public async Task<bool> DeleteAsync<T>(int internalId) where T : BaseEntity
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var entity = await dbContext.Set<T>().FindAsync(internalId);
            if (entity == null)
                return false;

            dbContext.Set<T>().Remove(entity);
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        dbContext.Dispose();
        _disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        await dbContext.DisposeAsync();
        _disposed = true;
    }
}