using System.ComponentModel.DataAnnotations;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Identity.Core;

public class PasswordRedefineCode
{
    protected PasswordRedefineCode()
    {
    }

    public PasswordRedefineCode(DateTime creationDate)
    {
        CreationDate = creationDate;
        Hash = GenerateHash();
    }

    public virtual DateTime? CreationDate { get; private set; }

    [MaxLength(1024)]
    public virtual string Hash { get; private set; }

    private static string GenerateHash()
    {
        string hash = string.Empty;

        for (int i = 0; i < 10; i++)
        {
            hash += Guid.NewGuid().EncodeId();
        }

        return hash;
    }
}
