namespace rbkApiModules.Identity.Core;

public class UserToRole
{
    protected UserToRole()
    {

    }

    public UserToRole(User user, Role role)
    {
        User = user;
        Role = role;
    }

    public virtual Guid UserId { get; private set; }
    public virtual User User { get; private set; }

    public virtual Guid RoleId { get; private set; }
    public virtual Role Role { get; private set; }

    public override string ToString()
    {
        var user = User != null ? User.ToString() : UserId.ToString();
        var role = Role != null ? Role.ToString() : RoleId.ToString();

        return $"{user} x {role}";
    }
}
