namespace TalentHub.Integration.RemoteOK.Exceptions;

public sealed class RemoteOkException : Exception
{
    public RemoteOkException(string code, string message) : base(message)
    {
        Code = code;
    }

    public string Code { get; }
}
