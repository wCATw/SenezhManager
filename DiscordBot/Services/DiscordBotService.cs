using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace DiscordBot.Services;

internal sealed class DiscordBotService(
    DiscordSocketClient client,
    InteractionService intService,
    IConfiguration config,
    CommandHandlerService commandHandlerService,
    ILogger<DiscordBotService> logger)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var commands = intService;

        client.Log += LogAsync;
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

    private static async Task LogAsync(LogMessage message)
    {
        var severity = message.Severity switch
        {
            LogSeverity.Critical => LogEventLevel.Fatal,
            LogSeverity.Error    => LogEventLevel.Error,
            LogSeverity.Warning  => LogEventLevel.Warning,
            LogSeverity.Info     => LogEventLevel.Information,
            LogSeverity.Verbose  => LogEventLevel.Verbose,
            LogSeverity.Debug    => LogEventLevel.Debug,
            _                    => LogEventLevel.Information
        };
        Log.Write(severity, message.Exception, "[{Source}] {Message}", message.Source, message.Message);
        await Task.CompletedTask;
    }
}