using System;
using System.Threading.Tasks;
using Discord.Interactions;

namespace DiscordBot.Modules;

[Group("регистрация", "Модуль регистрации старых и новых участников в системе.")]
public class RegistrationModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("новенький", "Открывает регистрацию нового участника в системе.")]
    public async Task NewMemberRegister()
    {
        throw new NotImplementedException();
    }
}