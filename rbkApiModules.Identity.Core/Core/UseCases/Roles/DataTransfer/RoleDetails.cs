using System.Diagnostics;
using rbkApiModules.Commons.Core.Abstractions;

namespace rbkApiModules.Identity.Core.DataTransfer;

[DebuggerDisplay("{Name}")]
public class RoleDetails 
{
    public required Guid Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public required EntityReference[] Claims { get; set; } = [];
    public required RoleMode Mode { get; set; }
    public required RoleSource Source { get; set; }

    public static RoleDetails FromModel(Role role)
    {
        return new RoleDetails
        {
            Id = role.Id,
            Name = role.Name,
            Claims = role.Claims.Select(c => new EntityReference(c.Claim.Id, c.Claim.Description)).ToArray(),
            Mode = role.HasTenant ? RoleMode.Overwritten : RoleMode.Normal,
            Source = role.IsApplicationWide ? RoleSource.Global : RoleSource.Local
        };
    }
}

public enum RoleMode
{
    Normal = 1,
    Overwritten = 2
}

public enum RoleSource
{
    Global = 1,
    Local = 2
} 
