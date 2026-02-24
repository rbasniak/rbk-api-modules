namespace rbkApiModules.Commons.Core;

public static class GuidHelpers
{
#if NET8_0
    // GuidV7 implementation for .NET 8 compatibility
    // Based on RFC 9562 specification for UUID version 7
    public static Guid CreateVersion7()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var randomBytes = new byte[10];
        Random.Shared.NextBytes(randomBytes);

        Span<byte> guidBytes = stackalloc byte[16];

        // Timestamp (48 bits = 6 bytes)
        guidBytes[0] = (byte)(timestamp >> 40);
        guidBytes[1] = (byte)(timestamp >> 32);
        guidBytes[2] = (byte)(timestamp >> 24);
        guidBytes[3] = (byte)(timestamp >> 16);
        guidBytes[4] = (byte)(timestamp >> 8);
        guidBytes[5] = (byte)timestamp;

        // Version and random data
        guidBytes[6] = (byte)((0x70 | (randomBytes[0] & 0x0F))); // Version 7
        guidBytes[7] = randomBytes[1];

        // Variant and random data
        guidBytes[8] = (byte)((0x80 | (randomBytes[2] & 0x3F))); // Variant 10
        guidBytes[9] = randomBytes[3];
        guidBytes[10] = randomBytes[4];
        guidBytes[11] = randomBytes[5];
        guidBytes[12] = randomBytes[6];
        guidBytes[13] = randomBytes[7];
        guidBytes[14] = randomBytes[8];
        guidBytes[15] = randomBytes[9];

        return new Guid(guidBytes);
    }
#else
    // Use native .NET 9+ implementation
    public static Guid CreateVersion7() => Guid.CreateVersion7();
#endif
}
