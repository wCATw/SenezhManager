using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using DiscordBot.Database;
using DiscordBot.Services.Interfaces;

namespace DiscordBot.Modules;

[Group("настройки", "Модуль управления настройками.")]
public class SettingsModule(ISettingsManagerService settingsManager) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("показать", "Показывает все настройки бота.")]
    public async Task ShowSettings()
    {
        await DeferAsync();

        var settings = await settingsManager.GetSettingsAsync(Context.Guild.Id);

        if (settings == null)
        {
            await FollowupAsync("Настройки для этого сервера не найдены.");
            return;
        }

        var embedBuilder = new EmbedBuilder()
            .WithTitle("Текущие настройки")
            .WithColor(Color.Blue);

        var properties = typeof(SettingsEntity)
            .GetProperties()
            .Where(p => p.Name != nameof(SettingsEntity.GuildId));

        foreach (var prop in properties)
        {
            var value = prop.GetValue(settings);
            var displayValue = value != null ? value.ToString() : "*не установлено*";

            embedBuilder.AddField(prop.Name, displayValue, true);
        }

        await FollowupAsync(embed: embedBuilder.Build());
    }

    [SlashCommand("изменить", "Изменяет настройки бота.")]
    [RequireUserPermission(GuildPermission.ManageGuild)]
    public async Task ChangeSettings([Autocomplete(typeof(SettingsFieldAutocompleteProvider))] string option,
        string value)
    {
        await DeferAsync();

        var entity = new SettingsEntity { GuildId = Context.Guild.Id };

        var prop = typeof(SettingsEntity).GetProperty(option);
        if (prop == null || prop.Name == nameof(SettingsEntity.GuildId))
        {
            await FollowupAsync($"Поле `{option}` не найдено или его нельзя изменять.");
            return;
        }

        try
        {
            var convertedValue =
                Convert.ChangeType(value, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            prop.SetValue(entity, convertedValue);
        }
        catch
        {
            await FollowupAsync($"Не удалось преобразовать значение `{value}` для поля `{option}`.");
            return;
        }

        var result = await settingsManager.TryUpdateSettingsAsync(entity);

        if (result)
            await FollowupAsync($"Настройка `{option}` успешно обновлена.");
        else
            await FollowupAsync($"Не удалось обновить настройку `{option}`.");
    }
}

public class SettingsFieldAutocompleteProvider : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,
        IAutocompleteInteraction interaction, IParameterInfo parameter, IServiceProvider services)
    {
        var props = typeof(SettingsEntity)
            .GetProperties()
            .Where(p => p.Name != nameof(SettingsEntity.GuildId))
            .Select(p => new AutocompleteResult(p.Name, p.Name))
            .ToList();

        return AutocompletionResult.FromSuccess(props);
    }
}