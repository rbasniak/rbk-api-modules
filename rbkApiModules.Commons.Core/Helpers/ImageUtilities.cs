namespace rbkApiModules.Core.Utilities;

public static class ImageUtilities
{
    public static string ExtractExtension(string base64String)
    {
        if (string.IsNullOrWhiteSpace(base64String))
        {
            throw new ArgumentException("Base64 string cannot be null or empty.", nameof(base64String));
        }

        // Expect a data URI like: data:image/png;base64,AAAA...
        if (!base64String.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            throw new FormatException("Invalid base64 string format.");
        }

        var commaIndex = base64String.IndexOf(',');

        if (commaIndex < 0)
        {
            throw new FormatException("Invalid base64 string format.");
        }

        var header = base64String.Substring(5, commaIndex - 5); // after "data:"

        var semiIndex = header.IndexOf(';');

        var mimeType = (semiIndex >= 0 ? header[..semiIndex] : header).Trim();

        return mimeType.ToLower() switch
        {
            "image/jpeg" or "image/jpg" => "jpg",
            "image/png" => "png",
            "image/gif" => ".gif",
            "image/webp" => ".webp",
            "image/bmp" => ".bmp",
            _ => throw new NotSupportedException($"Unsupported image MIME type: {mimeType}")
        };
    }
}

