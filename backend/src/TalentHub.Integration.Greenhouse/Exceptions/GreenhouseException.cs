namespace TalentHub.Integration.Greenhouse.Exceptions;

public sealed class GreenhouseException : Exception
{
    public GreenhouseException(string code, string message) : base(message)
    {
        Code = code;
    }

    public string Code { get; }
}
