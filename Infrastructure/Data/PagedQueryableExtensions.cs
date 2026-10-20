using Equipment.Api.DTOs.Paging;
using Microsoft.EntityFrameworkCore;

namespace Equipment.Api.Infrastructure.Data;

public static class PagedQueryableExtensions
{
    public const int MaxPageSize = 100;

    public static async Task<PagedResult<T>> ToPagedAsync<T>(
        this IQueryable<T> query,
        PagedRequest request,
        CancellationToken cancellationToken)
    {
        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Clamp(request.PageSize, 1, MaxPageSize);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
