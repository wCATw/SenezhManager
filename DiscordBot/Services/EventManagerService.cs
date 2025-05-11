using DiscordBot.Database;
using DiscordBot.Services.Interfaces;

namespace DiscordBot.Services;

public class EventManagerService(AppDbContext dbContext) : IEventManagerService
{
}