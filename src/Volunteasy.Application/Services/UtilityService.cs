using System.Linq.Expressions;
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

        return list.Count <= pageSize ? 
            (list, null) : 
            (list.Take(pageSize), idGetter(list.Last()).ToString());
    }
    
    public static IQueryable<T> WithFilters<T>(this IQueryable<T> query, params KeyValuePair<bool, Expression<Func<T, bool>>>[] filters)
    {
        return filters.Where(queryFilter => queryFilter.Key)
            .Aggregate(query, (current, filter) => current.Where(filter.Value));
    }

    public static IQueryable<T> WithFilters<T>(this DbSet<T> set, params KeyValuePair<bool, Expression<Func<T, bool>>>[] filters) where T : class
    {
        return set.AsQueryable().WithFilters(filters);
    }
}