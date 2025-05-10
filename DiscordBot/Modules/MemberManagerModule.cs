using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;

namespace DiscordBot.Modules;

[Group("участник", "Модуль управления пользователями.")]
public class MemberManagerModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("создать", "Создает новую запись об участнике.")]
    public async Task CreateMember()
    {
        throw new NotImplementedException();
    }

    [SlashCommand("изменить-статус", "Изменяет статус активности участника.")]
    public async Task ChangeMemberStatus()
    {
        throw new NotImplementedException();
    }

    [SlashCommand("изменить", "Изменяет данные об участнике.")]
    public async Task ChangeMember(IUser? user = null)
    {
        throw new NotImplementedException();
    }
}