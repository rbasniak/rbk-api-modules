namespace rbkApiModules.Identity.Core;

public sealed class ApiKeyUsageByDay
{
    private ApiKeyUsageByDay()
    {
    }

    public ApiKeyUsageByDay(Guid apiKeyId, DateOnly date, int count)
    {
        ApiKeyId = apiKeyId;
        Date = date;
        Count = count;
    }

    public Guid ApiKeyId { get; private set; }

    public ApiKey ApiKey { get; private set; } = null!;

    public DateOnly Date { get; private set; }

    public int Count { get; private set; }

    public void Increment()
    {
        Count++;
    }
}
