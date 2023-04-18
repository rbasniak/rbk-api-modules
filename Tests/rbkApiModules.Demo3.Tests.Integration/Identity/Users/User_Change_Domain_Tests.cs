using rbkApiModules.Commons.Core;
using System.IdentityModel.Tokens.Jwt;

namespace rbkApiModules.Demo3.Tests.Integration.Identity;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class UserChangeDomainTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public UserChangeDomainTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }


    /// <summary>
    /// Login call, just to store the access tokens for future tests to use, and seed for the tests
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-000"), Priority(-1)]
    public void Seed()
    {
        var context = _serverFixture.Context;

        context.Set<Tenant>().Add(new Tenant("WAYNE INC", "Wayne Industries"));
        context.Set<Tenant>().Add(new Tenant("STARK ENTERPRISES", "Stark Enterprises"));

        context.SaveChanges();

        var user1 = context.Set<User>().Add(new User("WAYNE INC", "bruce.wayne", "bruce.wayne@wayne-inc.com", null, String.Empty, "Bruce Wayne")).Entity;
        var user2 = context.Set<User>().Add(new User("WAYNE INC", "spy", "spy@wayne-inc.com", null, String.Empty, "Spy (Waynce Inc.)")).Entity;
        var user3 = context.Set<User>().Add(new User("STARK ENTERPRISES", "spy", "spy@stark-enterprises.com", null, String.Empty, "Spy (Stark Enterprises)")).Entity;

        user1.Confirm();
        user2.Confirm();
        user3.Confirm();

        context.SaveChanges();
    }

    /// <summary>
    /// User cannot access change tenant anonymously
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-402"), Priority(5)]
    public async Task User_cannot_change_access_change_domain_anonymously()
    {
        // Prepare
        var command = new SwitchTenant.Request
        {
            Tenant = "wayne inc",
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/switch-tenant", command, credentials: null);

        // Assert
        response.ShouldHaveErrors(HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// User cannot change domain if the domain doesn't exist in database
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-423"), Priority(10)]
    public async Task User_cannot_change_domain_if_destination_domain_doesnt_exist()
    {
        // Prepare
        var loginRequest = new UserLogin.Request
        {
            Tenant = "wayne inc"
        };

        var loginResponse = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", loginRequest, "spy");

        loginResponse.ShouldBeSuccess();

        var command = new SwitchTenant.Request
        {
            Tenant = "oscorp",
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/switch-tenant", command, loginResponse.Data.AccessToken);

        // Assert
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Tenant não encontrado");
    }

    /// <summary>
    /// User cannot change domain if its user doesn't exist in the new domain
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-424"), Priority(20)]
    public async Task User_cannot_change_domain_if_he_doesnt_exist_in_destination_domain()
    {
        // Prepare
        var loginRequest = new UserLogin.Request
        {
            Tenant = "wayne inc"
        };

        var loginResponse = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", loginRequest, "bruce.wayne");

        loginResponse.ShouldBeSuccess();

        var command = new SwitchTenant.Request
        {
            Tenant = "Stark Enterprises",
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/switch-tenant", command, loginResponse.Data.AccessToken);

        // Assert
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Acesso negado");
    }

    /// <summary>
    /// User can change domain 
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-426"), Priority(30)]
    public async Task User_can_change_domain_if_he_exists_in_both_tenants()
    {
        // Prepare
        var loginRequest = new UserLogin.Request
        {
            Tenant = "wayne inc"
        };

        var loginResponse = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", loginRequest, "spy");

        loginResponse.ShouldBeSuccess();

        var command = new SwitchTenant.Request
        {
            Tenant = "Stark Enterprises",
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/switch-tenant", command, loginResponse.Data.AccessToken);

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
        var authenticationMode = tokenData.Claims.First(claim => claim.Type == JwtClaimIdentifiers.AuthenticationMode).Value;

        tenant.ShouldBe("STARK ENTERPRISES");
        username1.ShouldBe("spy");
        username2.ShouldBe("spy");
        displayName.ShouldBe("Spy (Stark Enterprises)");
        authenticationMode.ShouldBe("windows");
    }
}
