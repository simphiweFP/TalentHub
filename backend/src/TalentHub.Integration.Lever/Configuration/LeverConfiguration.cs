using Microsoft.Extensions.Options;
using TalentHub.Integration.Lever.Options;

namespace TalentHub.Integration.Lever.Configuration;

public sealed class LeverConfiguration(IOptions<LeverOptions> options)
{
    public LeverOptions Options { get; } = options.Value;
}
