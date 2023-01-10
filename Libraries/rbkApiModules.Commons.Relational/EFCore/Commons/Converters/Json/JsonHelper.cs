using System.Text.Json;

namespace rbkApiModules.Commons.Relational;

internal static class JsonHelper
{
    public static T Deserialize<T>(string json) where T : class
    {
        return String.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<T>(json);
    }

    public static string Serialize<T>(T obj) where T : class
    {
        return obj == null ? null : JsonSerializer.Serialize(obj);
    }
}