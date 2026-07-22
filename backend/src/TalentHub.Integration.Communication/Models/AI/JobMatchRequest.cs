using TalentHub.Integration.Communication.Models;

namespace TalentHub.Integration.Communication.Models;

public sealed record JobMatchRequest(
    Job CandidateProfile,
    Job TargetJob,
    IReadOnlyCollection<string>? CandidateSkills = null,
    IReadOnlyCollection<string>? TargetSkills = null);
