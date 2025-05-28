using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBot.Database;

namespace DiscordBot.Services.Scoped.Interfaces;

public interface IEventManagerService : IDisposable, IAsyncDisposable
{
    Task<bool> TryAddEventTemplateAsync(EventTemplateEntity eventTemplateEnt);
    Task<bool> TryUpdateEventTemplateAsync(EventTemplateEntity eventTemplateEnt);

    Task<EventTemplateEntity?> GetEventTemplateAsync(ulong guildId, int eventTemplateId, bool includeRepeatability = false, bool asNoTracking = true);
    Task<List<EventTemplateEntity>> GetEventTemplatesAsync(ulong guildId);
    Task<bool> TryDeleteEventTemplateAsync(ulong guildId, int eventTemplateId);

    Task<bool> TryAddEventAsync(EventEntity eventEnt);
    Task<bool> TryUpdateEventAsync(EventEntity eventEnt);

    Task<EventEntity?> GetEventAsync(ulong guildId, int eventId, bool asNoTracking = true);
    Task<List<EventTemplateEntity>> GetEventsAsync(ulong guildId);
    Task<bool> TryDeleteEventAsync(ulong guildId, int eventId);

    Task<bool> TryAddEventRepeatabilityAsync(EventRepeatabilityEntity eventRepeatEnt);
    Task<bool> TryUpdateEventRepeatabilityAsync(EventRepeatabilityEntity eventRepeatEnt);

    Task<EventRepeatabilityEntity?> GetEventRepeatabilityAsync(ulong guildId, int eventTemplateId, bool includeTemplate = false, bool asNoTracking = true);
    Task<List<EventTemplateEntity>> GetEventRepeatabilitiesAsync(ulong guildId);

    Task<bool> TryDeleteEventRepeatabilityAsync(ulong guildId, int eventRepeatId);
}