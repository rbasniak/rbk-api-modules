namespace rbkApiModules.Identity.Core;

public interface ILoginData
{
    string Tenant {  get; }
    string Username { get; }
    string Password { get; }
}
