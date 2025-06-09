using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using DiscordBot.Database;
using DiscordBot.Services.Interfaces;
using DiscordBot.Utils;

namespace DiscordBot.Modules;

[Group("настройки", "Управление настройками.")]
public class SettingsGroup(ISettingsManagerService settingsManager) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("показать", "Показывает все настройки бота.")]
    public async Task ShowSettings()
    {
        await DeferAsync(true);

        var settings = await settingsManager.GetSettingsAsync(Context.Guild.Id) ?? new SettingsEntity();

        var embedBuilder = new EmbedBuilder()
                           .WithTitle("Текущие настройки")
                           .WithColor(Color.Blue);

        foreach (var member in typeof(SettingsEntity).GetUserVisibleMembers())
        {
            var value = member switch
            {
                PropertyInfo prop => prop.GetValue(settings),
                FieldInfo field   => field.GetValue(settings),
                _                 => null
            };

            var displayName  = DisplayHelper.GetDisplayName(member);
            var displayValue = value != null ? value.ToString() : "*не установлено*";

            embedBuilder.AddField(displayName, displayValue, true);
        }

        await FollowupAsync(embed: embedBuilder.Build(), ephemeral: true);
    }

    [SlashCommand("изменить", "Изменяет настройки бота.")]
    [RequireUserPermission(GuildPermission.ManageGuild)]
    public async Task ChangeSettings(
        [Autocomplete(typeof(EntityFieldAutocompleteProvider<SettingsEntity>))]
        string option,
        string value)
    {
        await DeferAsync(true);

        var entity = new SettingsEntity { GuildId = Context.Guild.Id };

        var prop = typeof(SettingsEntity).GetProperty(option);
        if (prop == null || prop.Name == nameof(SettingsEntity.GuildId))
        {
            await FollowupAsync($"Поле `{option}` не найдено или его нельзя изменять.", ephemeral: true);
            return;
        }

        var displayName = DisplayHelper.GetDisplayName(prop);

        try
        {
            var convertedValue =
                Convert.ChangeType(value, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            prop.SetValue(entity, convertedValue);
        }
        catch
        {
            await FollowupAsync($"Не удалось преобразовать значение `{value}` для поля `{displayName}`.",
                                ephemeral: true);
            return;
        }

        var result = await settingsManager.TryUpdateSettingsAsync(entity);

        if (result)
            await FollowupAsync($"Настройка `{displayName}` успешно обновлена.", ephemeral: true);
        else
            await FollowupAsync($"Не удалось обновить настройку `{option}`.", ephemeral: true);
    }
}