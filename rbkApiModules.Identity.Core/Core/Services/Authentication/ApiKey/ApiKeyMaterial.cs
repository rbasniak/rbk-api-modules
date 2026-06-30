using System.Security.Cryptography;
using System.Text;

namespace rbkApiModules.Identity.Core;

public static class ApiKeyMaterial
{
    public const string DefaultPublicPrefix = "rbk_live_";

    /// <summary>Generates a new API key: raw value (returned once), public prefix segment, and SHA-256 hash (hex, lowercase) of the raw key.</summary>
    public static (string RawKey, string KeyPrefix, string KeyHash) Generate(string publicPrefix = DefaultPublicPrefix)
    {
        var secret = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();
        publicPrefix ??= string.Empty;
        var rawKey = string.IsNullOrEmpty(publicPrefix) ? secret : $"{publicPrefix}{secret}";
        var keyHash = HashRawKey(rawKey);
        return (rawKey, publicPrefix, keyHash);
    }

    /// <summary>Builds a raw API key from a secret segment and optional prefix ({prefix}_{key} when prefix is set, otherwise just key).</summary>
    public static (string RawKey, string KeyPrefix, string KeyHash) Compose(string key, string prefix = "")
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("API key secret segment is empty.", nameof(key));
        }

        prefix ??= string.Empty;
        var rawKey = string.IsNullOrEmpty(prefix) ? key : $"{prefix}_{key}";
        var keyHash = HashRawKey(rawKey);
        return (rawKey, prefix, keyHash);
    }

    public static string HashRawKey(string rawKey)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawKey));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public static bool TryParseStoredPrefix(string rawKey, string expectedPrefix, out string error)
    {
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(rawKey))
        {
            error = "API key is empty.";
            return false;
        }

        if (!rawKey.StartsWith(expectedPrefix, StringComparison.Ordinal))
        {
            error = "API key format is invalid.";
            return false;
        }

        if (rawKey.Length <= expectedPrefix.Length)
        {
            error = "API key format is invalid.";
            return false;
        }

        return true;
    }
}
