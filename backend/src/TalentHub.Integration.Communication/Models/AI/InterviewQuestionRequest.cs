namespace TalentHub.Integration.Communication.Models;

public sealed record InterviewQuestionRequest(
    string RoleTitle,
    IReadOnlyCollection<string> Skills,
    string? JobDescription = null,
    int QuestionCount = 5);
