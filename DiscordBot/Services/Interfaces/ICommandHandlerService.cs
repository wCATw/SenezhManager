using System.Threading.Tasks;

namespace DiscordBot.Services.Interfaces;

public interface ICommandHandlerService
{
    /// <summary>
    ///  Инициализирует обработчик команд.
    /// </summary>
    Task InitializeAsync();
}