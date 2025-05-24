using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiscordBot.Database.Entities;

[Table("settings")]
public class SettingsEntity : GuildBaseEntity
{
    [Display(Name = "Канал расписания и мероприятий")]
    public ulong? ScheduleChannelId { get; set; }

    [Display(Name = "SYSTEM")] public ulong? ScheduleMessageId { get; set; }
}