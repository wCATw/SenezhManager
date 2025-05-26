using System;
using System.Threading.Tasks;
using DiscordBot.Database.Entities;
using DiscordBot.Services.Scoped.Interfaces;

namespace DiscordBot.Services.Scoped;

public class EventManagerService : IEventManagerService
{
    public Task<bool> TryAddUpdateEventTemplateAsync(EventTemplateEntity eventTemplateEnt)
    {
        throw new NotImplementedException();
    }

    public Task<EventTemplateEntity> GetEventTemplateAsync(ulong guildId, int eventTemplateId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> TryDeleteEventTemplateAsync(ulong guildId, int eventTemplateId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> TryAddUpdateEventAsync(EventEntity eventEnt)
    {
        throw new NotImplementedException();
    }

    public Task<EventTemplateEntity> GetEventAsync(ulong guildId, int eventId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> TryDeleteEventAsync(ulong guildId, int eventId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> TryAddUpdateEventRepeatabilityAsync(EventRepeatabilityEntity eventRepeatEnt)
    {
        throw new NotImplementedException();
    }

    public Task<EventRepeatabilityEntity> GetEventRepeatabilityAsync(ulong guildId, int eventRepeatId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> TryDeleteEventRepeatabilityAsync(ulong guildId, int eventRepeatId)
    {
        throw new NotImplementedException();
    }
}