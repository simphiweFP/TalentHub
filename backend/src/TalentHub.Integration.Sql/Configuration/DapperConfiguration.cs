using Dapper;

namespace TalentHub.Integration.Sql.Configuration;

public static class DapperConfiguration
{
    private static bool _configured;

    public static void Configure()
    {
        if (_configured)
        {
            return;
        }

        DefaultTypeMap.MatchNamesWithUnderscores = true;
        _configured = true;
    }
}
