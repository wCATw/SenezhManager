using Discord.Interactions;
using DiscordBot.Services.Scoped.Interfaces;

namespace DiscordBot.Modules;

[Group("событие", "Модуль управления событиями.")]
public class EventManagerModule(IEventManagerService eventManager) : InteractionModuleBase<SocketInteractionContext>
{
}