using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using DiscordBot.Database.Entities;
using DiscordBot.Services.Interfaces;

namespace DiscordBot.Modules;

[Group("участник", "Модуль управления пользователями.")]
public class MemberManagerModule(IMemberManagerService memberManager) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("изменить-статус", "Изменяет статус активности участника.")]
    [RequireUserPermission(GuildPermission.ManageRoles)]
    public async Task ChangeMemberStatus(IUser user, string status)
    {
        var statusEntity = await memberManager.GetStatusByName(Context.Guild.Id, status);
        if (statusEntity is null)
        {
            await RespondAsync($"Статус '{status}' не найден.", ephemeral: true);
            return;
        }

        var member = await memberManager.GetMemberById(Context.Guild.Id, user.Id);

        if (member == null)
            await memberManager.CreateMember(Context.Guild.Id, new MemberEntity
            {
                MemberId = user.Id,
                StatusId = statusEntity.Id
            });
        else
            await memberManager.UpdateMember(Context.Guild.Id, user.Id, new MemberEntity
            {
                StatusId = statusEntity.Id
            });

        await RespondAsync($"Статус пользователя {user.Mention} обновлён на '{statusEntity.Name}'.", ephemeral: true);
    }

    [SlashCommand("изменить", "Изменяет данные об участнике.")]
    [RequireUserPermission(GuildPermission.ManageRoles)]
    public async Task ChangeMember(
        IUser user,
        string? call_sign = null,
        string? status = null,
        string? rank = null)
    {
        byte? statusId = null;
        byte? rankId = null;

        if (!string.IsNullOrEmpty(status))
        {
            var statusEntity = await memberManager.GetStatusByName(Context.Guild.Id, status);
            if (statusEntity is null)
            {
                await RespondAsync($"Статус '{status}' не найден.", ephemeral: true);
                return;
            }

            statusId = statusEntity.Id;
        }

        if (!string.IsNullOrEmpty(rank))
        {
            var rankEntity = await memberManager.GetRankByName(Context.Guild.Id, rank);
            if (rankEntity is null)
            {
                await RespondAsync($"Звание '{rank}' не найдено.", ephemeral: true);
                return;
            }

            rankId = rankEntity.Id;
        }

        var member = await memberManager.GetMemberById(Context.Guild.Id, user.Id);

        if (member == null)
        {
            await memberManager.CreateMember(Context.Guild.Id, new MemberEntity
            {
                MemberId = user.Id,
                CallSign = call_sign,
                StatusId = statusId,
                RankId = rankId
            });

            await RespondAsync($"Участник {user.Mention} добавлен.", ephemeral: true);
        }
        else
        {
            await memberManager.UpdateMember(Context.Guild.Id, user.Id, new MemberEntity
            {
                CallSign = call_sign,
                StatusId = statusId,
                RankId = rankId
            });

            await RespondAsync($"Участник {user.Mention} обновлён.", ephemeral: true);
        }
    }
}