using TalentHub.Integration.Communication.Models;
using TalentHub.Integration.OpenAI.Providers;

namespace TalentHub.Tests.AI;

public sealed class AiProviderTests
{
    [Fact]
    public async Task OpenAiProvider_returns_placeholder_ai_results()
    {
        var provider = new OpenAiProvider();

        var resume = await provider.AnalyzeResumeAsync(new ResumeAnalysisRequest(
            "Experienced engineer with C# and cloud background",
            "Platform Engineer",
            RequiredSkills: ["C#", "Azure", "Testing"])).ConfigureAwait(false);

        var match = await provider.MatchJobAsync(new JobMatchRequest(
            new Job { Title = "Candidate Profile", CompanyName = "TalentHub", PublishedAtUtc = DateTimeOffset.UtcNow },
            new Job { Title = "Platform Engineer", CompanyName = "Acme", PublishedAtUtc = DateTimeOffset.UtcNow },
            ["C#", "Azure"],
            ["C#", "Docker", "Kubernetes"])).ConfigureAwait(false);

        var gap = await provider.AnalyzeSkillGapAsync(new SkillGapAnalysisRequest(["C#"], ["C#", "Azure", "Docker"], "Platform role")).ConfigureAwait(false);

        var questions = await provider.GenerateInterviewQuestionsAsync(new InterviewQuestionRequest("Platform Engineer", ["C#", "Azure"], QuestionCount: 3)).ConfigureAwait(false);

        var salary = await provider.GetSalaryInsightsAsync(new SalaryInsightRequest("Platform Engineer", "Remote", "Senior", 120000m, "USD")).ConfigureAwait(false);

        Assert.Equal("openai", provider.Name);
        Assert.NotEmpty(resume.Summary);
        Assert.NotEmpty(match.Summary);
        Assert.NotEmpty(gap.MissingSkills);
        Assert.Equal(3, questions.Questions.Count);
        Assert.Equal("USD", salary.CurrencyCode);
        Assert.True(salary.EstimatedMaximum >= salary.EstimatedMinimum);
    }
}
