namespace TalentHub.Integration.Communication.Models;

public sealed record SalaryInsightRequest(
    string RoleTitle,
    string? Location = null,
    string? Seniority = null,
    decimal? CurrentSalary = null,
    string? CurrencyCode = null);
