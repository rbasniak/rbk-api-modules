using System.ComponentModel;

namespace rbkApiModules.Identity.Core;

public enum ClaimAccessType
{
    [Description("Permitir")]
    Allow = 1,

    [Description("Bloquear")]
    Block = 0
}
