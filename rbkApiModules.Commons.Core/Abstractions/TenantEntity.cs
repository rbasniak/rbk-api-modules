using System.ComponentModel.DataAnnotations;

namespace rbkApiModules.Commons.Core;

public abstract class TenantEntity : AggregateRoot
{
    private string? _tenantId;

    protected TenantEntity() : base()
    {

    }

    // TODO: Estava dando problema no SetupTenant quando o tenantId nao era nulavel. Tem que descobrir e corrigir isso
    [MaxLength(255)]
    public string? TenantId
    {
        get
        {
            return _tenantId;
        }
        set
        {
            if (value == null)
            {
                _tenantId = null;
            }
            else
            {
                _tenantId = value.ToUpper();
            }
        }
    }

    public bool HasTenant => !String.IsNullOrEmpty(TenantId);

    public bool HasNoTenant => String.IsNullOrEmpty(TenantId);
}
