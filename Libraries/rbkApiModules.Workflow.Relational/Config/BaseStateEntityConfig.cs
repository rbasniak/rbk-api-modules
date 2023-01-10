using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Workflow.Core;

namespace rbkApiModules.Workflow.Relational;

public class BaseStateEntityConfig
{
    protected void Configure(EntityTypeBuilder<BaseStateEntity> entity)
    {
        entity.HasOne(x => x.State)
            .WithMany()
            .HasForeignKey(x => x.StateId)
            .OnDelete(DeleteBehavior.Restrict);
    }
} 
