using System.Data;

namespace TalentHub.Integration.Sql.Mapping;

public static class SqlDataReaderExtensions
{
    public static IReadOnlyList<T> MapToList<T>(this IDataReader reader) where T : new()
    {
        var items = new List<T>();

        while (reader.Read())
        {
            items.Add(reader.MapTo<T>());
        }

        return items;
    }

    public static T? MapSingleOrDefault<T>(this IDataReader reader) where T : new()
    {
        return reader.Read() ? reader.MapTo<T>() : default;
    }
}
