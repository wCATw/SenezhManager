using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using DiscordBot.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Utils;

public static class ServicesHelper
{
    public static void PatchEntity<T>(T inputEntity, ref T outputEntity) where T : BaseEntity
    {
        var properties = typeof(T).GetProperties();
        foreach (var prop in properties)
        {
            var displayAttr = prop.GetCustomAttribute<DisplayAttribute>();
            if (prop.IsSystem())
                continue;

            var newValue = prop.GetValue(inputEntity);
            if (newValue != null)
                prop.SetValue(outputEntity, newValue);
        }
    }

    public static async Task ExecuteSafeAsync(ILogger logger, Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
        }
    }

    public static IQueryable<T> BuildQuery<T>(DbContext dbContext, bool asNoTracking, params Expression<Func<T, object>>[] includes) where T : BaseEntity
    {
        var query = dbContext.Set<T>().AsQueryable();

        if (asNoTracking)
            query = query.AsNoTracking();

        foreach (var include in includes) query = query.Include(include);

        return query;
    }

    public static async Task<int> GenerateNextIdForGuild<T>(DbContext dbContext, ulong guildId) where T : GuildAndIdBaseEntity
    {
        var last = await dbContext.Set<T>()
                                  .Where(e => e.GuildId == guildId)
                                  .OrderByDescending(e => e.Id)
                                  .Select(e => e.Id)
                                  .FirstOrDefaultAsync();

        return (last ?? 0) + 1;
    }
}