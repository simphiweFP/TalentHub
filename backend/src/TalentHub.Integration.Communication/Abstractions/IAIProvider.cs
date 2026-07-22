using TalentHub.Integration.Communication.Models;

namespace TalentHub.Integration.Communication.Abstractions;

public interface IAIProvider
{
    string Name { get; }

    Task<ResumeAnalysisResult> AnalyzeResumeAsync(ResumeAnalysisRequest request, CancellationToken cancellationToken = default);

    Task<JobMatchResult> MatchJobAsync(JobMatchRequest request, CancellationToken cancellationToken = default);

    Task<SkillGapAnalysisResult> AnalyzeSkillGapAsync(SkillGapAnalysisRequest request, CancellationToken cancellationToken = default);

    Task<InterviewQuestionResult> GenerateInterviewQuestionsAsync(InterviewQuestionRequest request, CancellationToken cancellationToken = default);

    Task<SalaryInsightResult> GetSalaryInsightsAsync(SalaryInsightRequest request, CancellationToken cancellationToken = default);
}
