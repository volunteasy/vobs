using Microsoft.EntityFrameworkCore;

namespace Volunteasy.Application.Services;

public static class UtilityService
{
    public static async Task<(IEnumerable<T>, string?)> Paginate<T>(this IQueryable<T> query, Func<T, long> idGetter)
    {
        const int pageSize = 10;
        
        var list = await query
            .Take(pageSize + 1)
            .ToListAsync();

        if (list.Count == 0)
            return (list, null);

        return list.Count <= pageSize ? 
            (list, null) : 
            (list.Take(pageSize), idGetter(list.Last()).ToString());
    }
}