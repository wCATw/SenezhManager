using System.ComponentModel.DataAnnotations.Schema;

namespace DiscordBot.Database.Entities;

[Table("settings")]
public class SettingsEntity
{
    public ulong? GuildId { get; set; }
    public ulong? RegistrationChannelId { get; set; }
}