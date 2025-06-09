using System;
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
            var checkResult = await eventManager.GetEventTemplateAsync(Context.Guild.Id, event_template_id);

            if (checkResult == null)
            {
                await RespondAsync("Шаблон с таким ID не найден!", ephemeral: true);
                return;
            }

            await RespondWithModalAsync<EventTemplateModel>($"update_event_template_form:{event_template_id}");
        }

        [SlashCommand("список", "Выводит списком все существующие шаблоны событий.")]
        public async Task ListTemplates()
        {
            await DeferAsync(true);
            var message = await GetOriginalResponseAsync();

            var templates = await eventManager.GetEventTemplatesAsync(Context.Guild.Id);
            if (templates.Count == 0)
            {
                await FollowupAsync("Шаблоны не найдены.", ephemeral: true);
                return;
            }

            var elements = templates.Select(t => ($"ID:{t.Id}, {t.Title}", $"{t.Description}")).ToList();

            var paginationTuple = pagination.CreatePagination(
                                                              Context.User.Id,
                                                              "Список шаблонов",
                                                              elements,
                                                              message
                                                             );

            await FollowupAsync(
                                embed: paginationTuple.Contnet?.Embed.Build(),
                                components: paginationTuple.Contnet?.Component.Build()
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

        [ModalInteraction("update_event_template_form:*", true, TreatAsRegex = true)]
        public async Task HandleUpdateEventTemplateForm(EventTemplateModel modal)
        {
            await DeferAsync(true);

            var interactionData = Context.Interaction.Data;
            var prop            = interactionData.GetType().GetProperty("CustomId");
            var customData      = prop!.GetValue(interactionData) as string;
            var args            = customData!.Split(":");
            var eventTemplateId = int.Parse(args[1]);

            var entity = await eventManager.GetEventTemplateAsync(Context.Guild.Id, eventTemplateId);

            entity = new EventTemplateEntity
            {
                GuildId     = Context.Guild.Id,
                Id          = eventTemplateId,
                Title       = modal.TitleInput == "-" ? entity!.Title : modal.TitleInput,
                Description = modal.DescriptionInput == "-" ? entity!.Description : modal.DescriptionInput
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
}