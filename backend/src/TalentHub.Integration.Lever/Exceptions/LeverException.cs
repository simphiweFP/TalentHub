namespace TalentHub.Integration.Lever.Exceptions;

public sealed class LeverException : Exception
{
    public LeverException(string code, string message) : base(message)
    {
        Code = code;
    }

    public string Code { get; }
}
