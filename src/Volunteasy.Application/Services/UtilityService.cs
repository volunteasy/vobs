using Microsoft.EntityFrameworkCore;
using Volunteasy.Core.DTOs;

namespace Volunteasy.Application.Services;

public static class UtilityService
{
    public static async Task<(IEnumerable<T>, string?)> Paginate<T>(this IQueryable<T> query, string? pageToken, Func<T, long> idGetter)
    {
        const int pageSize = 10;
        long token = 0;

        if (!string.IsNullOrEmpty(pageToken))
        {
            token = Convert.ToInt64(pageToken);
        }
        
        var list = await query
            .Where(t => idGetter(t) > token)
            .Take(pageSize)
            .ToListAsync();

        var last = list.LastOrDefault();
        return last == null ? (list, null) : (list, idGetter(last).ToString());
    }
}