using Discord.Interactions;
using DiscordBot.Services.Scoped.Interfaces;

namespace DiscordBot.Modules;

public class EventModule(IEventManagerService eventManager) : InteractionModuleBase<SocketInteractionContext>
{
}