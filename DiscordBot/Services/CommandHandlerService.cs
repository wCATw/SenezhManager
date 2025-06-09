using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Services;

internal sealed class CommandHandlerService(
    DiscordSocketClient client,
    InteractionService interactionService,
    IServiceProvider service,
    ILogger<CommandHandlerService> logger)
{
    public async Task InitializeAsync()
    {
        await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), service);

        client.InteractionCreated += HandleInteraction;

        interactionService.SlashCommandExecuted     += SlashCommandExecuted;
        interactionService.ContextCommandExecuted   += ContextCommandExecuted;
        interactionService.ComponentCommandExecuted += ComponentCommandExecuted;
        interactionService.ModalCommandExecuted     += ModalCommandExecuted;

        logger.LogInformation("Hooking commands");
    }

    private async Task ComponentCommandExecuted(ComponentCommandInfo componentCommandInfo,
                                                IInteractionContext interactionContext, IResult result)
    {
        await HandleResult(interactionContext, result);
    }

    private async Task ModalCommandExecuted(ModalCommandInfo modalCommandInfo, IInteractionContext interactionContext,
                                            IResult result)
    {
        await HandleResult(interactionContext, result);
    }

    private async Task ContextCommandExecuted(ContextCommandInfo contextCommandInfo,
                                              IInteractionContext interactionContext, IResult result)
    {
        await HandleResult(interactionContext, result);
    }

    private async Task SlashCommandExecuted(SlashCommandInfo slashCommand, IInteractionContext interactionContext,
                                            IResult result)
    {
        await HandleResult(interactionContext, result);
    }

    private async Task HandleResult(IInteractionContext interactionContext, IResult result)
    {
        if (!result.IsSuccess)
        {
            var execResult = (ExecuteResult)result;
            logger.LogError($"{execResult.ErrorReason}\n{execResult.Exception}");
#if DEBUG
            var errorStr = $"# Ошибка!\n```js\n{execResult.ErrorReason}\n{execResult.Exception}\n```";

            if (errorStr.Length > 2000)
            {
                await interactionContext.Interaction.FollowupAsync("Ошибка больше 2000 символов, не могу обработать..", ephemeral: true);
                return;
            }

            await interactionContext.Interaction.FollowupAsync(errorStr, ephemeral: true);
#else
            await interactionContext.Interaction.FollowupAsync("Произошла ошибка!", ephemeral: true);
#endif
        }
    }

    private async Task HandleInteraction(SocketInteraction socketInteraction)
    {
        try
        {
            var socketInteractionContext = new SocketInteractionContext(client, socketInteraction);
            await interactionService.ExecuteCommandAsync(socketInteractionContext, service);
        }
        catch (Exception e)
        {
            if (socketInteraction.Type == InteractionType.ApplicationCommand)
            {
                await socketInteraction.GetOriginalResponseAsync()
                                       .ContinueWith(async msg => await msg.Result.DeleteAsync());

                logger.LogError(e.ToString());
#if DEBUG
                var errorStr = $"# Ошибка!\n```js\n{e}\n```";

                if (errorStr.Length > 2000)
                {
                    await socketInteraction.FollowupAsync("Ошибка больше 2000 символов, не могу обработать..",
                                                          ephemeral: true);
                    return;
                }

                await socketInteraction.FollowupAsync(errorStr, ephemeral: true);
#else
                await socketInteraction.FollowupAsync("Произошла ошибка!", ephemeral: true);
#endif
            }
        }
    }
}