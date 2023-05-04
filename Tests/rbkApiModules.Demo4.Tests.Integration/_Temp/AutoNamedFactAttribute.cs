using System.Runtime.CompilerServices;

namespace rbkApiModules.Testing.Core;

public sealed class FriendlyNamedFactAttribute : FactAttribute
{
    public FriendlyNamedFactAttribute(string code, [CallerMemberName] string memberName = null)
    {
        // DisplayName = memberName.Replace("_", " ");
        var parts = memberName.Split('_');

        for (int i = 1; i < parts.Length; i++)
        {
            if (parts[i].Length > 1)
            {
                parts[i] = parts[i].ToUpper() == parts[i] ? parts[i] : parts[i].ToLower();
            }
        }

        DisplayName = $"{code}: " + String.Join(' ', parts);
    }
}

public sealed class FriendlyNamedTheoryAttribute : TheoryAttribute
{
    public FriendlyNamedTheoryAttribute(string code, [CallerMemberName] string memberName = null)
    {
        // DisplayName = memberName.Replace("_", " ");
        var parts = memberName.Split('_');

        for (int i = 1; i < parts.Length; i++)
        {
            if (parts[i].Length > 1)
            {
                parts[i] = parts[i].ToUpper() == parts[i] ? parts[i] : parts[i].ToLower();
            }
        }

        DisplayName = $"{code}: " + String.Join(' ', parts);
    }
}