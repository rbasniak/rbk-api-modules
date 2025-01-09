using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace rbkApiModules.Commons.Core;

public class SafeException : ApplicationException, ISerializable
{
    private SafeException() : base()
    {
    }

    public SafeException(string message, bool shouldBeLogged = false) : base(message)
    {
        ShouldBeLogged = shouldBeLogged;
    }

    public SafeException(string message, Exception innerException, bool shouldBeLogged = false) : base(message, innerException)
    {
        ShouldBeLogged = shouldBeLogged;
    }

    public bool ShouldBeLogged { get; set; } 
}
