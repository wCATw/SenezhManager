namespace DiscordBot.Services.Interfaces;

public interface IMemberManagerService
{
//     /// <summary>
//     /// Получает из базы данных запись участника по его ID.
//     /// </summary>
//     /// <param name="guildId">ID сервера (гильдии).</param>
//     /// <param name="memberId">ID участника.</param>
//     /// <returns>Сущность участника или null, если он не найден.</returns>
//     Task<MemberEntity?> GetMemberById(ulong guildId, ulong memberId);
//
//     /// <summary>
//     /// Создаёт нового участника в базе данных и сохраняет переданные данные.
//     /// </summary>
//     /// <param name="guildId">ID сервера (гильдии).</param>
//     /// <param name="memberEnt">Сущность участника для заполнения данных (необязательно).</param>
//     /// <returns>ID созданного участника.</returns>
//     Task<ulong> CreateMember(ulong guildId, MemberEntity? memberEnt = null);
//
//     /// <summary>
//     /// Обновляет существующую запись участника новыми данными.
//     /// </summary>
//     /// <param name="guildId">ID сервера (гильдии).</param>
//     /// <param name="memberId">ID участника, которого нужно обновить.</param>
//     /// <param name="memberEnt">Сущность с обновлёнными данными (необязательно).</param>
//     Task UpdateMember(ulong guildId, ulong memberId, MemberEntity? memberEnt = null);
//
//     /// <summary>
//     /// Получает статус участника по его названию (регистронезависимо).
//     /// </summary>
//     /// <param name="guildId">ID сервера (гильдии).</param>
//     /// <param name="name">Название статуса (в нижнем регистре или любом другом).</param>
//     /// <returns>Сущность статуса или null, если не найден.</returns>
//     Task<MemberStatus?> GetStatusByName(ulong guildId, string name);
//
//     /// <summary>
//     /// Получает звание участника по его названию (регистронезависимо).
//     /// </summary>
//     /// <param name="guildId">ID сервера (гильдии).</param>
//     /// <param name="name">Название звания (в нижнем регистре или любом другом).</param>
//     /// <returns>Сущность звания или null, если не найдено.</returns>
//     Task<MemberRank?> GetRankByName(ulong guildId, string name);
}