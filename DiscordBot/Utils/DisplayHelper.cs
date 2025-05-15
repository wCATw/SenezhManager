using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using DiscordBot.Database.Entities;

namespace DiscordBot.Utils;

public static class DisplayHelper
{
    public static string GetDisplayName(PropertyInfo prop)
    {
        return prop.GetCustomAttribute<DisplayAttribute>()?.Name ?? prop.Name;
    }

    public static PropertyInfo? GetPropertyByDisplayName(Type type, string displayName)
    {
        return type.GetProperties()
            .FirstOrDefault(p =>
                p.GetCustomAttribute<DisplayAttribute>()?.Name == displayName
                || p.Name == displayName);
    }
}

public class SettingsFieldAutocompleteProvider : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
        IInteractionContext context,
        IAutocompleteInteraction interaction,
        IParameterInfo parameter,
        IServiceProvider services)
    {
        var props = typeof(SettingsEntity)
            .GetProperties()
            .Where(p => p.Name != nameof(SettingsEntity.GuildId))
            .Select(p =>
            {
                var displayName = DisplayHelper.GetDisplayName(p);
                return new AutocompleteResult(displayName, p.Name);
            })
            .ToList();

        return AutocompletionResult.FromSuccess(props);
    }
}