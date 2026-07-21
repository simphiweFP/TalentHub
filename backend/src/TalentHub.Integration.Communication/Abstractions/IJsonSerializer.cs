namespace TalentHub.Integration.Communication.Abstractions;

public interface IJsonSerializer
{
    string Serialize<T>(T value);

    T? Deserialize<T>(string json);
}
