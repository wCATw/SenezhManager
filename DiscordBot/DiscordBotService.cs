using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DiscordBot;

internal sealed class DiscordBotService(
    DiscordSocketClient client,
    InteractionService intService,
    IConfiguration config,
    ICommandHandlerService commandHandlerService,
    ILogger<DiscordBotService> logger)
    : IHostedService
{
    private Task Log(LogMessage message)
    {
        var logLevel = message.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Debug,
            LogSeverity.Debug => LogLevel.Trace,
            _ => LogLevel.Information
        };
        
        logger.Log(logLevel, message.Exception, $"{message.Source}: {message.Message}");
        
        return Task.CompletedTask;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var commands = intService;

        client.Log += Log;
        client.Ready += async () =>
        {
            await commands.RegisterCommandsGloballyAsync();
            logger.LogInformation($"Client is {client.CurrentUser}");
        };
        await client.LoginAsync(TokenType.Bot, config["Discord:Token"]);
        await client.StartAsync();
        await commandHandlerService.InitializeAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await client.StopAsync();
    }
}