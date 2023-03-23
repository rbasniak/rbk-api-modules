namespace rbkApiModules.Identity.Core;

public interface ILoginData
{
    string Tenant {  get; }
    string Username { get; }
    string Password { get; }
}

public interface IUserMetadata
{
    public Dictionary<string, string> Metadata { get; set; }
}