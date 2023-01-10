using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Workflow.Core;

namespace rbkApiModules.Workflow.Relational;

public class QueryDefinitionToGroupConfig
{
    protected void Configure(EntityTypeBuilder<QueryDefinitionToGroup> entity)
    {
        entity.HasKey(t => new { t.QueryId, t.GroupId });

        entity.HasOne(x => x.Group)
            .WithMany(x => x.Queries)
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.Cascade)
            .Metadata.PrincipalToDependent
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        entity.HasOne(x => x.Query)
            .WithMany(x => x.Groups)
            .HasForeignKey(x => x.QueryId)
            .OnDelete(DeleteBehavior.Cascade)
            .Metadata.PrincipalToDependent
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    } 
} 
