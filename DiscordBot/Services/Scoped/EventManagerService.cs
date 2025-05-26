using System;
using System.Reflection;
using System.Threading.Tasks;
using DiscordBot.Database.Entities;
using DiscordBot.Services.Scoped.Interfaces;

namespace DiscordBot.Services.Scoped;

public class EventManagerService(IDbManagerService dbManager) : IEventManagerService
{
    #region EventTemplate

    public async Task<bool> TryAddEventTemplateAsync(EventTemplateEntity eventTemplateEnt)
    {
        return await AddEntityAsync(eventTemplateEnt);
    }

    public async Task<bool> TryUpdateEventTemplateAsync(EventTemplateEntity eventTemplateEnt)
    {
        return await UpdateEntityAsync(eventTemplateEnt, (existing, incoming) =>
        {
            PatchEntity(existing, incoming);
            if (incoming.RepeatabilityEntity != null)
            {
                existing.RepeatabilityEntity ??= new EventRepeatabilityEntity
                    { GuildId = existing.GuildId, Id = incoming.RepeatabilityEntity.Id };
                PatchEntity(existing.RepeatabilityEntity, incoming.RepeatabilityEntity);
            }
        });
    }

    public async Task<EventTemplateEntity?> GetEventTemplateAsync(ulong guildId, int eventTemplateId,
        bool asNoTracking = true)
    {
        return await dbManager.GetGuildAndIdBaseAsync<EventTemplateEntity>(guildId, eventTemplateId, asNoTracking);
    }

    public async Task<bool> TryDeleteEventTemplateAsync(ulong guildId, int eventTemplateId)
    {
        return await DeleteEntityAsync<EventTemplateEntity>(guildId, eventTemplateId);
    }

    #endregion

    #region Event

    public async Task<bool> TryAddEventAsync(EventEntity eventEnt)
    {
        return await AddEntityAsync(eventEnt);
    }

    public async Task<bool> TryUpdateEventAsync(EventEntity eventEnt)
    {
        return await UpdateEntityAsync(eventEnt, PatchEntity);
    }

    public async Task<EventEntity?> GetEventAsync(ulong guildId, int eventId, bool asNoTracking = true)
    {
        return await dbManager.GetGuildAndIdBaseAsync<EventEntity>(guildId, eventId, asNoTracking);
    }

    public async Task<bool> TryDeleteEventAsync(ulong guildId, int eventId)
    {
        return await DeleteEntityAsync<EventEntity>(guildId, eventId);
    }

    #endregion

    #region EventRepeatability

    public async Task<bool> TryAddEventRepeatabilityAsync(EventRepeatabilityEntity eventRepeatEnt)
    {
        return await AddEntityAsync(eventRepeatEnt);
    }

    public async Task<bool> TryUpdateEventRepeatabilityAsync(EventRepeatabilityEntity eventRepeatEnt)
    {
        return await UpdateEntityAsync(eventRepeatEnt, PatchEntity);
    }

    public async Task<EventRepeatabilityEntity?> GetEventRepeatabilityAsync(ulong guildId, int eventRepeatId,
        bool asNoTracking = true)
    {
        return await dbManager.GetGuildAndIdBaseAsync<EventRepeatabilityEntity>(guildId, eventRepeatId, asNoTracking);
    }

    public async Task<bool> TryDeleteEventRepeatabilityAsync(ulong guildId, int eventRepeatId)
    {
        return await DeleteEntityAsync<EventRepeatabilityEntity>(guildId, eventRepeatId);
    }

    #endregion

    #region Helpers

    private async Task<bool> AddEntityAsync<T>(T entity) where T : GuildAndIdBaseEntity
    {
        var result = await dbManager.AddAsync(entity);
        return result != null;
    }

    private async Task<bool> UpdateEntityAsync<T>(T incoming, Action<T, T> patchAction) where T : GuildAndIdBaseEntity
    {
        if (incoming.GuildId is null || incoming.Id is null)
            return false;

        var existing = await dbManager.GetGuildAndIdBaseAsync<T>(incoming.GuildId.Value, incoming.Id.Value, false);
        if (existing == null)
            return false;

        patchAction(existing, incoming);

        var result = await dbManager.UpdateAsync(existing);
        return result != null;
    }

    private async Task<bool> DeleteEntityAsync<T>(ulong guildId, int id)
        where T : GuildAndIdBaseEntity
    {
        var entity = await dbManager.GetGuildAndIdBaseAsync<T>(guildId, id, false);
        if (entity == null)
            return false;

        var result = await dbManager.DeleteAsync(entity);
        return result != null;
    }

    private static void PatchEntity<T>(T target, T incoming)
    {
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in props)
        {
            if (!prop.CanWrite || prop.Name is "GuildId" or "Id" or "UserId") continue;

            var newValue = prop.GetValue(incoming);

            if (prop.PropertyType == typeof(string) && (string?)newValue == "-")
                continue;

            if (newValue != null)
                prop.SetValue(target, newValue);
        }
    }

    #endregion
}