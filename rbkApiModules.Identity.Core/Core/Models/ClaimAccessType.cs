using System.ComponentModel;

namespace rbkApiModules.Identity.Core;

public enum ClaimAccessType
{
    [Description("Allow")]
    Allow = 1,

    [Description("Block")]
    Block = 0
}
