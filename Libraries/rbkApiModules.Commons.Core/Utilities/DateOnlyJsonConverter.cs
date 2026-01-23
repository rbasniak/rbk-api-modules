using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace rbkApiModules.Commons.Core;

public sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private const string Format = "yyyy-MM-dd";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new JsonException("Cannot convert null or empty string to DateOnly.");
        }

        if (DateOnly.TryParseExact(value, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return date;
        }

        // Accept ISO 8601 datetime (e.g. "2026-01-01T00:00:00.000Z") and use the date part in UTC
        if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dto))
        {
            return DateOnly.FromDateTime(dto.UtcDateTime);
        }

        throw new JsonException($"Unable to parse '{value}' as DateOnly. Expected format: {Format} or ISO 8601 datetime.");
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
}

public sealed class NullableDateOnlyJsonConverter : JsonConverter<DateOnly?>
{
    private const string Format = "yyyy-MM-dd";

    public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (DateOnly.TryParseExact(value, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return date;
        }

        // Accept ISO 8601 datetime (e.g. "2026-01-01T00:00:00.000Z") and use the date part in UTC
        if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dto))
        {
            return DateOnly.FromDateTime(dto.UtcDateTime);
        }

        throw new JsonException($"Unable to parse '{value}' as DateOnly. Expected format: {Format} or ISO 8601 datetime.");
    }

    public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
        => writer.WriteStringValue(value?.ToString(Format, CultureInfo.InvariantCulture));
}