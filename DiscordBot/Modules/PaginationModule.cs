using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Services;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Modules;

public class PaginationModule(PaginationService pagination, DiscordSocketClient client, ILogger<PaginationModule> logger)
    : InteractionModuleBase<SocketInteractionContext>
{
    [ComponentInteraction("pagination_button:*:*", true)]
    public async Task HandlePaginationButton(string action, string guidStr)
    {
        await DeferAsync();

        var guid = Guid.Parse(guidStr);

        switch (action)
        {
            case "first":
                await HandlePaginationInteraction(guid, int.MinValue);
                break;
            case "previous":
                await HandlePaginationInteraction(guid, -1);
                break;
            case "next":
                await HandlePaginationInteraction(guid, +1);
                break;
            case "last":
                await HandlePaginationInteraction(guid, int.MaxValue);
                break;
            case "close":
                await Context.Interaction.DeleteOriginalResponseAsync();
                pagination.CloseSession(guid);
                break;
            default:
                logger.LogError($"Unhandled action \"{action}\"!");
                break;
        }
    }

    private async Task HandlePaginationInteraction(Guid guid, int direction)
    {
        var message = GetInteractionMessage(guid);

        if (message == null)
        {
            await Context.Interaction.DeleteOriginalResponseAsync();
            await FollowupAsync("Извините, пагинация устарела.", ephemeral: true);
            return;
        }

        pagination.ChangePage(guid, direction);
        var content = pagination.BuildPagination(guid);
        await message!.ModifyAsync(msg =>
        {
            msg.Embed      = content?.Embed.Build();
            msg.Components = content?.Component.Build();
        });
    }

    private IUserMessage? GetInteractionMessage(Guid guid)
    {
        if (!pagination.TryGetPagination(guid, out var foundSession))
            return null;

        if (foundSession.CreatorId != Context.User.Id)
            return null;

        return foundSession.Message;
    }
}