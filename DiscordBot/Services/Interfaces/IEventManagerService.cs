using System.Threading.Tasks;
using DiscordBot.Database.Entities;

namespace DiscordBot.Services.Interfaces;

public interface IEventManagerService
{
    /// <summary>
    ///  Берет из БД запись события по ID, если она существует.
    /// </summary>
    /// <param name="guildId">ID сервера из контекста</param>
    /// <param name="eventId">ID целевой записи события</param>
    /// <returns>Сущность целевой записи события</returns>
    Task<EventEntity?> GetEventById(ulong guildId, ulong eventId);
    
    /// <summary>
    ///  Создает новоую запись события и сразу её заполняет.
    /// </summary>
    /// <param name="guildId">ID сервера из контекста</param>
    /// <param name="eventEnt">Сущность для заполнения данных при создании</param>
    /// <returns>ID созданной записи события</returns>
    Task<ulong> CreateEvent(ulong guildId, EventEntity? eventEnt = null);
    
    /// <summary>
    ///  Обновляет существующую запись события.
    /// </summary>
    /// <param name="guildId">ID сервера из контекста</param>
    /// <param name="eventId">ID целевой записи события</param>
    /// <param name="eventEnt">Сущность с данными</param>
    Task UpdateEvent(ulong guildId, ulong eventId, EventEntity? eventEnt = null);
}