using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Demo1.Models.Domain.Demo;
using Demo1.Models.Domain.Folders;
using File = Demo1.Models.Domain.Folders.File;

namespace Demo1.Database.Domain;

internal class FileConfig : IEntityTypeConfiguration<File>
{
    public void Configure(EntityTypeBuilder<File> entity)
    {
        entity.HasOne(x => x.Folder)
            .WithMany(x => x.Files)
            .HasForeignKey(x => x.FolderId)
            .OnDelete(DeleteBehavior.Cascade)
            .Metadata.PrincipalToDependent
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
