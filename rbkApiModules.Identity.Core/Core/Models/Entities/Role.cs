using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace rbkApiModules.Identity.Core;

[DebuggerDisplay("{Name}")]
public sealed class Role : TenantEntity
{
    private HashSet<RoleToClaim> _claims;
    private HashSet<UserToRole> _users;
    
    private bool? _isOverwritten = false;

    private Role()
    {
        _claims = default!;
        _users = default!;
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
    public string Name { get; private set; } = string.Empty;

    public bool IsApplicationWide => String.IsNullOrEmpty(TenantId);

    public IEnumerable<RoleToClaim> Claims => _claims?.AsReadOnly()!;

    public IEnumerable<UserToRole> Users => _users?.AsReadOnly()!;

    public RoleToClaim AddClaim(Claim claim)
    {
        if (_claims == null) throw new Exception("Cannot change lists not loaded from database");

        var roleToClaim = new RoleToClaim(this, claim);

        _claims.Add(roleToClaim);

        return roleToClaim;
    }

    public void Rename(string name)
    {
        Name = name;
    }

    public void SetMode(bool isOverwritten)
    {
        _isOverwritten = isOverwritten;
    }

    public bool? GetMode()
    {
        return _isOverwritten;
    }

    public override string ToString()
    {
        return HasTenant ? $"[{TenantId}] {Name}" : $"[Application] {Name}";
    }
}