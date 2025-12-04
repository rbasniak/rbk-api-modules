namespace rbkApiModules.Commons.Core.Abstractions;

public record EntityReference(Guid Id, string Name);

public record EntityReference<T>(T Id, string Name);