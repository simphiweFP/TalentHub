namespace TalentHub.Integration.Sql.Pagination;

public sealed record PaginationRequest(int PageNumber = 1, int PageSize = 25)
{
    public int NormalizedPageNumber => PageNumber < 1 ? 1 : PageNumber;

    public int NormalizedPageSize => PageSize < 1 ? 25 : PageSize;

    public int Skip => (NormalizedPageNumber - 1) * NormalizedPageSize;
}
