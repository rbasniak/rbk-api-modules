namespace Demo3;

public interface ICustomIdentityProviderService
{
    ExternalUserInfo GetUserInfo(string username); 
}

public class CustomIdentityProviderService : ICustomIdentityProviderService
{
    public ExternalUserInfo GetUserInfo(string username)
    {
        if (username == "tony.stark")
        {
            return new ExternalUserInfo
            {
                IsValid = false,
                DisplayName = null,
                Email = null,
                Manager = null,
                Sector = null
            };
        }

        return new ExternalUserInfo
        {
            IsValid = true,
            DisplayName = username.Replace(".", " "),
            Email = username + "@wayne-inc.com",
            Manager = "Bruce Wayne",
            Sector = "Gadget Research"
        };
    } 
}

public class ExternalUserInfo
{
    public required bool IsValid { get; set; }
    public required string DisplayName { get; set; }
    public required string Email { get; set; }
    public required string Sector { get; set; }
    public required string Manager { get; set; }
}