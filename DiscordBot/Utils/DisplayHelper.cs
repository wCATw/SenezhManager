using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;

namespace DiscordBot.Utils;

public static class DisplayHelper
{
    public static string GetDisplayName(MemberInfo member)
    {
        if (member.IsSystemField())
            return member.Name;

        return member.GetCustomAttribute<DisplayAttribute>()?.Name ?? member.Name;
    }

    public static IEnumerable<MemberInfo> GetUserVisibleMembers(this Type type)
    {
        return type.GetMembers(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m is PropertyInfo or FieldInfo)
            .Where(m => !m.IsSystemField());
    }

    public static PropertyInfo? GetPropertyByDisplayName(Type type, string displayName)
    {
        return type.GetProperties()
            .FirstOrDefault(p =>
                (p.GetCustomAttribute<DisplayAttribute>()?.Name == displayName
                 || p.Name == displayName)
                && !p.IsSystemField());
    }
}

public class EntityFieldAutocompleteProvider<T> : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
        IInteractionContext context,
        IAutocompleteInteraction interaction,
        IParameterInfo parameter,
        IServiceProvider services)
    {
        var props = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<DisplayAttribute>()?.Name != "SYSTEM")
            .Select(p => new AutocompleteResult(DisplayHelper.GetDisplayName(p), p.Name))
            .ToList();

        return AutocompletionResult.FromSuccess(props);
    }
}