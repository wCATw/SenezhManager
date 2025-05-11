using System.ComponentModel.DataAnnotations.Schema;

namespace DiscordBot.Database.Entities;

[Table("statuses")]
public class MemberStatusEntity
{
    public ulong? GuildId { get; set; }
    public string? Name { get; set; }
}