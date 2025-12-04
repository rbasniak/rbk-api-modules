using System.Text.Json.Serialization;

namespace rbkApiModules.Commons.Core.Abstractions;

public record EnumReference
{
    [JsonConstructor]
    public EnumReference(int id, string value)
    {
        Id = id;
        Value = value;
    }

    public EnumReference(Enum value)
    {
        Id = Convert.ToInt32(value);
        Value = value.ToString();
    }

    public int Id { get; init; }
    public string Value { get; init; }

    public static EnumReference Empty => new EnumReference(-1, string.Empty);
}
