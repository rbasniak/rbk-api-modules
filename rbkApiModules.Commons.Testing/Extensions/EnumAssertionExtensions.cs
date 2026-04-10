using rbkApiModules.Commons.Core.Abstractions;
using Shouldly;

namespace rbkApiModules.Testing.Core;

public static class EnumAssertionExtensions
{
    public static void ShouldBeEquivalentTo(this Enum actual, EnumReference expected, string? message = null)
    {
        var expectedMessage = message ?? $"Expected enum to be {expected.Value} (Id: {expected.Id}) but was {actual} (Id: {Convert.ToInt32(actual)})";
        
        Convert.ToInt32(actual).ShouldBe(expected.Id, expectedMessage);
        actual.ToString().ShouldBe(expected.Value, expectedMessage);
    }

    public static void ShouldBeEquivalentTo(this EnumReference actual, Enum expected, string? message = null)
    {
        var expectedMessage = message ?? $"Expected enum reference to be {expected} (Id: {Convert.ToInt32(expected)}) but was {actual.Value} (Id: {actual.Id})";
        
        actual.Id.ShouldBe(Convert.ToInt32(expected), expectedMessage);
        actual.Value.ShouldBe(expected.ToString(), expectedMessage);
    }
}
