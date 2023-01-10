namespace rbkApiModules.Commons.Core;

public static class Base32GuidEncoder
{
    private static readonly string _alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789".ToLower();

    public static string ToString(this Guid guid)
    {
        if (guid == Guid.Empty) return null;

        var bytes = guid.ToByteArray();

        string output = "";

        for (int bitIndex = 0; bitIndex < bytes.Length * 8; bitIndex += 5)
        {
            int dualbyte = bytes[bitIndex / 8] << 8;

            if (bitIndex / 8 + 1 < bytes.Length)
            {
                dualbyte |= bytes[bitIndex / 8 + 1];
            }

            dualbyte = 0x1f & (dualbyte >> (16 - bitIndex % 8 - 5));

            output += _alphabet[dualbyte];
        }

        return output;
    }

    public static string ToString(this Guid? guid)
    {
        if (guid == null) return null;

        return Base32GuidEncoder.ToString(guid.Value);
    }

    public static Guid Parse(string input)
    {
        if (String.IsNullOrEmpty(input))
        {
            return Guid.Empty;
        }

        input = input.ToLower();

        List<byte> output = new List<byte>();

        char[] bytes = input.ToCharArray();

        var compensation = input.Length % 5 != 0 ? 1 : 0;

        for (int bitIndex = 0; bitIndex < input.Length * 5 - compensation; bitIndex += 8)
        {
            int dualbyte = _alphabet.IndexOf(bytes[bitIndex / 5]) << 10;

            if (bitIndex / 5 + 1 < bytes.Length)
            {
                dualbyte |= _alphabet.IndexOf(bytes[bitIndex / 5 + 1]) << 5;
            }

            if (bitIndex / 5 + 2 < bytes.Length)
            {
                dualbyte |= _alphabet.IndexOf(bytes[bitIndex / 5 + 2]);
            }

            dualbyte = 0xff & (dualbyte >> (15 - bitIndex % 5 - 8));
            output.Add((byte)(dualbyte));
        }

        if (output.Count == 17)
        {
            output.RemoveAt(output.Count - 1);
        }

        if (output.Count != 16)
        {
            throw new ArgumentException("Invalid shortened guid");
        }

        return new Guid(output.ToArray());
    }

    public static Guid ToGuid(this string id)
    {
        return Parse(id);
    }

    public static string EncodeId(this Guid id)
    {
        return Base32GuidEncoder.ToString(id);
    }
}