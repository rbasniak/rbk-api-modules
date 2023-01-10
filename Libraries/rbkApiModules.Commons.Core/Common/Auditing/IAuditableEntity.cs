namespace rbkApiModules.Commons.Core.Auditing;

public interface IAuditableEntity
{
    string CreatedBy { get; set; }
    DateTime CreatedAt { get; set; }
    string ModifiedBy { get; set; }
    DateTime ModifiedAt { get; set; }
}

public class AuditableEntity<T> : BaseEntity, IAuditableEntity
{
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime ModifiedAt { get; set; }
}
