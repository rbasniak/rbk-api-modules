using GCAB.Models;
using GCAB.Models.States;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Demo2.Database.Config.Relational
{
    public class StateConfig : IEntityTypeConfiguration<State>
    {
        [ExcludeFromCodeCoverage]
        public void Configure(EntityTypeBuilder<State> entity)
        {
            entity.ToTable("States");

            entity.Property(c => c.Name)
                .IsRequired();
        }
    }
}
