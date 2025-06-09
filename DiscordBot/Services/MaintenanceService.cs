using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace DiscordBot.Services;

public class MaintenanceService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
    }
}