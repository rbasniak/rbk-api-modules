﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Demo1.Models.Read;

namespace Demo1.Database.Read.Config;

internal class PerformanceTest1Config : IEntityTypeConfiguration<PerformanceTest1>
{
    public void Configure(EntityTypeBuilder<PerformanceTest1> entity)
    {
        
    }
}