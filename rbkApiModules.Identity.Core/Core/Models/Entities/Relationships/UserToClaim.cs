namespace rbkApiModules.Identity.Core;

public sealed class UserToClaim
{
    private UserToClaim()
    {

    }

    public UserToClaim(User user, Claim claim, ClaimAccessType access)
    {
        User = user;
        Claim = claim;
        Access = access;
    }

    public Guid UserId { get; private set; }
    public User User { get; private set; }

    public Guid ClaimId { get; private set; }
    public Claim Claim { get; private set; }

    public ClaimAccessType Access { get; private set; }

    public void SetAccessType(ClaimAccessType mode)
    {
        Access = mode;
    }
}
