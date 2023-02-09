using Demo2.Domain.Events.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo2.Domain.Events.MyImplementation;

internal class DomainEventDataObjectConfig : IEntityTypeConfiguration<DomainEventDataObject>
{
    public void Configure(EntityTypeBuilder<DomainEventDataObject> builder)
    {
        builder.ToTable("DomainEvents");
    }
}
