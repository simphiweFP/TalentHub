using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.Communication.Models;

namespace TalentHub.Integration.OpenAI.Providers;

public sealed class OpenAiProvider : IAIProvider
{
    public string Name => "openai";

    public Task<ResumeAnalysisResult> AnalyzeResumeAsync(ResumeAnalysisRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var requiredSkills = NormalizeSkills(request.RequiredSkills);
        var resumeText = request.ResumeText ?? string.Empty;
        var matchedSkills = requiredSkills.Where(skill => resumeText.Contains(skill, StringComparison.OrdinalIgnoreCase)).ToArray();
        var gaps = requiredSkills.Except(matchedSkills, StringComparer.OrdinalIgnoreCase).ToArray();

        return Task.FromResult(new ResumeAnalysisResult
        {
            Summary = $"Placeholder resume analysis for {request.TargetRole ?? "the target role"}.",
            MatchScore = requiredSkills.Count == 0 ? 75 : (int)Math.Round((matchedSkills.Length / (double)requiredSkills.Count) * 100),
            Strengths = matchedSkills.Length == 0 ? ["Clear experience summary"] : matchedSkills,
            Gaps = gaps,
            RecommendedSkills = gaps.Take(5).ToArray()
        });
    }

    public Task<JobMatchResult> MatchJobAsync(JobMatchRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var candidateSkills = NormalizeSkills(request.CandidateSkills);
        var targetSkills = NormalizeSkills(request.TargetSkills);
        var overlap = candidateSkills.Intersect(targetSkills, StringComparer.OrdinalIgnoreCase).ToArray();
        var concerns = targetSkills.Except(overlap, StringComparer.OrdinalIgnoreCase).ToArray();
        var score = targetSkills.Count == 0 ? 70 : (int)Math.Round((overlap.Length / (double)targetSkills.Count) * 100);

        return Task.FromResult(new JobMatchResult
        {
            Summary = $"Placeholder match between {request.CandidateProfile.Title} and {request.TargetJob.Title}.",
            MatchScore = score,
            MatchingFactors = overlap,
            Concerns = concerns,
            SuggestedActions = concerns.Take(3).Select(skill => $"Strengthen {skill}").ToArray()
        });
    }

    public Task<SkillGapAnalysisResult> AnalyzeSkillGapAsync(SkillGapAnalysisRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var currentSkills = NormalizeSkills(request.CurrentSkills);
        var targetSkills = NormalizeSkills(request.TargetSkills);
        var matched = currentSkills.Intersect(targetSkills, StringComparer.OrdinalIgnoreCase).ToArray();
        var missing = targetSkills.Except(matched, StringComparer.OrdinalIgnoreCase).ToArray();

        return Task.FromResult(new SkillGapAnalysisResult
        {
            Summary = $"Placeholder skill gap analysis{(string.IsNullOrWhiteSpace(request.Context) ? string.Empty : $" for {request.Context}") }.",
            MatchedSkills = matched,
            MissingSkills = missing,
            RecommendedLearningPaths = missing.Take(5).Select(skill => $"Learn {skill}").ToArray()
        });
    }

    public Task<InterviewQuestionResult> GenerateInterviewQuestionsAsync(InterviewQuestionRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var count = Math.Max(1, request.QuestionCount);
        var skills = NormalizeSkills(request.Skills).Take(5).ToArray();
        var questions = Enumerable.Range(1, count)
            .Select(index => skills.Length == 0
                ? $"What experience do you have for the {request.RoleTitle} role?"
                : $"How have you used {skills[(index - 1) % skills.Length]} in a {request.RoleTitle} context?")
            .ToArray();

        return Task.FromResult(new InterviewQuestionResult
        {
            Summary = $"Placeholder interview questions for {request.RoleTitle}.",
            Questions = questions
        });
    }

    public Task<SalaryInsightResult> GetSalaryInsightsAsync(SalaryInsightRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var roleFactor = Math.Clamp(request.RoleTitle.Length * 1000, 40000, 140000);
        var seniorityMultiplier = string.Equals(request.Seniority, "Senior", StringComparison.OrdinalIgnoreCase) ? 1.25m : 1.0m;
        var minimum = Math.Round(roleFactor * 0.85m * seniorityMultiplier, 0);
        var maximum = Math.Round(roleFactor * 1.15m * seniorityMultiplier, 0);

        return Task.FromResult(new SalaryInsightResult
        {
            Summary = $"Placeholder salary insight for {request.RoleTitle}.",
            CurrencyCode = string.IsNullOrWhiteSpace(request.CurrencyCode) ? "USD" : request.CurrencyCode,
            EstimatedMinimum = minimum,
            EstimatedMaximum = maximum,
            Factors =
            [
                request.Location is null ? "Location not specified" : $"Location: {request.Location}",
                request.Seniority is null ? "Seniority not specified" : $"Seniority: {request.Seniority}",
                request.CurrentSalary is null ? "No current salary provided" : $"Current salary: {request.CurrentSalary:0}"
            ]
        });
    }

    private static IReadOnlyList<string> NormalizeSkills(IReadOnlyCollection<string>? skills)
        => skills is null
            ? []
            : skills.Where(skill => !string.IsNullOrWhiteSpace(skill))
                .Select(skill => skill.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
}
