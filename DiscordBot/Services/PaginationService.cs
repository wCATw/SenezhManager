using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
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
        IUserMessage message)
    {
        var guid    = Guid.NewGuid();
        var pages   = SplitIntoPages(elements);
        var session = new PaginationSession(title, creatorId, pages, message);

        memoryCache.Set(guid, session, new MemoryCacheEntryOptions
        {
            SlidingExpiration = SessionTimeout,
            PostEvictionCallbacks =
            {
                new PostEvictionCallbackRegistration
                {
                    EvictionCallback = void (_, value, _, _) =>
                    {
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                if (value is not PaginationSession evictedSession) return;
                                await evictedSession.Message.DeleteAsync();
                            }
                            catch (HttpException ex) when (ex.HttpCode == HttpStatusCode.NotFound)
                            {
                                logger.LogWarning("Message already deleted during eviction.");
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex, "Error during session eviction cleanup.");
                            }
                        });
                    },
                    State = client
                }
            }
        });


        return (BuildPagination(guid), guid);
    }

    public bool TryGetPagination(Guid guid, [NotNullWhen(true)] out PaginationSession? session)
    {
        return memoryCache.TryGetValue(guid, out session);
    }

    public void CloseSession(Guid guid)
    {
        memoryCache.Remove(guid);
    }

    public void ChangePage(Guid guid, int direction)
    {
        if (!TryGetPagination(guid, out var session))
            return;

        switch (direction)
        {
            case int.MinValue:
                session.PageIndex = 0;
                break;
            case int.MaxValue:
                session.PageIndex = session.Pages.Length - 1;
                break;
            default:
            {
                var newIndex = session.PageIndex + direction;
                if (newIndex >= 0 && newIndex < session.Pages.Length)
                    session.PageIndex = newIndex;
                break;
            }
        }
    }

    public (EmbedBuilder Embed, ComponentBuilder Component)? BuildPagination(Guid guid)
    {
        if (!TryGetPagination(guid, out var session))
            return null;

        var embed     = new EmbedBuilder().WithTitle(session!.Title);
        var component = new ComponentBuilder();

        foreach (var (name, content) in session!.Pages[session.PageIndex])
            embed.AddField(name, content);

        component
            .WithButton("⏮", $"pagination_button:first:{guid}", disabled: session.PageIndex <= 0)
            .WithButton("◀", $"pagination_button:previous:{guid}", disabled: session.PageIndex <= 0)
            .WithButton("▶", $"pagination_button:next:{guid}", disabled: session.PageIndex >= session.Pages.Length - 1)
            .WithButton("⏭", $"pagination_button:last:{guid}", disabled: session.PageIndex >= session.Pages.Length - 1)
            .WithButton("✖", $"pagination_button:close:{guid}", ButtonStyle.Danger);

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

    public IUserMessage? GetSessionMetadataFallback(Guid guid)
    {
        return !memoryCache.TryGetValue(guid, out PaginationSession? session) ? null : session?.Message;
    }
}

public class PaginationSession(
    string title,
    ulong creatorId,
    List<(string Name, string Content)>[] pages,
    IUserMessage message)
{
    public string Title { get; } = title;
    public ulong CreatorId { get; } = creatorId;
    public IUserMessage Message { get; } = message;
    public List<(string Name, string Content)>[] Pages { get; } = pages;
    public int PageIndex { get; set; }
}