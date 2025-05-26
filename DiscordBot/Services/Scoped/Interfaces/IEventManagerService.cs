using System.Threading.Tasks;
using DiscordBot.Database.Entities;

namespace DiscordBot.Services.Scoped.Interfaces;

public interface IEventManagerService
{
    Task<bool> TryAddEventTemplateAsync(EventTemplateEntity eventTemplateEnt);
    Task<bool> TryUpdateEventTemplateAsync(EventTemplateEntity eventTemplateEnt);

    Task<EventTemplateEntity?> GetEventTemplateAsync(ulong guildId, int eventTemplateId, bool asNoTracking = true);
    Task<bool> TryDeleteEventTemplateAsync(ulong guildId, int eventTemplateId);

    Task<bool> TryAddEventAsync(EventEntity eventEnt);
    Task<bool> TryUpdateEventAsync(EventEntity eventEnt);

    Task<EventEntity?> GetEventAsync(ulong guildId, int eventId, bool asNoTracking = true);
    Task<bool> TryDeleteEventAsync(ulong guildId, int eventId);

    Task<bool> TryAddEventRepeatabilityAsync(EventRepeatabilityEntity eventRepeatEnt);
    Task<bool> TryUpdateEventRepeatabilityAsync(EventRepeatabilityEntity eventRepeatEnt);

    Task<EventRepeatabilityEntity?> GetEventRepeatabilityAsync(ulong guildId, int eventRepeatId,
        bool asNoTracking = true);

    Task<bool> TryDeleteEventRepeatabilityAsync(ulong guildId, int eventRepeatId);
}