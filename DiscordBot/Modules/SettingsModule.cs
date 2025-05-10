using System;
using System.Threading.Tasks;
using Discord.Interactions;

namespace DiscordBot.Modules;

[Group("настройки", "Модуль управления настройками.")]
public class SettingsModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("показать", "Показывает все настройки бота.")]
    public async Task ShowSettings()
    {
        throw new NotImplementedException();
    }

    [SlashCommand("изменить", "Изменяет настройки бота.")]
    public async Task ChangeSettings()
    {
        throw new NotImplementedException();
    }
}