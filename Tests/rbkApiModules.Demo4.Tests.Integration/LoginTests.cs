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
    [FriendlyNamedFact("IT-X001"), Priority(10)]
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

    /// <summary>
    /// With Windows Authentication and automatic user creation, if user does not exist, 
    /// he can login and the user will be created with the default role
    /// </summary>
    [FriendlyNamedFact("IT-X002"), Priority(20)]
    public async Task User_can_login_with_Windows_Authentication_when_it_does_not_exist_and_the_application_allows()
    {
        var command = new UserLogin.Request
        {
            Tenant = "PARKER INDUSTRIES"
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", command, credentials: "UNKNOWN");

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
        username1.ShouldBe("unknown");
        username2.ShouldBe("unknown");
        displayName.ShouldBe("Unknown John Doe");
        authenticationMode.ShouldBe("Windows");
        allowedTenants.Count.ShouldBe(2);
        allowedTenants.FirstOrDefault(x => x.Value == "PARKER INDUSTRIES").ShouldNotBeNull();
        allowedTenants.FirstOrDefault(x => x.Value == "OSCORP INDUSTRIES").ShouldNotBeNull();
        roles.Count.ShouldBe(1);

        // Assert the database
        var user = await _serverFixture.Context.Set<User>()
            .Include(x => x.Roles)
                .ThenInclude(x => x.Role)
            .SingleOrDefaultAsync(x => x.Username.ToLower() == "unknown" && x.TenantId == "PARKER INDUSTRIES");

        user.ShouldNotBeNull("User was not created automatically");
        user.DisplayName.ShouldBe("Unknown John Doe");
        user.Email.ShouldBe("unknown_john_doe@company.com");
        user.Roles.Count().ShouldBe(1);
        user.Roles.First().Role.Name.ShouldBe("Readonly user");
    }

    /// <summary>
    /// With Windows Authentication and automatic user creation, if user does not exist, 
    /// he can switch to any tenant and the user will be created with the default role
    /// </summary>
    [FriendlyNamedFact("IT-X003"), Priority(20)]
    public async Task User_can_switch_tenant_with_Windows_Authentication_when_it_does_not_exist_in_the_tenant_and_the_application_allows()
    {
        // Prepare
        var loginResponse = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", new UserLogin.Request
        {
            Tenant = "PARKER INDUSTRIES"
        }, credentials: "UNKNOWN");

        var command = new SwitchTenant.Request
        {
            Tenant = "OSCORP INDUSTRIES"
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/switch-tenant", command, credentials: loginResponse.Data.AccessToken);

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

        tenant.ShouldBe("OSCORP INDUSTRIES");
        username1.ShouldBe("unknown");
        username2.ShouldBe("unknown");
        displayName.ShouldBe("Unknown John Doe");
        authenticationMode.ShouldBe("Windows");
        allowedTenants.Count.ShouldBe(2);
        allowedTenants.FirstOrDefault(x => x.Value == "PARKER INDUSTRIES").ShouldNotBeNull();
        allowedTenants.FirstOrDefault(x => x.Value == "OSCORP INDUSTRIES").ShouldNotBeNull();
        roles.Count.ShouldBe(1);

        // Assert the database
        var user = await _serverFixture.Context.Set<User>()
            .Include(x => x.Roles)
                .ThenInclude(x => x.Role)
            .SingleOrDefaultAsync(x => x.Username.ToLower() == "unknown" && x.TenantId == "OSCORP INDUSTRIES");

        user.ShouldNotBeNull("User was not created automatically");
        user.DisplayName.ShouldBe("Unknown John Doe");
        user.Email.ShouldBe("unknown_john_doe@company.com");
        user.Roles.Count().ShouldBe(1);
        user.Roles.First().Role.Name.ShouldBe("Readonly user");
    }
}