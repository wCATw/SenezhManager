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
    public static string DateTimeToDiscordTimeStamp(DateTime dateTime, string mode = "")
    {
        switch (mode)
        {
            case "d":
            case "D":
            case "t":
            case "T":
            case "f":
            case "F":
            case "R":
            case "":
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, "Incorrect mode!");
        }

        return $"<t:{new DateTimeOffset(dateTime).ToUnixTimeSeconds()}:{mode}>";
    }

    public static string GetDisplayName(MemberInfo member)
    {
        if (member.IsHidden())
            return member.Name;

        return member.GetCustomAttribute<DisplayAttribute>()?.Name ?? member.Name;
    }

    public static IEnumerable<MemberInfo> GetUserVisibleMembers(this Type type)
    {
        return type.GetMembers(BindingFlags.Public | BindingFlags.Instance)
                   .Where(m => m is PropertyInfo or FieldInfo)
                   .Where(m => !m.IsHidden());
    }

    public static PropertyInfo? GetPropertyByDisplayName(Type type, string displayName)
    {
        return type.GetProperties()
                   .FirstOrDefault(p =>
                                       (p.GetCustomAttribute<DisplayAttribute>()?.Name == displayName
                                        || p.Name == displayName)
                                       && !p.IsHidden());
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
                    .Where(p => !p.IsHidden())
                    .Select(p => new AutocompleteResult(DisplayHelper.GetDisplayName(p), p.Name))
                    .ToList();

        return AutocompletionResult.FromSuccess(props);
    }
}