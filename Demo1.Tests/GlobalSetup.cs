// Here you could define global logic that would affect all tests

// You can use attributes at the assembly level to apply to all tests in the assembly
using TUnit.Core.Interfaces;

[assembly: System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]

[assembly: ParallelLimiter<MyParallelLimit>]

public record MyParallelLimit : IParallelLimit
{
    public int Limit => 1;
}