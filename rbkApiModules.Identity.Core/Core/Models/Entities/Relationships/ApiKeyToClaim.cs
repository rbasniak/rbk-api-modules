namespace rbkApiModules.Identity.Core;

public sealed class ApiKeyToClaim
{
    private ApiKeyToClaim()
    {
    }

    public ApiKeyToClaim(ApiKey apiKey, Claim claim)
    {
        ApiKey = apiKey;
        Claim = claim;
    }

    public Guid ApiKeyId { get; private set; }
    public ApiKey ApiKey { get; private set; }

    public Guid ClaimId { get; private set; }
    public Claim Claim { get; private set; }
}
