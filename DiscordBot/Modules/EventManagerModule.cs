using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Discord.Interactions;
using DiscordBot.Database;
using DiscordBot.Models.InteractionModels;
using DiscordBot.Services;
using DiscordBot.Services.Interfaces;

namespace DiscordBot.Modules;

[Group("событие", "Менеджер событий.")]
public class EventManagerGroup(IEventManagerService eventManager, PaginationService pagination) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("инициализация", "Создает событие через форму.")]
    public async Task Init()
    {
        await DeferAsync(true);

        var result = await eventManager.InitChannel(Context.Guild.Id);

        if (!result)
        {
            await FollowupAsync("Ошибка службы!", ephemeral: true);
            return;
        }

        await FollowupAsync("Успешно.", ephemeral: true);
    }

#region Шаблон

    [Group("шаблон", "Управление шаблонами событий.")]
    public class EventManagerTemplateGroup(IEventManagerService eventManager, PaginationService pagination)
        : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("создать", "Создает шаблон события через форму.")]
        public async Task CreateTemplate()
        {
            await RespondWithModalAsync<EventTemplateModel>("create_event_template_form");
        }

        [SlashCommand("удалить", "Удаляет шаблон события.")]
        public async Task DeleteTemplate(int event_template_id)
        {
            await DeferAsync(true);

            var result = await eventManager.TryDeleteEventTemplateAsync(Context.Guild.Id, event_template_id);

            await FollowupAsync(result ? $"Успешно удален шаблон c ID {event_template_id}!" : $"Произошла ошибка при удалении шаблона с ID {event_template_id}.", ephemeral: true);
        }

        [SlashCommand("изменить", "Изменяет шаблон события через форму.")]
        public async Task UpdateTemplate(int event_template_id)
        {
            var entity = await eventManager.GetEventTemplateAsync(Context.Guild.Id, event_template_id);

            if (entity == null)
            {
                await RespondAsync("Шаблон с таким ID не найден!", ephemeral: true);
                return;
            }

            await RespondWithModalAsync<EventTemplateModel>($"update_event_template_form:{entity.Id}", modifyModal: modal =>
            {
                modal.UpdateTextInput("title_input", input => { input.Value       = entity.Title; });
                modal.UpdateTextInput("description_input", input => { input.Value = entity.Description; });
            });
        }

        [SlashCommand("список", "Выводит списком все существующие шаблоны событий.")]
        public async Task ListTemplates()
        {
            await DeferAsync();
            var message = await GetOriginalResponseAsync();

            var templates = await eventManager.GetEventTemplatesAsync(Context.Guild.Id);
            if (templates.Count == 0)
            {
                await FollowupAsync("Шаблоны не найдены.", ephemeral: true);
                return;
            }

            var elements = templates.Select(t =>
            {
                var creationTimestamp = new DateTimeOffset(t.CreationDateTime!.Value.ToUniversalTime()).ToUnixTimeSeconds();

                return ($"ID:{t.Id} \"{t.Title}\"",
                        $"```md\n{t.Description}\n```\n*Создано: <t:{creationTimestamp}:F>*");
            }).ToList();

            var paginationTuple = pagination.CreatePagination(Context.User, "Список шаблонов", elements, message);

            if (paginationTuple.Contnet == null)
            {
                await FollowupAsync("Ошибка при сборке пагинации.", ephemeral: true);
                return;
            }

            await FollowupAsync(
                embeds: paginationTuple.Contnet.Value.Embeds,
                components: paginationTuple.Contnet.Value.Component
            );
        }

#region Обработка форм

        [ModalInteraction("create_event_template_form", true)]
        public async Task HandleCreateEventTemplateForm(EventTemplateModel modal)
        {
            await DeferAsync(true);

            var entity = new EventTemplateEntity
            {
                GuildId          = Context.Guild.Id,
                Title            = modal.TitleInput,
                Description      = modal.DescriptionInput,
                CreationDateTime = DateTime.Now
            };

            var result = await eventManager.TryAddEventTemplateAsync(entity);

            if (!result)
            {
                await FollowupAsync("Произошла ошибка при создании шаблона события!", ephemeral: true);
                return;
            }

            await FollowupAsync("Шаблон события успешно создан!", ephemeral: true);
        }

        [ModalInteraction("update_event_template_form:*", true)]
        public async Task HandleUpdateEventTemplateForm(string eventTemplateIdStr, EventTemplateModel modal)
        {
            await DeferAsync(true);

            var eventTemplateId = int.Parse(eventTemplateIdStr);

            var entity = await eventManager.GetEventTemplateAsync(Context.Guild.Id, eventTemplateId);

            if (entity == null)
            {
                await FollowupAsync("Шаблон события не найдено!", ephemeral: true);
                return;
            }

            entity = new EventTemplateEntity
            {
                GuildId     = Context.Guild.Id,
                Id          = eventTemplateId,
                Title       = modal.TitleInput,
                Description = modal.DescriptionInput
            };

            var result = await eventManager.TryUpdateEventTemplateAsync(entity);

            if (!result)
            {
                await FollowupAsync($"Произошла ошибка при изменении шаблона события с ID \"{eventTemplateId}\"",
                                    ephemeral: true);
                return;
            }

            await FollowupAsync($"Шаблон события с ID \"{eventTemplateId}\" успешно изменено!", ephemeral: true);
        }

