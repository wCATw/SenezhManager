using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using System;
using Discord.Interactions;

namespace Microsoft.Extensions.DependencyInjection;

internal static class HostBuilderExtensions
{
    public static IHostBuilder ConfigureDiscord(this IHostBuilder builder,
        Action<DiscordSocketConfig, InteractionServiceConfig> configureOptions)
    {
        var clientConfig = new DiscordSocketConfig();
        var interactionConfig = new InteractionServiceConfig();
        configureOptions(clientConfig, interactionConfig);

        var client = new DiscordSocketClient(clientConfig);
        
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