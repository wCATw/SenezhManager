using System;
using System.Threading;
using System.Threading.Tasks;
using DiscordBot.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Services;

public class MaintenanceService(ILogger<MaintenanceService> logger, IServiceScopeFactory scopeFactory) : BackgroundService, IDisposable, IAsyncDisposable
{
    private bool _disposed;
    private Timer _mainTimer = null!;

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        await _mainTimer.DisposeAsync();
        _disposed = true;
    }

    public override void Dispose()
    {
        if (_disposed)
            return;

        base.Dispose();
        _mainTimer.Dispose();
        _disposed = true;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _mainTimer = new Timer(TimerRoutine, null, TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(30));

        await Task.CompletedTask;
    }

    private void TimerRoutine(object? state)
    {
        try
        {
            using var scope        = scopeFactory.CreateScope();
            var       eventManager = scope.ServiceProvider.GetService<IEventManagerService>();

            eventManager!.RoutineCheck().Wait();

            logger.LogInformation("Timer executed.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
        }
    }
}