namespace DiscordBot.Database.Entities;

public abstract class GuildBaseEntity
{
    public ulong? GuildId { get; set; }
}

public abstract class GuildAndIdBaseEntity : GuildBaseEntity
{
    public ulong? Id { get; set; }
}

public abstract class GuildAndUserBaseEntity : GuildBaseEntity
{
    public ulong? UserId { get; set; }
}