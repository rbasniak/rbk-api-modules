using rbkApiModules.Identity.Core;

namespace Demo4;

public class AutomaticUserCreationPostProcessor : ICustomUserPostProcessor
{
    public AutomaticUserCreationPostProcessor()
    {
        
    }

    public async Task<UserCustomInformation> GetUserExtraInformationAsync(string tenant, string username)
    {
        return await Task.FromResult(new UserCustomInformation
        {
            Avatar = AvatarGenerator.GenerateBase64Avatar(username),
            DisplayName = "Unknown John Doe",
            Email = "unknown_john_doe@company.com",
            Metadata = new Dictionary<string, string>()
        });
    }
}