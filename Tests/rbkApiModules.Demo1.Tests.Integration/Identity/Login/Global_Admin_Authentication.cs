using rbkApiModules.Commons.Core;
using System.IdentityModel.Tokens.Jwt;

namespace rbkApiModules.Tests.Integration.Identity;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class GlobalAdminLoginTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public GlobalAdminLoginTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    /// <summary>
    /// New seeded database, try to login with global admin and wrong password
    /// </summary>
    [FriendlyNamedFact("IT-011"), Priority(10)]
    public async Task Global_Admin_Cannot_Login_With_Wrong_Password()
    {
        // Prepare
        var request = new UserLogin.Request
        {
            Username = "superuser",
            Password = "zzzzz"
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", request, credentials: null);

        // Assert
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Invalid credentials");
    }

    /// <summary>
    /// Try to login with correct credentials, also save the token for future requests
    /// </summary>
    [FriendlyNamedFact("IT-012/IT-215"), Priority(20)]
    public async Task Global_Admin_Can_Login_With_Username()
    {
        // Prepare
        var request = new UserLogin.Request
        {
            Username = "superuser",
            Password = "admin"
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", request, credentials: null);

        // Assert
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.AccessToken.ShouldNotBeNull();
        response.Data.RefreshToken.ShouldNotBeNull();

        var handler = new JwtSecurityTokenHandler();
        var tokenData = handler.ReadJwtToken(response.Data.AccessToken);

        var tenant = tokenData.Claims.First(claim => claim.Type == JwtClaimIdentifiers.Tenant).Value;
        var username1 = tokenData.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value;
        var username2 = tokenData.Claims.First(claim => claim.Type == System.Security.Claims.ClaimTypes.Name).Value;
        var displayName = tokenData.Claims.First(claim => claim.Type == JwtClaimIdentifiers.DisplayName).Value;
        var avatar = tokenData.Claims.First(claim => claim.Type == JwtClaimIdentifiers.Avatar).Value;
        var roles = tokenData.Claims.Where(claim => claim.Type == JwtClaimIdentifiers.Roles).ToList();

        tenant.ShouldBeEmpty();
        username1.ShouldBe("superuser");
        username2.ShouldBe("superuser");
        displayName.ShouldBe("Administrator");
        avatar.ShouldStartWith("data:image/svg+xml;base64,");
        roles.Count.ShouldBe(4);
        roles.FirstOrDefault(x => x.Value == AuthenticationClaims.MANAGE_TENANTS).ShouldNotBeNull();
        roles.FirstOrDefault(x => x.Value == AuthenticationClaims.MANAGE_CLAIMS).ShouldNotBeNull();
        roles.FirstOrDefault(x => x.Value == AuthenticationClaims.CHANGE_CLAIM_PROTECTION).ShouldNotBeNull();
        roles.FirstOrDefault(x => x.Value == AuthenticationClaims.MANAGE_APPLICATION_WIDE_ROLES).ShouldNotBeNull();

        var hasTenantClaim = tokenData.Claims.FirstOrDefault(claim => claim.Type == "has-tenant");
        var lastLoginClaim = tokenData.Claims.FirstOrDefault(claim => claim.Type == "last-login");

        hasTenantClaim.ShouldNotBeNull();
        lastLoginClaim.ShouldNotBeNull();
    }

    /// <summary>
    /// The ACME tenant exists in the databse, try to login with the global admin and this tenant.
    /// Since the global admin can only manage tenants, it should not be able to login with 
    /// a given tenant
    /// </summary>
    [FriendlyNamedFact("IT-013"), Priority(30)]
    public async Task Global_Admin_Cannot_Login_With_Tenant()
    {
        // Prepare 
        var request = new UserLogin.Request
        {
            Username = "superuser",
            Password = "admin",
            Tenant = "ACME"
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", request, credentials: null);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Invalid credentials");
    }
}

