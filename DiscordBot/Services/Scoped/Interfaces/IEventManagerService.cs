using System.Threading.Tasks;
using DiscordBot.Database.Entities;

namespace DiscordBot.Services.Scoped.Interfaces;

public interface IEventManagerService
{
    Task<bool> TryAddUpdateEventTemplateAsync(EventTemplateEntity eventTemplateEnt);
    Task<EventTemplateEntity> GetEventTemplateAsync(ulong guildId, int eventTemplateId);
    Task<bool> TryDeleteEventTemplateAsync(ulong guildId, int eventTemplateId);

    Task<bool> TryAddUpdateEventAsync(EventEntity eventEnt);
    Task<EventTemplateEntity> GetEventAsync(ulong guildId, int eventId);
    Task<bool> TryDeleteEventAsync(ulong guildId, int eventId);

    Task<bool> TryAddUpdateEventRepeatabilityAsync(EventRepeatabilityEntity eventRepeatEnt);
    Task<EventRepeatabilityEntity> GetEventRepeatabilityAsync(ulong guildId, int eventRepeatId);
    Task<bool> TryDeleteEventRepeatabilityAsync(ulong guildId, int eventRepeatId);
}