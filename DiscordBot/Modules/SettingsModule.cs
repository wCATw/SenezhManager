using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using DiscordBot.Database.Entities;
using DiscordBot.Services.Interfaces;

namespace DiscordBot.Modules;

[Group("настройки", "Модуль управления настройками.")]
public class SettingsModule(ISettingsManagerService settingsManager) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("показать", "Показывает все настройки бота.")]
    public async Task ShowSettings()
    {
        var settings = await settingsManager.GetSettingsAsync(Context.Guild.Id);

        if (settings == null) await RespondAsync("Произошла ошибка!", ephemeral: true);

        var resultBuilder = new StringBuilder();

        resultBuilder.AppendLine("## Настройки регистрации");
        resultBuilder.AppendLine($"Канал регистрации: <#{settings?.RegistrationChannelId}>");

        await RespondAsync(resultBuilder.ToString(), ephemeral: true);
    }


    [SlashCommand("изменить", "Изменяет настройки бота.")]
    [RequireUserPermission(GuildPermission.ManageGuild)]
    public async Task ChangeSettings(IChannel? registration_channel = null)
    {
        var newEnt = new SettingsEntity
        {
            RegistrationChannelId = registration_channel?.Id
        };

        if (!await settingsManager.TryUpdateSettingsAsync(Context.Guild.Id, newEnt))
            await RespondAsync("Произошла ошибка!", ephemeral: true);

        await RespondAsync("Настройки обновлены!", ephemeral: true);
    }
}