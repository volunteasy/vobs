using Microsoft.EntityFrameworkCore;
using Volunteasy.Core.DTOs;

namespace Volunteasy.Application.Services;

public static class UtilityService
{
    public static async Task<(IEnumerable<T>, bool)> Paginate<T>(this IQueryable<T> query, Filter f)
    {
        var list = await query.Take(f.ExceedingLimit).ToListAsync();
        return (list.Take(f.Limit), list.Count > f.Limit);
    }
}