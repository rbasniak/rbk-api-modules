
namespace rbkApiModules.Commons.Core;

public sealed record ErrorMessage(string Code, string Message): IEquatable<ErrorMessage>
{
    public bool Equals(ErrorMessage other) =>
        Code != null && other.Code != null 
            ? Code.ToLower() == other.Code.ToLower() && Message.ToLower() == other.Message.ToLower()
            : Message.ToLower() == other.Message.ToLower();

    public override int GetHashCode() =>
        Code != null ? Code.GetHashCode() ^ Message.GetHashCode() : Message.GetHashCode();
}
