using System.Net.Http;

namespace TalentHub.Integration.Communication.Abstractions;

public interface ICommunicationHttpClientFactory
{
    HttpClient CreateClient(string name);
}
