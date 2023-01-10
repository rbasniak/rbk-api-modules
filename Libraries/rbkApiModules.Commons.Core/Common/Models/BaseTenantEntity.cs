using System.ComponentModel.DataAnnotations;

namespace rbkApiModules.Commons.Core;

public abstract class TenantEntity : BaseEntity, ITenantEntity
{
    protected TenantEntity(): base()
    {

    }

    [MaxLength(32)]
    public virtual string TenantId { get; protected set; }

    public virtual bool HasTenant => !String.IsNullOrEmpty(TenantId);

    public virtual bool HasNoTenant => String.IsNullOrEmpty(TenantId);
}
