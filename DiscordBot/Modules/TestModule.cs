using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using DiscordBot.Services;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Modules;

#if DEBUG
[Group("debug", "FOR DEVELOPERS.")]
public class TestModule(ILogger<TestModule> logger, PaginationService pagination)
    : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("pagination", "just test")]
    public async Task TestPaginationCommand()
    {
        await DeferAsync();

        var message = await GetOriginalResponseAsync();
        if (message.Channel is not IGuildChannel guildChannel)
        {
            await FollowupAsync("IS NOT GUILD CHANNEL!");
            return;
        }

        var elements = Enumerable.Range(0, 10)
                                 .Select(i => ($"MEOW {i}", "MEOW MEOW MEOW"))
                                 .ToList();

        var paginationTuple = pagination.CreatePagination(
                                                          Context.User.Id,
                                                          "DEBUG PAGINATION",
                                                          elements,
                                                          (guildChannel.GuildId, guildChannel.Id, message.Id)
                                                         );

        await FollowupAsync(
                            embed: paginationTuple.Contnet?.Embed.Build(),
                            components: paginationTuple.Contnet?.Component.Build()
                           );
    }
}
#endif