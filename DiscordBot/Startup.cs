using System;
using System.IO;
using System.Threading.Tasks;
using DiscordBot.Database;
using DiscordBot.Services;
using DiscordBot.Services.Interfaces;
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
            if (!Directory.Exists("Database"))
                Directory.CreateDirectory("Database");

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
                        .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                        .Build();

                    builder.AddConfiguration(config);
                })
                .ConfigureDiscord((socketOptions, interactionOptions) => { })
                .ConfigureCommands(options => { })
                .ConfigureServices((context, services) =>
                {
                    services.AddScoped<IEventManagerService, EventManagerService>();
                    services.AddScoped<ISettingsManagerService, SettingsManagerService>();
                    services.AddScoped<IMemberManagerService, MemberManagerService>();
                    services.AddSingleton<CommandHandlerService>();

                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseSqlite("Data Source=Database/bot.db");
                    });

                    services.AddHostedService<DiscordBotService>();
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddSerilog();
                })
                .UseSerilog()
                .Build();

            // Применяем миграции перед запуском хоста
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var dbContext = services.GetRequiredService<AppDbContext>();
                    dbContext.Database.Migrate();
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
            Log.CloseAndFlush();
        }
    }
}