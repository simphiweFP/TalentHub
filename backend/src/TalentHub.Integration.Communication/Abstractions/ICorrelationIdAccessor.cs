namespace TalentHub.Integration.Communication.Abstractions;

public interface ICorrelationIdAccessor
{
    string? CorrelationId { get; set; }
}
