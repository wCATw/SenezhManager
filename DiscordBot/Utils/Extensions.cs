using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiscordBot.Utils;

internal static class MemberInfoExtensions
{
    public static bool IsHidden(this MemberInfo member)
    {
        var display = member.GetCustomAttribute<DisplayAttribute>();
        return display?.Name is "SYSTEM" or "HIDDEN";
    }

    public static bool IsSystem(this MemberInfo member)
    {
        var display = member.GetCustomAttribute<DisplayAttribute>();
        return display?.Name is "SYSTEM";
    }
}

internal static class HostBuilderExtensions
{
    public static IHostBuilder ConfigureDiscord(this IHostBuilder builder,
                                                Action<DiscordSocketConfig, InteractionServiceConfig> configureOptions)
    {
        var clientConfig      = new DiscordSocketConfig();
        var interactionConfig = new InteractionServiceConfig();

        configureOptions(clientConfig, interactionConfig);

        var client      = new DiscordSocketClient(clientConfig);
        var interaction = new InteractionService(client, interactionConfig);

        builder.ConfigureServices(services =>
        {
            services.AddSingleton(clientConfig);
            services.AddSingleton(interactionConfig);
            services.AddSingleton(client);
            services.AddSingleton(interaction);
        });

        return builder;
    }

    public static IHostBuilder ConfigureCommands(this IHostBuilder builder,
                                                 Action<CommandServiceConfig> configureOptions)
    {
        var config = new CommandServiceConfig();

        configureOptions(config);

        var commands = new CommandService(config);

        builder.ConfigureServices(services =>
        {
            services.AddSingleton(config);
            services.AddSingleton(commands);
        });

        return builder;
    }
}