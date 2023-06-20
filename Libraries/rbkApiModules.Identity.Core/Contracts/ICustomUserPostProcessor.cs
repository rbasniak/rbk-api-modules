namespace rbkApiModules.Identity.Core;

public interface ICustomUserPostProcessor
{
    Task<UserCustomInformation> GetUserExtraInformationAsync(string tenant, string username);
}

public class UserCustomInformation
{
    public required string DisplayName { get; set; }
    public required string Email { get; set; }
    public required string Avatar { get; set; }
    public required Dictionary<string, string> Metadata { get; set; }
}