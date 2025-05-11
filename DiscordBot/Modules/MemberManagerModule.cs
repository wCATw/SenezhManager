using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Services.Interfaces;

namespace DiscordBot.Modules;

[Group("участник", "Модуль управления пользователями.")]
public class MemberManagerModule(IMemberManagerService memberManager) : InteractionModuleBase<SocketInteractionContext>
{
    private static async Task SendPagedEmbedAsync<T>(SocketInteractionContext context, string title, List<T> items,
        Func<T, string> formatter)
    {
        const int pageSize = 10;
        var pages = items.Chunk(pageSize).ToList();
        var pageIndex = 0;

        async Task<IUserMessage> SendPage()
        {
            var embed = new EmbedBuilder()
                .WithTitle(title)
                .WithColor(Color.DarkBlue)
                .WithDescription(string.Join("\n", pages[pageIndex].Select(formatter)))
                .WithFooter($"Страница {pageIndex + 1}/{pages.Count}");

            var message = await context.Interaction.FollowupAsync(embed: embed.Build(),
                components: GetNavigationButtons().Build(), ephemeral: true);
            return message;
        }

        ComponentBuilder GetNavigationButtons()
        {
            return new ComponentBuilder()
                .WithButton("⬅️", "prev", disabled: pageIndex == 0)
                .WithButton("➡️", "next", disabled: pageIndex >= pages.Count - 1);
        }

        var sentMessage = await SendPage();

        async Task HandleComponent(SocketMessageComponent component)
        {
            if (component.User.Id != context.User.Id) return;

            if (component.Data.CustomId == "prev" && pageIndex > 0)
                pageIndex--;
            else if (component.Data.CustomId == "next" && pageIndex < pages.Count - 1)
                pageIndex++;

            await component.UpdateAsync(msg =>
            {
                msg.Embed = new EmbedBuilder()
                    .WithTitle(title)
                    .WithColor(Color.DarkBlue)
                    .WithDescription(string.Join("\n", pages[pageIndex].Select(formatter)))
                    .WithFooter($"Страница {pageIndex + 1}/{pages.Count}")
                    .Build();

                msg.Components = GetNavigationButtons().Build();
            });
        }

        context.Client.ButtonExecuted += HandleComponent;

        _ = Task.Delay(TimeSpan.FromMinutes(5)).ContinueWith(_ =>
        {
            context.Client.ButtonExecuted -= HandleComponent;
        });
    }

    [SlashCommand("изменить", "Изменяет данные об участнике.")]
    [RequireUserPermission(GuildPermission.ManageRoles)]
    public async Task ChangeMember(IUser user, string? new_callsign, string? new_rank, string? new_status)
    {
        await DeferAsync(true);
        var success =
            await memberManager.TryChangeMemberAsync(Context.Guild.Id, user.Id, new_callsign, new_rank, new_status);
        await FollowupAsync(success ? "Участник обновлён." : "Произошла ошибка!");
    }

    [SlashCommand("добавить", "Добавляет участника.")]
    [RequireUserPermission(GuildPermission.ManageRoles)]
    public async Task AddMember(IUser user, string? callSign = null, string? rank = null, string? status = null)
    {
        await DeferAsync(true);
        var success = await memberManager.TryAddMemberAsync(Context.Guild.Id, user.Id, callSign, rank, status);
        await FollowupAsync(success ? "Участник добавлен." : "Ошибка!");
    }

    [SlashCommand("удалить", "Удаляет участника.")]
    [RequireUserPermission(GuildPermission.ManageRoles)]
    public async Task RemoveMember(IUser user)
    {
        await DeferAsync(true);
        var success = await memberManager.TryRemoveMemberAsync(Context.Guild.Id, user.Id);
        await FollowupAsync(success ? "Участник удалён." : "Ошибка!");
    }

    [SlashCommand("список", "Показывает список всех участников.")]
    public async Task ListMembers()
    {
        await DeferAsync(true);
        var members = await memberManager.GetAllMembersAsync(Context.Guild.Id);
        if (members.Count == 0)
        {
            await FollowupAsync("Список пуст.", ephemeral: true);
            return;
        }

        await SendPagedEmbedAsync(Context, "Участники", members, m =>
            $"<@{m.MemberId}> | Звание: `{m.RankName}` | Статус: `{m.StatusName}` | Позывной: `{m.CallSign}`");
    }

    [Group("статус", "Управление статусами участников.")]
    public class StatusCommands(IMemberManagerService memberManager) : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("добавить", "Добавляет новый статус.")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task AddStatus(string status_name)
        {
            await DeferAsync(true);
            var success = await memberManager.TryAddStatusAsync(Context.Guild.Id, status_name);
            await FollowupAsync(success ? "Статус добавлен." : "Произошла ошибка!");
        }

        [SlashCommand("удалить", "Удаляет статус.")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task RemoveStatus(string status_name)
        {
            await DeferAsync(true);
            var success = await memberManager.TryRemoveStatusAsync(Context.Guild.Id, status_name);
            await FollowupAsync(success ? "Статус удалён." : "Произошла ошибка!");
        }

        [SlashCommand("изменить", "Изменяет существующий статус.")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task ChangeStatus(string status_name, string? new_status_name)
        {
            await DeferAsync(true);
            var success = await memberManager.TryChangeStatusAsync(Context.Guild.Id, status_name, new_status_name);
            await FollowupAsync(success ? "Статус изменён." : "Произошла ошибка!");
        }

        [SlashCommand("список", "Список всех статусов.")]
        public async Task ListStatuses()
        {
            await DeferAsync(true);
            var statuses = await memberManager.GetAllStatusesAsync(Context.Guild.Id);
            if (statuses.Count == 0)
            {
                await FollowupAsync("Нет статусов.", ephemeral: true);
                return;
            }

            await SendPagedEmbedAsync(Context, "Статусы", statuses, s => $"`{s.Name}`");
        }
    }

    [Group("ранг", "Управление рангами участников.")]
    public class RankCommands(IMemberManagerService memberManager) : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("добавить", "Добавляет новый ранг.")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task AddRank(string rank_name, int order)
        {
            await DeferAsync(true);
            var success = await memberManager.TryAddRankAsync(Context.Guild.Id, rank_name, order);
            await FollowupAsync(success ? "Ранг добавлен." : "Произошла ошибка!");
        }

        [SlashCommand("удалить", "Удаляет ранг.")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task RemoveRank(string rank_name)
        {
            await DeferAsync(true);
            var success = await memberManager.TryRemoveRankAsync(Context.Guild.Id, rank_name);
            await FollowupAsync(success ? "Ранг удалён." : "Произошла ошибка!");
        }

        [SlashCommand("изменить", "Изменяет существующий ранг.")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task ChangeRank(string rank_name, string? new_rank_name, int? order)
        {
            await DeferAsync(true);
            var success = await memberManager.TryChangeRankAsync(Context.Guild.Id, rank_name, new_rank_name, order);
            await FollowupAsync(success ? "Ранг изменён." : "Произошла ошибка!");
        }

        [SlashCommand("список", "Список всех рангов.")]
        public async Task ListRanks()
        {
            await DeferAsync(true);
            var ranks = await memberManager.GetAllRanksAsync(Context.Guild.Id);
            if (ranks.Count == 0)
            {
                await FollowupAsync("Нет рангов.", ephemeral: true);
                return;
            }

            await SendPagedEmbedAsync(Context, "Звания", ranks, r => $"`{r.Order:00}` | `{r.Name}`");
        }
    }
}