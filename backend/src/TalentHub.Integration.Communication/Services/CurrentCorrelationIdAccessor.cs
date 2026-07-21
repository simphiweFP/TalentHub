using System.Threading;
using TalentHub.Integration.Communication.Abstractions;

namespace TalentHub.Integration.Communication.Services;

public sealed class CurrentCorrelationIdAccessor : ICorrelationIdAccessor
{
    private static readonly AsyncLocal<string?> Current = new();

    public string? CorrelationId
    {
        get => Current.Value;
        set => Current.Value = value;
    }
}
