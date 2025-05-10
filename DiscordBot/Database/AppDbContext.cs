using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    
}