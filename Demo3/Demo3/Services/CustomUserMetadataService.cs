using rbkApiModules.Identity.Core;

namespace Demo3;

public class CustomUserMetadataService : IUserMetadataService
{
    private readonly ICustomIdentityProviderService _identityProvider;

    public CustomUserMetadataService(ICustomIdentityProviderService identityProvider)
    {
        _identityProvider = identityProvider;
    } 

    public Task<Dictionary<string, string>> GetIdentityInfo(User user)
    {
        return Task.FromResult(new Dictionary<string, string> 
        {
            { "office", "1219" },
            { "building", "B11" },
            { "desk", "3" }
        });
    }
} 