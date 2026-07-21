namespace TalentHub.Integration.Sql.Common;

public static class SqlSortHelper
{
    public static string BuildOrderBy(string sortBy, SortDirection direction, IReadOnlyDictionary<string, string> sortableColumns, string defaultColumn)
    {
        if (!sortableColumns.TryGetValue(sortBy, out var column))
        {
            column = defaultColumn;
        }

        var sortDirection = direction == SortDirection.Ascending ? "ASC" : "DESC";
        return $"ORDER BY {column} {sortDirection}";
    }
}
