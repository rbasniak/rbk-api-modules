using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Authentication;
using rbkApiModules.Demo.Models;

namespace rbkApiModules.Demo.Database
{
    public class PostConfig: IEntityTypeConfiguration<Post>
    { 
        public void Configure(EntityTypeBuilder<Post> entity)
        {
            entity.HasOne(x => x.Blog)
                .WithMany(x => x.Posts)
                .HasForeignKey(x => x.BlogId)
                .OnDelete(DeleteBehavior.Cascade)
                .Metadata.PrincipalToDependent
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
