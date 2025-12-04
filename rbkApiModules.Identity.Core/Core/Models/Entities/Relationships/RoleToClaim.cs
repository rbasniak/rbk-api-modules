namespace rbkApiModules.Identity.Core;

public sealed class RoleToClaim
{
    private RoleToClaim()
    {
        Role = default!;
        Claim = default!;
    }

    public RoleToClaim(Role role, Claim claim)
    {
        Role = role;
        Claim = claim;
    }

    public Guid RoleId { get; private set; }
    public Role Role { get; private set; }

    public Guid ClaimId { get; private set; }
    public Claim Claim { get; private set; }
}