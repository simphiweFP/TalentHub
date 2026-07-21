namespace TalentHub.Integration.Sql.Pagination;

public static class PaginationExtensions
{
    public static PaginationRequest Normalize(this PaginationRequest request)
        => new(request.NormalizedPageNumber, request.NormalizedPageSize);

    public static PagedResult<T> ToPagedResult<T>(this IEnumerable<T> items, PaginationRequest request, long totalCount)
        => new(items.ToList(), request.NormalizedPageNumber, request.NormalizedPageSize, totalCount);
}
