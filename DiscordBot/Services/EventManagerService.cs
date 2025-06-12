using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Database;
using DiscordBot.Services.Interfaces;
using DiscordBot.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Services;

public class EventManagerService(IDbManagerService dbManager, ISettingsManagerService settingsManager, ILogger<EventManagerService> logger, DiscordSocketClient client)
    : IEventManagerService
{
    private static bool _routineInProgress;
    private bool _disposed;

#region Реализации интерфейса.

    public async Task<bool> TryAddEventTemplateAsync(EventTemplateEntity eventTemplateEnt)
    {
        if (eventTemplateEnt.GuildId == null)
            return false;

        eventTemplateEnt.Id = await ServicesHelper.GenerateNextIdForGuild<EventTemplateEntity>(dbManager.DbContext, eventTemplateEnt.GuildId.Value);

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

        eventEnt.Notified = false;
        eventEnt.Id       = await ServicesHelper.GenerateNextIdForGuild<EventEntity>(dbManager.DbContext, eventEnt.GuildId.Value);

        var result = await dbManager.AddAsync(eventEnt);

        if (result != null)
        {
            await RoutineCheck();
            return true;
        }

        return false;
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

        if (result != null)
        {
            await RoutineCheck();
            return true;
        }

        return false;
    }

    public async Task<EventEntity?> GetEventAsync(ulong guildId, int eventId, bool asNoTracking = true)
    {
        return await dbManager.GetGuildAndIdBaseEntityAsync<EventEntity>(guildId, eventId, asNoTracking);
    }

    public async Task<List<EventEntity>> GetEventsAsync(ulong guildId)
    {
        return (await dbManager.GetAllGuildBaseEntitiesAsync<EventEntity>(guildId)).OrderBy(e => e.EventDateTime).ToList();
    }

    public async Task<bool> TryDeleteEventAsync(ulong guildId, int eventId)
    {
        var entity = await GetEventAsync(guildId, eventId, false);

        if (entity?.InternalId == null)
            return false;

        var result = await dbManager.DeleteAsync<EventEntity>(entity.InternalId.Value);

        if (result)
            await RoutineCheck();

        return result;
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

    public async Task RoutineCheck(SettingsEntity? targetSettings = null)
    {
        if (_routineInProgress)
        {
            logger.LogWarning("RoutineCheck is already running!");
            return;
        }

        _routineInProgress = true;

        if (targetSettings == null)
        {
            var settingsList = dbManager.DbContext.SettingsEntities.AsNoTracking().Where(s => s.ScheduleMessageId != null).ToList();
            foreach (var settings in settingsList)
            {
                var eventEntities = await GetEventsAsync(settings.GuildId!.Value);
                await UpdateSchedule(eventEntities, settings);
                await ClearExpiredEvents(eventEntities, settings);
                await SendEditMessages(eventEntities, settings);
            }
        }
        else
        {
            var eventEntities = await GetEventsAsync(targetSettings.GuildId!.Value);
            await UpdateSchedule(eventEntities, targetSettings);
            await ClearExpiredEvents(eventEntities, targetSettings);
            await SendEditMessages(eventEntities, targetSettings);
        }

        _routineInProgress = false;
    }

    public async Task<bool> InitChannel(ulong guildId)
    {
        try
        {
            var settings = await settingsManager.GetSettingsAsync(guildId);
            await InitSchedule(settings);
            await RoutineCheck(settings);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return false;
        }
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

#endregion

#region Внутренние методы.

    /// <summary>
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    private Embed BuildScheduleMessage(List<EventEntity> entities)
    {
        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithTitle("Расписание событий на следующие 10 дней.");
        var targetEntities = entities.Where(entity => entity.EventDateTime!.Value > DateTime.Now && entity.EventDateTime!.Value < DateTime.Now.Add(TimeSpan.FromDays(10))).ToList();

        foreach (var entity in targetEntities)
            embedBuilder.AddField(entity.Title, $"Дата и время проведения события: {DisplayHelper.DateTimeToDiscordTimeStamp(entity.EventDateTime!.Value, "F")}");

        return embedBuilder.Build();
    }

    /// <summary>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    private Embed BuildEventMessage(EventEntity entity)
    {
        var embedBuilder = new EmbedBuilder();

        embedBuilder.WithTitle(entity.Title!);
        embedBuilder.WithDescription(entity.Description! + $"\n\n||Создано: {DisplayHelper.DateTimeToDiscordTimeStamp(entity.CreationDateTime!.Value, "F")}||");

        return embedBuilder.Build();
    }

    /// <summary>
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private ITextChannel GetNotificationChannel(SettingsEntity settings)
    {
        var guild   = client.Guilds.First(g => g.Id == settings.GuildId);
        var channel = guild.Channels.First(c => c.Id == settings.EventNotificationChannelId);
        if (channel is not ITextChannel textChannel)
            throw new ArgumentException("Event notification channel is not ITextChannel!");

        return textChannel;
    }

    /// <summary>
    /// </summary>
    /// <param name="settings"></param>
    /// <exception cref="ArgumentException"></exception>
    private async Task InitSchedule(SettingsEntity settings)
    {
        var channel = GetNotificationChannel(settings);

        try
        {
            if (settings.ScheduleMessageId == null)
                throw new ArgumentException("ScheduleMessageId is not specified!");
            var oldMessage = await channel.GetMessageAsync(settings.ScheduleMessageId.Value);
            await oldMessage.DeleteAsync();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex.ToString());
        }

        await foreach (var messages in channel.GetMessagesAsync())
        {
            foreach (var message in messages)
                try
                {
                    await message.DeleteAsync();
                }
                catch (Exception ex)
                {
#if DEBUG
                    logger.LogWarning(ex.ToString());
#endif
                }
        }

        var scheduleMessage = await channel.SendMessageAsync("@everyone", allowedMentions: AllowedMentions.All);
        settings.ScheduleMessageId = scheduleMessage.Id;
        await settingsManager.TryUpdateSettingsAsync(settings);
    }

    /// <summary>
    /// </summary>
    /// <param name="eventList"></param>
    /// <param name="settings"></param>
    /// <exception cref="ArgumentException"></exception>
    private async Task UpdateSchedule(List<EventEntity> eventList, SettingsEntity settings)
    {
        var channel = GetNotificationChannel(settings);
        if (settings.ScheduleMessageId == null)
            throw new ArgumentException("ScheduleMessageId is not specified!");

        var message = await channel.GetMessageAsync(settings.ScheduleMessageId.Value);
        if (message is not IUserMessage userMessage)
            throw new ArgumentException("Schedule message is not IUserMessage!");

        await userMessage.ModifyAsync(msg => msg.Embed = BuildScheduleMessage(eventList));
    }

    /// <summary>
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="settings"></param>
    private async Task DeleteEventsMessages(List<EventEntity> entities, SettingsEntity settings)
    {
        var messagesToDelete = new List<IUserMessage>();
        var channel          = GetNotificationChannel(settings);
        await foreach (var messages in channel.GetMessagesAsync())
        {
            if (messages is null)
                continue;

            foreach (var message in messages)
            {
                if (message is not IUserMessage userMessage)
                    continue;

                if (entities.Any(e => e.MessageId == message.Id))
                    messagesToDelete.Add(userMessage);
            }
        }

        await channel.DeleteMessagesAsync(messagesToDelete);
    }

    /// <summary>
    /// </summary>
    /// <param name="eventList"></param>
    /// <param name="settings"></param>
    private async Task ClearExpiredEvents(List<EventEntity> eventList, SettingsEntity settings)
    {
        var expiredEntities = eventList.Where(entity => entity.EventDateTime!.Value < DateTime.Now).ToList();

        foreach (var entity in expiredEntities)
        {
            var removingResult = await TryDeleteEventAsync(entity.GuildId!.Value, entity.Id!.Value);
            if (!removingResult)
                logger.LogWarning("An error occurred when trying to delete a record from the database, while cleaning outdated events.");
        }

        await DeleteEventsMessages(expiredEntities, settings);
    }

    /// <summary>
    /// </summary>
    /// <param name="eventList"></param>
    /// <param name="settings"></param>
    private async Task SendEditMessages(List<EventEntity> eventList, SettingsEntity settings)
    {
        var channel        = GetNotificationChannel(settings);
        var targetEntities = eventList.Where(entity => entity.EventDateTime!.Value > DateTime.Now && entity.EventDateTime!.Value < DateTime.Now.Add(TimeSpan.FromDays(3))).ToList();
        foreach (var entity in targetEntities)
            if (entity.MessageId == null)
            {
                var message = await channel.SendMessageAsync("@everyone", embed: BuildEventMessage(entity), allowedMentions: AllowedMentions.All);
                entity.MessageId = message.Id;
                await TryUpdateEventAsync(entity);
            }
            else
            {
                var message = await channel.GetMessageAsync(entity.MessageId.Value);
                if (message is not IUserMessage userMessage)
                {
                    logger.LogWarning($"Can't edit event message. Message is not IUserMessage! Internal ID: {entity.InternalId}");
                    continue;
                }

                await userMessage.ModifyAsync(msg => msg.Embed = BuildEventMessage(entity));
            }
    }

#endregion
}