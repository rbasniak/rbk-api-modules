namespace rbkApiModules.Identity.Core;

public interface IUserMetadataService
{
    Task AppendMetadataAsync(User user);
}

