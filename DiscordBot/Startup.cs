using System;
using System.IO;
using System.Threading.Tasks;
using DiscordBot.Database;
using DiscordBot.Services;
using DiscordBot.Services;
using DiscordBot.Services.Interfaces;
using DiscordBot.Services;
using DiscordBot.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DiscordBot;

internal static class Startup
{
    private static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
                     .Enrich.FromLogContext()
                     .WriteTo.Console()
                     .CreateLogger();

        try
        {
            if (!Directory.Exists("data"))
                Directory.CreateDirectory("data");

            var host = new HostBuilder()
#if DEBUG
                       .UseEnvironment(Environments.Development)
#else
                .UseEnvironment(Environments.Production)
#endif
                       .ConfigureAppConfiguration(builder =>
                       {
                           var config = new ConfigurationBuilder()
                                        .SetBasePath(Directory.GetCurrentDirectory())
                                        .AddJsonFile("config.json", false, true)
                                        .Build();

                           builder.AddConfiguration(config);
                       })
                       .ConfigureDiscord((socketOptions, interactionOptions) => { })
                       .ConfigureCommands(options => { })
                       .ConfigureServices((context, services) =>
                       {
                           services.AddScoped<ISettingsManagerService, SettingsManagerService>();
                           services.AddScoped<IDbManagerService, DbManagerService>();
                           services.AddScoped<IEventManagerService, EventManagerService>();

                           services.AddSingleton<CommandHandlerService>();
                           services.AddSingleton<PaginationService>();

                           services.AddDbContext<AppDbContext>(options => { options.UseSqlite("Data Source=data/bot.db"); });

                           services.AddHostedService<MaintenanceService>();
                           services.AddHostedService<DiscordBotService>();
                       })
                       .ConfigureLogging((context, logging) =>
                       {
                           logging.ClearProviders();
                           logging.AddSerilog();
                       })
                       .UseSerilog()
                       .Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var dbContext = services.GetRequiredService<AppDbContext>();
                    await dbContext.Database.MigrateAsync();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An error occurred while migrating the database!");
                    throw;
                }
            }

            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application startup failed!");
            throw;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}