using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Workflow.Core;

namespace rbkApiModules.Workflow.Relational;

public class QueryDefinitionToStateConfig
{
    protected void Configure(EntityTypeBuilder<QueryDefinitionToState> entity)
    {
        entity.HasKey(t => new { t.QueryId, t.StateId});

        entity.HasOne(x => x.State)
            .WithMany()
            .HasForeignKey(x => x.StateId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(x => x.Query)
            .WithMany(x => x.FilteringStates)
            .HasForeignKey(x => x.QueryId)
            .OnDelete(DeleteBehavior.Restrict)
            .Metadata.PrincipalToDependent
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    } 
} 
