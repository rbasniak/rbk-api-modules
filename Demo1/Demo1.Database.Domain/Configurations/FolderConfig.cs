using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Demo1.Models.Domain.Demo;
using Demo1.Models.Domain.Folders;

namespace Demo1.Database.Domain;

internal class FolderConfig : IEntityTypeConfiguration<Folder>
{
    public void Configure(EntityTypeBuilder<Folder> entity)
    {
        entity.HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict)
            .Metadata.PrincipalToDependent
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
