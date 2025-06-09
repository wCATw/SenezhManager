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
        if (!TryValidateInteraction(guid, out var message))
        {
            await Context.Interaction.DeleteOriginalResponseAsync();
            await FollowupAsync("Извините, пагинация устарела.", ephemeral: true);
            return;
        }

        pagination.ChangePage(guid, direction);
        var (embed, component) = pagination.BuildPagination(guid);
        await message!.ModifyAsync(msg =>
        {
            msg.Embed      = embed.Build();
            msg.Components = component.Build();
        });
    }

    private bool TryValidateInteraction(Guid guid, out IUserMessage? message)
    {
        message = null;

        if (!pagination.TryGetPagination(guid, out var foundSession) || foundSession is null)
        {
            _ = Task.Run(async () =>
            {
                var tempSession = pagination.GetSessionMetadataFallback(guid);
                if (tempSession is not null)
                {
                    var guild = client.GetGuild(tempSession.Value.GuildId);

                    if (guild?.GetChannel(tempSession.Value.ChannelId) is not ITextChannel textChannel) return;

                    var msg = await textChannel.GetMessageAsync(tempSession.Value.MessageId);
                    if (msg is IUserMessage userMessage)
                        await userMessage.DeleteAsync();
                }
            });

            return false;
        }

        if (foundSession.CreatorId != Context.User.Id)
            return false;

        var guild = client.GetGuild(foundSession.MessageTuple.GuildId);

        if (guild?.GetChannel(foundSession.MessageTuple.ChannelId) is not ITextChannel channel) return false;

        var msg = channel.GetMessageAsync(foundSession.MessageTuple.MessageId).Result;
        if (msg is not IUserMessage userMsg) return false;

        message = userMsg;
        return true;
    }
}