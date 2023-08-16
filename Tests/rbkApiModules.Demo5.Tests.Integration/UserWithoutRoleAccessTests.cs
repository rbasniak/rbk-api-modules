using Demo5;
using rbkApiModules.Commons.Core;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace rbkApiModules.Demo5.Tests.Integration;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class UserWithouthRoleAccessTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    private static string _token;

    public UserWithouthRoleAccessTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    /// <summary>
    ///  
    /// </summary>
    [FriendlyNamedFact(""), Priority(10)]
    public async Task User_Can_Get_Access_Token()
    {
        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", new UserLogin.Request { Username = "user1", Password = "password1", Tenant = "DEMO" }, authenticated: false);

        // Assert
        response.ShouldBeSuccess();
        response.Data.ShouldNotBeNull("Response from server is null");
        response.Data.AccessToken.ShouldNotBeNull("Access token is null");
        response.Data.AccessToken.ShouldNotBeEmpty("Access token is empty");

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

        tenant.ShouldBe("DEMO");
        username1.ShouldBe("user1");
        username2.ShouldBe("user1");
        displayName.ShouldBe("User1");
        authenticationMode.ShouldBe("Credentials");
        allowedTenants.Count.ShouldBe(1);
        allowedTenants.FirstOrDefault(x => x.Value == "DEMO").ShouldNotBeNull();
        roles.Count.ShouldBe(0);

        _token = response.Data.AccessToken;
    }

    /// <summary>
    ///  
    /// </summary>
    [FriendlyNamedTheory(""), Priority(20)]
    [InlineData(ResourcesController.LEGACY_UNPROTECTED)]
    [InlineData(ResourcesController.LEGACY_PROTECTED)]
    public async Task Service_Should_Be_Unauthorized(string url)
    {
        // Act
        var response = await _serverFixture.GetAsync(url, _token);

        // Assert
        response.ShouldHaveErrors(HttpStatusCode.Unauthorized);
    }

    /// <summary>
    ///  
    /// </summary>
    [FriendlyNamedTheory(""), Priority(30)]
    [InlineData(ResourcesController.CURRENT_PROTECTED)]
    [InlineData(ResourcesController.SHARED_PROTECTED)]
    public async Task Service_Should_Be_Forbiden(string url)
    {
        // Act
        var response = await _serverFixture.GetAsync(url, _token);

        // Assert
        response.ShouldHaveErrors(HttpStatusCode.Forbidden);
    }

    /// <summary>
    ///  
    /// </summary>
    [FriendlyNamedTheory(""), Priority(40)]
    [InlineData(ResourcesController.CURRENT_UNPROTECTED)]
    [InlineData(ResourcesController.SHARED_UNPROTECTED)]
    public async Task Service_Should_Be_Authorized(string url)
    {
        // Act
        var response = await _serverFixture.GetAsync(url, _token);

        // Assert
        response.ShouldBeSuccess();
    }
}