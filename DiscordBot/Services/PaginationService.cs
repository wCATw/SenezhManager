using System;
using System.Collections.Generic;
using System.Linq;
using Discord;

namespace DiscordBot.Services;

public class PaginationService
{
    private const int PageSize = 5;
    private readonly Dictionary<Guid, PaginationSession> _sessions = new();

    public (EmbedBuilder Embed, ComponentBuilder Component) CreatePagination(
        ulong creatorId,
        string title,
        List<(string Name, string Content)> elements,
        (ulong GuildId, ulong ChannelId, ulong MessageId) messageTuple)
    {
        var guid  = Guid.NewGuid();
        var pages = SplitIntoPages(elements);
        _sessions[guid] = new PaginationSession(title, creatorId, pages)
        {
            MessageTuple = messageTuple
        };
        return BuildPagination(guid);
    }

    public bool TryGetPagination(Guid guid, out PaginationSession? pagination)
    {
        return _sessions.TryGetValue(guid, out pagination);
    }

    public void CloseSession(Guid guid)
    {
        _sessions.Remove(guid);
    }

    public void ChangePage(Guid guid, int direction)
    {
        if (!_sessions.TryGetValue(guid, out var session)) return;

        var newIndex                                                            = (int)session.PageIndex + direction;
        if (newIndex >= 0 && newIndex < session.Pages.Length) session.PageIndex = (uint)newIndex;
    }

    public (EmbedBuilder Embed, ComponentBuilder Component) BuildPagination(Guid guid)
    {
        if (!_sessions.TryGetValue(guid, out var session))
            throw new KeyNotFoundException("Session not found");

        var embed     = new EmbedBuilder().WithTitle(session.Title);
        var component = new ComponentBuilder();

        foreach (var (name, content) in session.Pages[session.PageIndex])
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
        if (_sessions.TryGetValue(guid, out var session))
            return session.MessageTuple;

        return null;
    }
}

public class PaginationSession(string title, ulong creatorId, List<(string Name, string Content)>[] pages)
{
    public string Title { get; set; } = title;
    public ulong CreatorId { get; set; } = creatorId;
    public (ulong GuildId, ulong ChannelId, ulong MessageId) MessageTuple { get; set; }
    public List<(string Name, string Content)>[] Pages { get; set; } = pages;
    public uint PageIndex { get; set; } = 0;
}