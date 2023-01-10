using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Identity.Core;

[DebuggerDisplay("{Name}")]
public class Role : TenantEntity
{
    private HashSet<RoleToClaim> _claims;
    private HashSet<UserToRole> _users;

    protected Role()
    {

    }

    public Role(string name)
    {
        Name = name;

        _users = new HashSet<UserToRole>();
        _claims = new HashSet<RoleToClaim>();
    }

    public Role(string tenant, string name) : this(name)
    {
        TenantId = tenant;
    }

    [Required, MinLength(3), MaxLength(128), Description("Nome")]
    // TODO: [DefaultInput(OperationType.CreateAndUpdate)]
    public virtual string Name { get; private set; }

    public virtual bool IsApplicationWide => String.IsNullOrEmpty(TenantId);

    public virtual IEnumerable<RoleToClaim> Claims => _claims?.ToList();

    public virtual IEnumerable<UserToRole> Users => _users?.ToList();

    public RoleToClaim AddClaim(Claim claim)
    {
        if (_claims == null) throw new Exception("Não é possível manipular listas que não foram carregadas completamente do banco de dados");

        var roleToClaim = new RoleToClaim(this, claim);

        _claims.Add(roleToClaim);

        return roleToClaim;
    }

    public void Rename(string name)
    {
        Name = name;
    }

    public override string ToString()
    {
        return HasTenant ? $"[{TenantId}] {Name}" : $"[Application] {Name}";
    }
}