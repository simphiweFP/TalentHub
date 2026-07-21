namespace TalentHub.Integration.Sql.Constants;

public static class SqlConstants
{
    public const string DefaultSchema = "dbo";

    public const string ParameterPrefix = "@";

    public const int DefaultCommandTimeoutSeconds = 30;

    public static class Pagination
    {
        public const int DefaultPageNumber = 1;
        public const int DefaultPageSize = 25;
        public const int MaximumPageSize = 500;
    }
}
