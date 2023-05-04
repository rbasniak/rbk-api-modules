using rbkApiModules.Commons.Core;
using System.IdentityModel.Tokens.Jwt;

namespace rbkApiModules.Demo4.Tests.Integration;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class LoginTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public LoginTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    /// <summary>
    /// With Windows Authentication, user can login if all conditions are met
    /// </summary>
    [FriendlyNamedFact("IT-0000"), Priority(0)]
    public async Task User_can_login_with_Windows_Authentication_when_all_conditions_are_ok()
    {
        var command = new UserLogin.Request
        {
            Tenant = "PARKER INDUSTRIES"
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", command, credentials: Environment.UserName);

        // Assert
        response.ShouldBeSuccess();
        response.Data.ShouldNotBeNull("Response from server is null");
        response.Data.AccessToken.ShouldNotBeNull("Access token is null");
        response.Data.AccessToken.ShouldNotBeEmpty("Access token is empty");
        response.Data.RefreshToken.ShouldNotBeNull("Refresh token is null");
        response.Data.RefreshToken.ShouldNotBeEmpty("Refresh token is empty");

        var handler = new JwtSecurityTokenHandler();
        var tokenData = handler.ReadJwtToken(response.Data.AccessToken);

        var tenant = tokenData.Claims.First(claim => claim.Type == JwtClaimIdentifiers.Tenant).Value;
        var username1 = tokenData.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value;
        var username2 = tokenData.Claims.First(claim => claim.Type == System.Security.Claims.ClaimTypes.Name).Value;
        var displayName = tokenData.Claims.First(claim => claim.Type == JwtClaimIdentifiers.DisplayName).Value;
        var avatar = tokenData.Claims.First(claim => claim.Type == JwtClaimIdentifiers.Avatar).Value;
        var roles = tokenData.Claims.Where(claim => claim.Type == JwtClaimIdentifiers.Roles).ToList();
        var allowedTenants = tokenData.Claims.Where(claim => claim.Type == JwtClaimIdentifiers.AllowedTenants).ToList();
        var authenticationMode = tokenData.Claims.First(claim => claim.Type == JwtClaimIdentifiers.AuthenticationMode).Value;

        tenant.ShouldBe("PARKER INDUSTRIES");
        username1.ShouldBe(Environment.UserName);
        username2.ShouldBe(Environment.UserName);
        displayName.ShouldBe("John Doe");
        authenticationMode.ShouldBe("Windows");
        allowedTenants.Count.ShouldBe(2);
        allowedTenants.FirstOrDefault(x => x.Value == "PARKER INDUSTRIES").ShouldNotBeNull();
        allowedTenants.FirstOrDefault(x => x.Value == "OSCORP INDUSTRIES").ShouldNotBeNull();
    }
}