using System.ComponentModel.DataAnnotations.Schema;

namespace DiscordBot.Database.Entities;

[Table("members")]
public class MemberEntity
{
    public ulong? GuildId { get; set; }
    public ulong? MemberId { get; set; }

    public string? CallSign { get; set; }
    public string? StatusName { get; set; }
    public string? RankName { get; set; }

    public MemberStatusEntity? Status { get; set; }
    public MemberRankEntity? Rank { get; set; }
}