using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Services;

namespace DiscordBot.Modules;

public class PaginationModule(PaginationService pagination, DiscordSocketClient client)
    : InteractionModuleBase<SocketInteractionContext>
{
    [ComponentInteraction("pagination_next_page_button:*", true)]
    public async Task NextPageButton(string guidStr)
    {
        await DeferAsync();
        await HandlePaginationInteraction(Guid.Parse(guidStr), +1);
    }

    [ComponentInteraction("pagination_previous_page_button:*", true)]
    public async Task PreviousPageButton(string guidStr)
    {
        await DeferAsync();
        await HandlePaginationInteraction(Guid.Parse(guidStr), -1);
    }

    [ComponentInteraction("pagination_close_button:*", true)]
    public async Task CloseButton(string guidStr)
    {
        await DeferAsync();
        await Context.Interaction.DeleteOriginalResponseAsync();
        pagination.CloseSession(Guid.Parse(guidStr));
    }

    private async Task HandlePaginationInteraction(Guid guid, int direction)
    {
        var message = await GetInteractionMessage(guid);

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

    private async Task<IUserMessage?> GetInteractionMessage(Guid guid)
    {
        if (!pagination.TryGetPagination(guid, out var foundSession) || foundSession is null)
            return null;

        if (foundSession.CreatorId != Context.User.Id)
            return null;

        var guild = client.GetGuild(foundSession.MessageTuple.GuildId);

        if (guild?.GetChannel(foundSession.MessageTuple.ChannelId) is not ITextChannel channel)
            return null;

        var message = await channel.GetMessageAsync(foundSession.MessageTuple.MessageId);

        return message is not IUserMessage userMessage ? null : userMessage;
    }
}