#endregion
    }

#endregion

#region Событие

    [SlashCommand("создать", "Создает событие через форму.")]
    public async Task Create()
    {
        await RespondWithModalAsync<EventModel>("create_event_form");
    }

    [SlashCommand("удалить", "Удаляет событие.")]
    public async Task Delete(int event_id)
    {
        await DeferAsync(true);

        var result = await eventManager.TryDeleteEventAsync(Context.Guild.Id, event_id);

        await FollowupAsync(result ? $"Успешно удалено событие c ID {event_id}!" : $"Произошла ошибка при удалении события с ID {event_id}.", ephemeral: true);
    }

    [SlashCommand("изменить", "Изменяет событие через форму.")]
    public async Task Update(int event_id)
    {
        var entity = await eventManager.GetEventAsync(Context.Guild.Id, event_id);

        if (entity == null)
        {
            await RespondAsync("Событие с таким ID не найдено!", ephemeral: true);
            return;
        }

        await RespondWithModalAsync<EventModel>($"update_event_form:{entity.Id}", modifyModal: modal =>
        {
            modal.UpdateTextInput("title_input", input => { input.Value       = entity.Title; });
            modal.UpdateTextInput("description_input", input => { input.Value = entity.Description; });
            modal.UpdateTextInput("time_input", input => { input.Value        = entity.EventDateTime!.Value.ToString("HH:mm-dd.MM.yyyy"); });
        });
    }

    [SlashCommand("список", "Выводит списком все существующие события.")]
    public async Task List()
    {
        await DeferAsync();
        var message = await GetOriginalResponseAsync();

        var events = await eventManager.GetEventsAsync(Context.Guild.Id);
        if (events.Count == 0)
        {
            await FollowupAsync("События не найдены.", ephemeral: true);
            return;
        }

        var elements = events.Select(t =>
        {
            var eventTimestamp    = new DateTimeOffset(t.EventDateTime!.Value.ToUniversalTime()).ToUnixTimeSeconds();
            var creationTimestamp = new DateTimeOffset(t.CreationDateTime!.Value.ToUniversalTime()).ToUnixTimeSeconds();

            return ($"ID:{t.Id} \"{t.Title}\"",
                    $"```md\n{t.Description}\n```\n" + $"**Время проведения:** <t:{eventTimestamp}:F>\n" + $"*Создано: <t:{creationTimestamp}:F>*");
        }).ToList();

        var paginationTuple = pagination.CreatePagination(Context.User, "Список событий", elements, message);

        if (paginationTuple.Contnet == null)
        {
            await FollowupAsync("Ошибка при сборке пагинации.", ephemeral: true);
            return;
        }

        await FollowupAsync(
            embeds: paginationTuple.Contnet.Value.Embeds,
            components: paginationTuple.Contnet.Value.Component
        );
    }

#region Обработка форм

    [ModalInteraction("create_event_form", true)]
    public async Task HandleCreateEventForm(EventModel modal)
    {
        await DeferAsync(true);

        if (!DateTime.TryParseExact(modal.TimeInput, "HH:mm-dd.MM.yyyy", null, DateTimeStyles.None, out var time))
        {
            await FollowupAsync("Неправильный формат времени!", ephemeral: true);
            return;
        }

        if (time < DateTime.Now)
        {
            await FollowupAsync("Время указано в прошлом!", ephemeral: true);
            return;
        }

        var entity = new EventEntity
        {
            GuildId          = Context.Guild.Id,
            Title            = modal.TitleInput,
            Description      = modal.DescriptionInput,
            CreationDateTime = DateTime.Now,
            EventDateTime    = time
        };

        var result = await eventManager.TryAddEventAsync(entity);

        if (!result)
        {
            await FollowupAsync("Произошла ошибка при создании события!", ephemeral: true);
            return;
        }

        await FollowupAsync("Событие успешно создано!", ephemeral: true);
    }

    [ModalInteraction("update_event_form:*", true)]
    public async Task HandleUpdateEventForm(string eventIdStr, EventModel modal)
    {
        await DeferAsync(true);

        var eventId = int.Parse(eventIdStr);

        var entity = await eventManager.GetEventAsync(Context.Guild.Id, eventId);

        if (entity == null)
        {
            await FollowupAsync("Событие не найдено!", ephemeral: true);
            return;
        }

        var time = entity.EventDateTime!.Value;

        if (DateTime.TryParseExact(modal.TimeInput, "HH:mm-dd.MM.yyyy", null, DateTimeStyles.None, out var parsedTime))
        {
            if (parsedTime < DateTime.Now)
                await FollowupAsync("Время указано в прошлом и не будет изменено!", ephemeral: true);
            else
                time = parsedTime;
        }

        entity = new EventEntity
        {
            GuildId       = Context.Guild.Id,
            Id            = eventId,
            Title         = modal.TitleInput,
            Description   = modal.DescriptionInput,
            EventDateTime = time
        };

        var result = await eventManager.TryUpdateEventAsync(entity);

        if (!result)
        {
            await FollowupAsync($"Произошла ошибка при изменении события с ID \"{eventId}\"", ephemeral: true);
            return;
        }

        await FollowupAsync($"Событие с ID \"{eventId}\" успешно изменено!", ephemeral: true);
    }

#endregion

#endregion
}