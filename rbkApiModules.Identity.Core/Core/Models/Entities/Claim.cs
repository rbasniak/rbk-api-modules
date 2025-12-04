using System.ComponentModel.DataAnnotations;

namespace rbkApiModules.Identity.Core;

public sealed class Claim : BaseEntity
{
    private HashSet<UserToClaim> _users;
    private HashSet<RoleToClaim> _roles;

    private Claim()
    {
        _users = default!;
        _roles = default!;
    }

    public Claim(string identification, string description)
    {
        Description = description;
        Identification = identification.ToUpper();
        IsProtected = false;

        _users = new HashSet<UserToClaim>();
        _roles = new HashSet<RoleToClaim>();
    }

    [Required, MinLength(5), MaxLength(128)]
    public string Identification { get; private set; } = string.Empty;

    [Required, MinLength(5), MaxLength(255)]
    public string Description { get; private set; } = string.Empty;

    public bool Hidden { get; private set; }

    public bool IsProtected { get; private set; }

    public IEnumerable<UserToClaim> Users => _users?.AsReadOnly()!;

    public IEnumerable<RoleToClaim> Roles => _roles?.AsReadOnly()!;

    public void SetDescription(string description)
    {
        Description = description;
    }

    public void Protect()
    {
        IsProtected = true;
    }
    public void Unprotect()
    {
        IsProtected = false;
    }

    public override string ToString()
    {
        return $"[{Identification}] {Description} | IsProtected={IsProtected}";
    }

    public void Hide()
    {
        Hidden = true;
    }
}
