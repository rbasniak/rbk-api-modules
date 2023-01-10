
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Faqs.Core;

namespace rbkApiModules.Faqs.Relational;

public class FaqConfig : IEntityTypeConfiguration<Faq>
{
    public void Configure(EntityTypeBuilder<Faq> entity)
    {
        entity.ToTable("Faqs");
    }
}
