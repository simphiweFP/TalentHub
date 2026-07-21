namespace TalentHub.Web.API.Models.Common;

public sealed record ApiPageResponse<T>(IReadOnlyList<T> Items, int PageNumber, int PageSize, long TotalCount)
{
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
