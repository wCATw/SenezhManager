using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace DiscordBot.Services.Hosted;

public class MaintenanceService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
    }
}