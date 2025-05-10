using System.Threading.Tasks;
using Discord.Interactions;

namespace DiscordBot.Modules;

public class PingPongModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "Pong!")]
    public Task PongAsync()
    {
        return base.ReplyAsync("Pong!");
    }
}