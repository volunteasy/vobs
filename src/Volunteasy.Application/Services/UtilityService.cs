using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Volunteasy.Core.Model;

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
    
    public static async Task<PaginatedList<T>> PaginateList<T>(this IQueryable<T> query, Func<T, long> idGetter)
    {
        const int pageSize = 10;
        
        var list = await query
            .Take(pageSize + 1)
            .ToListAsync();

        return list.Count <= pageSize ? 
            new PaginatedList<T>(list) : 
            new PaginatedList<T>(
                list.Take(pageSize), idGetter(list.Last()).ToString());
    }

    public static IQueryable<T> WithPageToken<T>(this IQueryable<T> query, long page) where T : IId
    {
        return query.Where(x => x.Id >= page);
    }
    
    public static IQueryable<T> WithOrganization<T>(this IQueryable<T> query, long orgId) where T : IOrganization
    {
        return query.Where(x => x.OrganizationId == orgId);
    }
    
    public static IQueryable<T> WithFilters<T>(this IQueryable<T> query, params KeyValuePair<bool, Expression<Func<T, bool>>>[] filters) where T : class
    {
        return filters.Where(queryFilter => queryFilter.Key)
            .Aggregate(query, (current, filter) => current.Where(filter.Value));
    }

    public static IQueryable<T> WithFilters<T>(this DbSet<T> set, params KeyValuePair<bool, Expression<Func<T, bool>>>[] filters) where T : class
    {
        return set.AsQueryable().WithFilters(filters);
    }
}