using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiscordBot.Database.Entities;

[Table("settings")]
public class SettingsEntity : GuildBaseEntity
{
    [Display(Name = "Задержка удаления сообщений (минуты)")]
    public byte? ExpireEventMessageDelayMinutes { get; set; }
}