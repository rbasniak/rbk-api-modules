using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Demo.Models;

namespace rbkApiModules.Demo.Database
{
    public class EditorConfig : IEntityTypeConfiguration<Editor>
    { 
        public void Configure(EntityTypeBuilder<Editor> entity)
        {
            entity.HasOne(x => x.Blog)
                .WithMany(x => x.Editors)
                .HasForeignKey(x => x.BlogId)
                .OnDelete(DeleteBehavior.Cascade)
                .Metadata.PrincipalToDependent
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
