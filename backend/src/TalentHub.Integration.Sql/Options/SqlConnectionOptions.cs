namespace TalentHub.Integration.Sql.Options;

public sealed class SqlConnectionOptions
{
    public const string SectionName = "Sql";

    public string ConnectionStringName { get; set; } = "DefaultConnection";

    public string? ConnectionString { get; set; }

    public int CommandTimeoutSeconds { get; set; } = 30;
}
