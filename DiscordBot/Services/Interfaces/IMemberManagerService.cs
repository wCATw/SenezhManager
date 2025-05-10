using System.Threading.Tasks;
using DiscordBot.Database.Entities;

namespace DiscordBot.Services.Interfaces;

public interface IMemberManagerService
{
    /// <summary>
    ///  Берет из БД запись участника по ID, если она существует.
    /// </summary>
    /// <param name="guildId">ID сервера из контекста</param>
    /// <param name="memberId">ID целевого участника</param>
    /// <returns>Сущность целевого участника</returns>
    Task<MemberEntity?> GetMemberById(ulong guildId, ulong memberId);

    /// <summary>
    ///  Создает запись нового участника и сразу её заполняет.
    /// </summary>
    /// <param name="guildId">ID сервера из контекста</param>
    /// <param name="memberEnt">Сущность для заполнения данных при создании</param>
    /// <returns>ID созданной записи участника</returns>
    Task<ulong> CreateMember(ulong guildId, MemberEntity? memberEnt = null);

    /// <summary>
    ///  Обновляет существующую запись участника.
    /// </summary>
    /// <param name="guildId">ID сервера из контекста</param>
    /// <param name="memberId">ID целевой записи события</param>
    /// <param name="memberEnt">Сущность с данными</param>
    Task UpdateMember(ulong guildId, ulong memberId, MemberEntity? memberEnt = null);

    Task<MemberStatus?> GetStatusByName(ulong guildId, string name);
    Task<MemberRank?> GetRankByName(ulong guildId, string name);
}