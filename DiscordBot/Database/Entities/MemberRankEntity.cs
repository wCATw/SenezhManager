using System.ComponentModel.DataAnnotations.Schema;

namespace DiscordBot.Database.Entities;

[Table("ranks")]
public class MemberRankEntity
{
    public ulong? GuildId { get; set; }
    public string? Name { get; set; }
    public int? Order { get; set; }
}