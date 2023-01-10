using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Demo1.Models.Domain.Demo;

namespace Demo1.Database.Domain;

internal class PostConfig : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> entity)
    {
        entity.HasOne(x => x.Blog)
            .WithMany(x => x.Posts)
            .HasForeignKey(x => x.BlogId)
            .OnDelete(DeleteBehavior.Cascade)
            .Metadata.PrincipalToDependent
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        entity.HasOne(x => x.Author)
            .WithMany(x => x.Posts)
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Cascade)
            .Metadata.PrincipalToDependent
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
