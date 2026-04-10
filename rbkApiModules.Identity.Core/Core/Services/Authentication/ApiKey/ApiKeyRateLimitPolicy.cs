namespace rbkApiModules.Identity.Core;

public sealed record ApiKeyRateLimitPolicy(int RequestsPerMinute, int BurstLimit);
