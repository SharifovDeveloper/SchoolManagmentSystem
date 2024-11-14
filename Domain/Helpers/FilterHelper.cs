using Microsoft.EntityFrameworkCore;

namespace Domain.Helpers;

public static class FilterHelper
{
    public static IQueryable<T> ApplyDateFilters<T>(
        this IQueryable<T> query,
        DateTime? createdDateFrom,
        DateTime? createdDateTo,
        DateTime? lastUpdatedDateFrom,
        DateTime? lastUpdatedDateTo) where T : class
    {
        if (createdDateFrom.HasValue)
        {
            query = query.Where(x => EF.Property<DateTime>(x, "CreatedDate") >= createdDateFrom.Value.Date);
        }

        if (createdDateTo.HasValue)
        {
            query = query.Where(x => EF.Property<DateTime>(x, "CreatedDate") <= createdDateTo.Value.Date);
        }

        if (lastUpdatedDateFrom.HasValue)
        {
            query = query.Where(x => EF.Property<DateTime>(x, "LastUpdatedDate") >= lastUpdatedDateFrom.Value.Date);
        }

        if (lastUpdatedDateTo.HasValue)
        {
            query = query.Where(x => EF.Property<DateTime>(x, "LastUpdatedDate") <= lastUpdatedDateTo.Value.Date);
        }

        return query;
    }

    public static IQueryable<T> ApplyIsDeletedFilter<T>(
        this IQueryable<T> query, bool? isDeleted) where T : class
    {
        if (isDeleted.HasValue)
        {
            query = query.Where(x => EF.Property<bool>(x, "IsDeleted") == isDeleted.Value);
        }

        return query;
    }
}
