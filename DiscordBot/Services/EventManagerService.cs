using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DiscordBot.Database;
using DiscordBot.Services.Interfaces;
using DiscordBot.Utils;

namespace DiscordBot.Services;

public class EventManagerService(IDbManagerService dbManager) : IEventManagerService
{
    private bool _disposed;

    public async Task<bool> TryAddEventTemplateAsync(EventTemplateEntity eventTemplateEnt)
    {
        if (eventTemplateEnt.GuildId == null)
            return false;

        eventTemplateEnt.Id = await ServicesHelper.GenerateNextIdForGuild<EventEntity>(dbManager.DbContext, eventTemplateEnt.GuildId.Value);

        var result = await dbManager.AddAsync(eventTemplateEnt);
        return result != null;
    }

    public async Task<bool> TryUpdateEventTemplateAsync(EventTemplateEntity eventTemplateEnt)
    {
        if (eventTemplateEnt.GuildId == null || eventTemplateEnt.Id == null)
            return false;

        var entity = await GetEventTemplateAsync(eventTemplateEnt.GuildId.Value, eventTemplateEnt.Id.Value, false, false);

        if (entity?.InternalId == null)
            return false;

        ServicesHelper.PatchEntity(eventTemplateEnt, ref entity);

        var result = await dbManager.UpdateAsync(entity);
        return result != null;
    }

    public async Task<EventTemplateEntity?> GetEventTemplateAsync(ulong guildId, int eventTemplateId, bool includeRepeatability = false, bool asNoTracking = true)
    {
        var includes = includeRepeatability
            ? new Expression<Func<EventTemplateEntity, object>>[] { e => e.EventRepeatabilityEntity! }
            : Array.Empty<Expression<Func<EventTemplateEntity, object>>>();

        return await dbManager.GetGuildAndIdBaseEntityAsync(guildId, eventTemplateId, asNoTracking, includes);
    }

    public async Task<List<EventTemplateEntity>> GetEventTemplatesAsync(ulong guildId)
    {
        return await dbManager.GetAllGuildBaseEntitiesAsync<EventTemplateEntity>(guildId, true);
    }

    public async Task<bool> TryDeleteEventTemplateAsync(ulong guildId, int eventTemplateId)
    {
        var entity = await GetEventTemplateAsync(guildId, eventTemplateId, false, false);

        if (entity?.InternalId == null)
            return false;

        return await dbManager.DeleteAsync<EventTemplateEntity>(entity.InternalId.Value);
    }

    public async Task<bool> TryAddEventAsync(EventEntity eventEnt)
    {
        if (eventEnt.GuildId == null)
            return false;

        eventEnt.Id = await ServicesHelper.GenerateNextIdForGuild<EventEntity>(dbManager.DbContext, eventEnt.GuildId.Value);

        var result = await dbManager.AddAsync(eventEnt);
        return result != null;
    }

    public async Task<bool> TryUpdateEventAsync(EventEntity eventEnt)
    {
        if (eventEnt.GuildId == null || eventEnt.Id == null)
            return false;

        var entity = await GetEventAsync(eventEnt.GuildId.Value, eventEnt.Id.Value, false);

        if (entity?.InternalId == null)
            return false;

        ServicesHelper.PatchEntity(eventEnt, ref entity);

        var result = await dbManager.UpdateAsync(entity);
        return result != null;
    }

    public async Task<EventEntity?> GetEventAsync(ulong guildId, int eventId, bool asNoTracking = true)
    {
        return await dbManager.GetGuildAndIdBaseEntityAsync<EventEntity>(guildId, eventId, asNoTracking);
    }

    public async Task<List<EventEntity>> GetEventsAsync(ulong guildId)
    {
        return await dbManager.GetAllGuildBaseEntitiesAsync<EventEntity>(guildId);
    }

    public async Task<bool> TryDeleteEventAsync(ulong guildId, int eventId)
    {
        var entity = await GetEventAsync(guildId, eventId, false);

        if (entity?.InternalId == null)
            return false;

        return await dbManager.DeleteAsync<EventEntity>(entity.InternalId.Value);
    }

    public async Task<bool> TryAddEventRepeatabilityAsync(EventRepeatabilityEntity eventRepeatEnt)
    {
        if (eventRepeatEnt.GuildId == null || eventRepeatEnt.Id == null)
            return false;

        var result = await dbManager.AddAsync(eventRepeatEnt);
        return result != null;
    }

    public async Task<bool> TryUpdateEventRepeatabilityAsync(EventRepeatabilityEntity eventRepeatEnt)
    {
        if (eventRepeatEnt.GuildId == null || eventRepeatEnt.Id == null)
            return false;

        var entity = await GetEventRepeatabilityAsync(eventRepeatEnt.GuildId.Value, eventRepeatEnt.Id.Value, false, false);

        if (entity?.InternalId == null)
            return false;

        ServicesHelper.PatchEntity(eventRepeatEnt, ref entity);

        var result = await dbManager.UpdateAsync(entity);
        return result != null;
    }

    public async Task<EventRepeatabilityEntity?> GetEventRepeatabilityAsync(ulong guildId, int eventTemplateId, bool includeTemplate = false, bool asNoTracking = true)
    {
        var includes = includeTemplate
            ? new Expression<Func<EventRepeatabilityEntity, object>>[] { r => r.EventTemplateEntity! }
            : Array.Empty<Expression<Func<EventRepeatabilityEntity, object>>>();

        return await dbManager.GetGuildAndIdBaseEntityAsync(guildId, eventTemplateId, asNoTracking, includes);
    }

    public async Task<List<EventRepeatabilityEntity>> GetEventRepeatabilitiesAsync(ulong guildId)
    {
        return await dbManager.GetAllGuildBaseEntitiesAsync<EventRepeatabilityEntity>(guildId, true);
    }

    public async Task<bool> TryDeleteEventRepeatabilityAsync(ulong guildId, int eventRepeatId)
    {
        var entity = await dbManager.GetGuildAndIdBaseEntityAsync<EventRepeatabilityEntity>(guildId, eventRepeatId, false);
        if (entity?.InternalId == null)
            return false;

        return await dbManager.DeleteAsync<EventRepeatabilityEntity>(entity.InternalId.Value);
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