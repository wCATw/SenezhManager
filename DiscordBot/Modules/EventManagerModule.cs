using System;
using System.Threading.Tasks;
using Discord.Interactions;
using DiscordBot.Models.InteractionModels;
using DiscordBot.Services.Scoped.Interfaces;

namespace DiscordBot.Modules;

[Group("событие", "Менеджер событий.")]
public class EventManagerGroup(IEventManagerService eventManager) : InteractionModuleBase<SocketInteractionContext>
{
    #region Шаблон

    [Group("шаблон", "Управление шаблонами событий.")]
    public class EventManagerTemplateGroup(IEventManagerService eventManager)
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

            await FollowupAsync(result ? "Успешно!" : "Произошла ошибка службы.", ephemeral: true);
        }

        [SlashCommand("изменить", "Изменяет шаблон события через форму.")]
        public async Task UpdateTemplate(int event_id)
        {
            await RespondWithModalAsync<EventTemplateModel>($"update_event_template_form:{event_id}");
        }

        #region Повторяемость

        [Group("повторяемость", "Управления шаблонами событий.")]
        public class EventManagerTemplateRepeatabilityGroup(IEventManagerService eventManager)
            : InteractionModuleBase<SocketInteractionContext>
        {
            [SlashCommand("назначить", "Назначает повторяемость шаблонному событию.")]
            public async Task SetRepeatability()
            {
                throw new NotImplementedException();
            }

            [SlashCommand("убрать", "Убирает повторяемость шаблонному событию.")]
            public async Task RemoveRepeatability()
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Обработка форм

        [ModalInteraction("create_event_template_form")]
        public async Task HandleCreateEventTemplate(EventTemplateModel modal)
        {
            await DeferAsync(true);
            await FollowupAsync("METHOD_NOT_IMPLEMENTED", ephemeral: true);
        }

        [ModalInteraction("update_event_template_form")]
        public async Task HandleUpdateEventTemplate(EventTemplateModel modal, int eventId)
        {
            await DeferAsync(true);
            await FollowupAsync("METHOD_NOT_IMPLEMENTED", ephemeral: true);
        }

        #endregion
    }

    #endregion
}