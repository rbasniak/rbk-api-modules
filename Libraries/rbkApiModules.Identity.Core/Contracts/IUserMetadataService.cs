namespace rbkApiModules.Identity.Core;

public interface IUserMetadataService
{
    Task<Dictionary<string, string>> GetIdentityInfo(User user);
}

