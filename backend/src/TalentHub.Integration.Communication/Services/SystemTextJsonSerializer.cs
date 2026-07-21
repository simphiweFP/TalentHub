using System.Text.Json;
using System.Text.Json.Serialization;
using TalentHub.Integration.Communication.Abstractions;

namespace TalentHub.Integration.Communication.Services;

public sealed class SystemTextJsonSerializer : IJsonSerializer
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    public string Serialize<T>(T value) => JsonSerializer.Serialize(value, Options);

    public T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, Options);
}
