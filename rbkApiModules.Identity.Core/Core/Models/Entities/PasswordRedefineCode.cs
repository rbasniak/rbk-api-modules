using System.ComponentModel.DataAnnotations;

namespace rbkApiModules.Identity.Core;

public sealed class PasswordRedefineCode
{
    private PasswordRedefineCode()
    {
    }

    public PasswordRedefineCode(DateTime creationDate)
    {
        CreationDate = creationDate;
        Hash = GenerateHash();
    }

    public DateTime? CreationDate { get; private set; }

    [MaxLength(1024)]
    public string Hash { get; private set; } = string.Empty;

    private static string GenerateHash()
    {
        string hash = string.Empty;

        for (int i = 0; i < 10; i++)
        {
            hash += $"{Guid.NewGuid():N}";
        }

        return hash;
    }
}
