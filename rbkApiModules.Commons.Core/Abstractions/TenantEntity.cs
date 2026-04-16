using System.ComponentModel.DataAnnotations;

namespace rbkApiModules.Commons.Core;

/// <summary>
/// Base class for tenant-scoped entities.
/// TenantId is nullable to support hybrid entities that can be either tenant-specific
/// or application-wide (global). Examples: Role and ApiKey can belong to a specific tenant
/// or be shared across all tenants when TenantId is null.
/// </summary>
public abstract class TenantEntity : AggregateRoot
{
    private string? _tenantId;

    protected TenantEntity() : base()
    {

    }

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
