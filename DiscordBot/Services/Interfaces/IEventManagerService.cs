using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBot.Database;

namespace DiscordBot.Services.Interfaces;

/// <summary>
/// 
/// </summary>
public interface IEventManagerService : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventTemplateEnt"></param>
    /// <returns></returns>
    Task<bool> TryAddEventTemplateAsync(EventTemplateEntity eventTemplateEnt);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventTemplateEnt"></param>
    /// <returns></returns>
    Task<bool> TryUpdateEventTemplateAsync(EventTemplateEntity eventTemplateEnt);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="eventTemplateId"></param>
    /// <param name="includeRepeatability"></param>
    /// <param name="asNoTracking"></param>
    /// <returns></returns>
    Task<EventTemplateEntity?> GetEventTemplateAsync(ulong guildId, int eventTemplateId, bool includeRepeatability = false, bool asNoTracking = true);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <returns></returns>
    Task<List<EventTemplateEntity>> GetEventTemplatesAsync(ulong guildId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="eventTemplateId"></param>
    /// <returns></returns>
    Task<bool> TryDeleteEventTemplateAsync(ulong guildId, int eventTemplateId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventEnt"></param>
    /// <returns></returns>
    Task<bool> TryAddEventAsync(EventEntity eventEnt);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventEnt"></param>
    /// <returns></returns>
    Task<bool> TryUpdateEventAsync(EventEntity eventEnt);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="eventId"></param>
    /// <param name="asNoTracking"></param>
    /// <returns></returns>
    Task<EventEntity?> GetEventAsync(ulong guildId, int eventId, bool asNoTracking = true);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <returns></returns>
    Task<List<EventEntity>> GetEventsAsync(ulong guildId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="eventId"></param>
    /// <returns></returns>
    Task<bool> TryDeleteEventAsync(ulong guildId, int eventId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventRepeatEnt"></param>
    /// <returns></returns>
    Task<bool> TryAddEventRepeatabilityAsync(EventRepeatabilityEntity eventRepeatEnt);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventRepeatEnt"></param>
    /// <returns></returns>
    Task<bool> TryUpdateEventRepeatabilityAsync(EventRepeatabilityEntity eventRepeatEnt);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="eventTemplateId"></param>
    /// <param name="includeTemplate"></param>
    /// <param name="asNoTracking"></param>
    /// <returns></returns>
    Task<EventRepeatabilityEntity?> GetEventRepeatabilityAsync(ulong guildId, int eventTemplateId, bool includeTemplate = false, bool asNoTracking = true);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <returns></returns>
    Task<List<EventRepeatabilityEntity>> GetEventRepeatabilitiesAsync(ulong guildId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="eventRepeatId"></param>
    /// <returns></returns>
    Task<bool> TryDeleteEventRepeatabilityAsync(ulong guildId, int eventRepeatId);
}