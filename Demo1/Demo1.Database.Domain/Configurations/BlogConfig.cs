﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Demo1.Models.Domain.Demo;

namespace Demo1.Database.Domain;

internal class BlogConfig : IEntityTypeConfiguration<Blog>
{
    public void Configure(EntityTypeBuilder<Blog> entity)
    {
    }
}