using System.ComponentModel.DataAnnotations.Schema;

namespace DiscordBot.Database.Entities;

[Table("statuses")]
public class MemberStatus
{
    public ulong? GuildId { get; set; }
    public byte? Id { get; set; }
    public string? Name { get; set; }
}

[Table("ranks")]
public class MemberRank
{
    public ulong? GuildId { get; set; }
    public byte? Id { get; set; }
    public string? Name { get; set; }
    public int? Order { get; set; }
}

[Table("members")]
public class MemberEntity
{
    public ulong? GuildId { get; set; }
    public ulong? MemberId { get; set; }
    public string? CallSign { get; set; }

    public byte? StatusId { get; set; }
    public byte? RankId { get; set; }

    public MemberStatus Status { get; set; }
    public MemberRank Rank { get; set; }
}