using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Services;

public class PaginationService(IMemoryCache memoryCache, DiscordSocketClient client, ILogger<PaginationService> logger)
{
    private const int PageSize = 5;
    private readonly TimeSpan SessionTimeout = TimeSpan.FromMinutes(5);

    public ((EmbedBuilder Embed, ComponentBuilder Component)? Contnet, Guid Guid) CreatePagination(
        ulong creatorId,
        string title,
        List<(string Name, string Content)> elements,
        (ulong GuildId, ulong ChannelId, ulong MessageId) messageTuple)
    {
        var guid    = Guid.NewGuid();
        var pages   = SplitIntoPages(elements);
        var session = new PaginationSession(title, creatorId, pages, messageTuple);

        memoryCache.Set(guid, session, new MemoryCacheEntryOptions
        {
            SlidingExpiration = SessionTimeout,
            PostEvictionCallbacks =
            {
                new PostEvictionCallbackRegistration
                {
                    EvictionCallback = async void (_, value, _, _) =>
                    {
                        try
                        {
                            if (value is not PaginationSession expiredSession) return;

                            var guild = client.GetGuild(expiredSession.MessageTuple.GuildId);

                            if (guild?.GetChannel(expiredSession.MessageTuple.ChannelId) is not ITextChannel channel)
                                return;

                            var message = await channel.GetMessageAsync(expiredSession.MessageTuple.MessageId);
                            if (message is IUserMessage userMessage) await userMessage.DeleteAsync();
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex.ToString());
                        }
                    },
                    State = client
                }
            }
        });


        return (BuildPagination(guid), guid);
    }

    public bool TryGetPagination(Guid guid, out PaginationSession? session)
    {
        return memoryCache.TryGetValue(guid, out session);
    }

    public void CloseSession(Guid guid)
    {
        memoryCache.Remove(guid);
    }

    public void ChangePage(Guid guid, int direction)
    {
        if (!memoryCache.TryGetValue(guid, out PaginationSession? session)) return;

        if (session == null) return;

        var newIndex = session.PageIndex + direction;
        if (newIndex >= 0 && newIndex < session.Pages.Length)
            session.PageIndex = newIndex;
    }

    public (EmbedBuilder Embed, ComponentBuilder Component)? BuildPagination(Guid guid)
    {
        if (!memoryCache.TryGetValue(guid, out PaginationSession? session)) return null;

        var embed     = new EmbedBuilder().WithTitle(session!.Title);
        var component = new ComponentBuilder();

        foreach (var (name, content) in session!.Pages[session.PageIndex])
            embed.AddField(name, content);

        component
            .WithButton("PREVIOUS", $"pagination_previous_page_button:{guid}", disabled: session.PageIndex <= 0)
            .WithButton("NEXT", $"pagination_next_page_button:{guid}", disabled: session.PageIndex >= session.Pages.Length - 1)
            .WithButton("CLOSE", $"pagination_close_button:{guid}");

        return (embed, component);
    }

    private static List<(string Name, string Content)>[] SplitIntoPages(List<(string Name, string Content)> elements)
    {
        ArgumentNullException.ThrowIfNull(elements);

        return elements
               .Select((item, index) => new { item, index })
               .GroupBy(x => x.index / PageSize)
               .Select(g => g.Select(x => x.item).ToList())
               .ToArray();
    }

    public (ulong GuildId, ulong ChannelId, ulong MessageId)? GetSessionMetadataFallback(Guid guid)
    {
        return !memoryCache.TryGetValue(guid, out PaginationSession? session) ? null : session?.MessageTuple;
    }
}

public class PaginationSession(
    string title,
    ulong creatorId,
    List<(string Name, string Content)>[] pages,
    (ulong GuildId, ulong ChannelId, ulong MessageId) messageTuple)
{
    public string Title { get; } = title;
    public ulong CreatorId { get; } = creatorId;
    public (ulong GuildId, ulong ChannelId, ulong MessageId) MessageTuple { get; } = messageTuple;
    public List<(string Name, string Content)>[] Pages { get; } = pages;
    public int PageIndex { get; set; }
}