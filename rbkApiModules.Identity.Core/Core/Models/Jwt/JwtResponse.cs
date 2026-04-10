namespace rbkApiModules.Identity.Core;

public class JwtResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}