using System.Threading.Tasks;

namespace DiscordBot.Services.Interfaces;

public interface ICommandHandlerService
{
    Task InitializeAsync();
}