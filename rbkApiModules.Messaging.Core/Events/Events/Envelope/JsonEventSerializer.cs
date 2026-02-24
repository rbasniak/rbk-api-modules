using System.Text.Json;
using System.Text.Json.Serialization;

namespace rbkApiModules.Commons.Core;

public static class JsonEventSerializer
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        Converters = 
        { 
            new JsonStringEnumConverter(),
            new RuntimeTypeConverter<IDomainEvent>()
        },
        WriteIndented = false
    };

    public static JsonSerializerOptions GetOptions() => Options;

    public static string Serialize<T>(EventEnvelope<T> envelope) =>
        JsonSerializer.Serialize(envelope, Options);

    public static EventEnvelope<T> Deserialize<T>(string json) =>
        JsonSerializer.Deserialize<EventEnvelope<T>>(json, Options)!;

    public static object Deserialize(string json, Type targetType) =>
        JsonSerializer.Deserialize(json, targetType, Options)!;

    public static EnvelopeHeader DeserializeHeader(string json) =>
        JsonSerializer.Deserialize<EnvelopeHeader>(json, Options)!;
}

public class RuntimeTypeConverter<TInterface> : JsonConverter<TInterface>
{
    public override TInterface? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException("Deserialization not supported in this converter.");
    }

    public override void Write(Utf8JsonWriter writer, TInterface value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value!, value!.GetType(), options);
    }
}