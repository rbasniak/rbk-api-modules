namespace rbkApiModules.Identity.Core;

public class UserToClaim
{
    protected UserToClaim()
    {

    }

    public UserToClaim(User user, Claim claim, ClaimAccessType access)
    {
        User = user;
        Claim = claim;
        Access = access;
    }

    public virtual Guid UserId { get; private set; }
    public virtual User User { get; private set; }

    public virtual Guid ClaimId { get; private set; }
    public virtual Claim Claim { get; private set; }

    public virtual ClaimAccessType Access { get; private set; }

    public void SetAccessType(ClaimAccessType mode)
    {
        Access = mode;
    }
}
