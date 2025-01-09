
namespace rbkApiModules.Commons.Core;

public sealed record ErrorMessage: IEquatable<ErrorMessage>
{
    public ErrorMessage(Exception exception, string code, string message)
    {
        Exception = exception.ToBetterString();
        Code = code;
        Message = message;
    }

    public string Exception { get; }
    public string Code { get; }
    public string Message { get; }

    public bool Equals(ErrorMessage other) =>
        Code != null && other.Code != null 
            ? Code.ToLower() == other.Code.ToLower() && Message.ToLower() == other.Message.ToLower()
            : Message.ToLower() == other.Message.ToLower();

    public override int GetHashCode() =>
        Code != null ? Code.GetHashCode() ^ Message.GetHashCode() : Message.GetHashCode();
}
