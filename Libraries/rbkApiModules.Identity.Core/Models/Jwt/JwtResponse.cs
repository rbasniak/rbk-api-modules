namespace rbkApiModules.Identity.Core;

public class JwtResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}