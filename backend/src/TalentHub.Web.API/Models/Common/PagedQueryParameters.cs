namespace TalentHub.Web.API.Models.Common;

public abstract record PagedQueryParameters
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public string? SearchTerm { get; init; }

    public string SortBy { get; init; } = "CreatedAtUtc";

    public SortDirection SortDirection { get; init; } = SortDirection.Descending;
}
