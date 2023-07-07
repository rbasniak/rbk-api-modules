using Demo5;
using rbkApiModules.Commons.Core;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace rbkApiModules.Demo5.Tests.Integration;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class ServiceWithouthRoleAccessTests : SequentialTest, IClassFixture<ServerFixture> 
{
    private ServerFixture _serverFixture;

    private static string _token;

    public ServiceWithouthRoleAccessTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    /// <summary>
    ///  
    /// </summary>
    [FriendlyNamedFact(""), Priority(10)]
    public async Task Service_Can_Get_Access_Token()
    {
        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>(LegacyController.AUTH + "?withClaim=false", null, authenticated: false);

        // Assert
        response.ShouldBeSuccess();
        response.Data.ShouldNotBeNull("Response from server is null");
        response.Data.AccessToken.ShouldNotBeNull("Access token is null");
        response.Data.AccessToken.ShouldNotBeEmpty("Access token is empty");

        var handler = new JwtSecurityTokenHandler();
        var tokenData = handler.ReadJwtToken(response.Data.AccessToken);

        var roles = tokenData.Claims.Where(claim => claim.Type == JwtClaimIdentifiers.Roles).ToList();
        var issuers = tokenData.Claims.Where(claim => claim.Type == "iss").ToList();
        var audience = tokenData.Claims.Where(claim => claim.Type == "aud").ToList();

        roles.Count.ShouldBe(0);

        issuers.Count.ShouldBe(1);
        issuers.First().Value.ShouldBe("LEGACY AUTHENTICATION");

        audience.Count.ShouldBe(1);
        audience.First().Value.ShouldBe("LEGACY APPLICATION");

        _token = response.Data.AccessToken;
    }

    /// <summary>
    ///  
    /// </summary>
    [FriendlyNamedTheory(""), Priority(20)]
    [InlineData(ResourcesController.CURRENT_UNPROTECTED)]
    [InlineData(ResourcesController.CURRENT_PROTECTED)]
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
    [InlineData(ResourcesController.LEGACY_PROTECTED)]
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
    [InlineData(ResourcesController.LEGACY_UNPROTECTED)]
    [InlineData(ResourcesController.SHARED_UNPROTECTED)]
    public async Task Service_Should_Be_Authorized(string url)
    {
        // Act
        var response = await _serverFixture.GetAsync(url, _token);

        // Assert
        response.ShouldBeSuccess();
    }
}