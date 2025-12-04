using Microsoft.AspNetCore.Builder;

namespace rbkApiModules.Commons.Relational;

public interface IDatabaseSeeder
{
    Type DbContextType { get; }

    void ApplySeed(IApplicationBuilder builder);
}