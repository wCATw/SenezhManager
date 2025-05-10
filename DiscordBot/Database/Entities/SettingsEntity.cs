namespace DiscordBot.Database.Entities;

public class SettingsEntity
{
    public ulong GuildId { get; set; }
    public ulong RegistrationChannelId { get; set; }
}