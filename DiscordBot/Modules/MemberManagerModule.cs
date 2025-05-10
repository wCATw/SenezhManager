using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;

namespace DiscordBot.Modules;

[Group("участник", "Модуль управления пользователями.")]
public class MemberManagerModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("изменить-статус", "Изменяет статус активности участника.")]
    [RequireUserPermission(GuildPermission.ManageRoles)]
    public async Task ChangeMemberStatus()
    {
        throw new NotImplementedException();
    }

    [SlashCommand("изменить", "Изменяет данные об участнике.")]
    [RequireUserPermission(GuildPermission.ManageRoles)]
    public async Task ChangeMember(IUser? user = null)
    {
        throw new NotImplementedException();
    }
}