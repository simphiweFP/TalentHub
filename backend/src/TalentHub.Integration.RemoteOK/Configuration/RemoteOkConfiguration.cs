using Microsoft.Extensions.Options;
using TalentHub.Integration.RemoteOK.Options;

namespace TalentHub.Integration.RemoteOK.Configuration;

public sealed class RemoteOkConfiguration(IOptions<RemoteOkOptions> options)
{
    public RemoteOkOptions Options { get; } = options.Value;
}
