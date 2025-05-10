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
    static Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            if (!Directory.Exists("Database"))
                Directory.CreateDirectory("Database");

            var hostingTask = new HostBuilder()
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
                .RunConsoleAsync();

            return hostingTask;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application startup failed");
            return Task.FromException(ex);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}