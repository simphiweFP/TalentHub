using System.ComponentModel;
using System.Data;
using System.Reflection;
using Dapper;

namespace TalentHub.Integration.Sql.Mapping;

public static class MappingExtensions
{
    public static IDictionary<string, object?> ToDictionary<T>(this T value, params string[] ignoredProperties)
    {
        if (value is null)
        {
            return new Dictionary<string, object?>();
        }

        var ignored = new HashSet<string>(ignoredProperties ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);
        var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        foreach (var property in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!property.CanRead || ignored.Contains(property.Name))
            {
                continue;
            }

            result[property.Name] = property.GetValue(value);
        }

        return result;
    }

    public static DynamicParameters ToDynamicParameters<T>(this T value, params string[] ignoredProperties)
    {
        var parameters = new DynamicParameters();

        foreach (var pair in value.ToDictionary(ignoredProperties))
        {
            parameters.Add(pair.Key, pair.Value);
        }

        return parameters;
    }

    public static T MapTo<T>(this IDataRecord record) where T : new()
    {
        var instance = new T();
        var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.CanWrite)
            .ToDictionary(property => property.Name, StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < record.FieldCount; i++)
        {
            var fieldName = record.GetName(i);
            if (!properties.TryGetValue(fieldName, out var property))
            {
                continue;
            }

            var value = record.IsDBNull(i) ? null : record.GetValue(i);
            if (value is null)
            {
                continue;
            }

            var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            if (targetType.IsEnum)
            {
                property.SetValue(instance, Enum.ToObject(targetType, value));
                continue;
            }

            var converter = TypeDescriptor.GetConverter(targetType);
            if (converter.CanConvertFrom(value.GetType()))
            {
                property.SetValue(instance, converter.ConvertFrom(value));
                continue;
            }

            property.SetValue(instance, Convert.ChangeType(value, targetType));
        }

        return instance;
    }
}
