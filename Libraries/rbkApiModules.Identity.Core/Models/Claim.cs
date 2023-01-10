using System.ComponentModel.DataAnnotations;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Identity.Core;

public class Claim : BaseEntity
{
    private HashSet<UserToClaim> _users;
    private HashSet<RoleToClaim> _roles;

    protected Claim()
    {

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
    // TODO: [DialogData(OperationType.Create, "Chave")]
    public virtual string Identification { get; private set; }

    [Required, MinLength(5), MaxLength(255)]
    // TODO: [DialogData(OperationType.CreateAndUpdate, "Descrição")]
    public virtual string Description { get; private set; }

    public virtual bool Hidden { get; private set; }

    public virtual bool IsProtected { get; private set; }

    public virtual IEnumerable<UserToClaim> Users => _users?.ToList();

    public virtual IEnumerable<RoleToClaim> Roles => _roles?.ToList();

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
