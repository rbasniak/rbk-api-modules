using rbkApiModules.Commons.Core;
using System.IdentityModel.Tokens.Jwt;

namespace rbkApiModules.Demo3.Tests.Integration.Identity;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class WindowsAuthenticationTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public WindowsAuthenticationTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    /// <summary>
    /// With Windows Authentication, user can login if all conditions are met
    /// </summary>
    [FriendlyNamedFact("IT-403"), Priority(10)]
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
    /// With Windows Authentication, user cannot login if it doesn't pass a a custom login validator 
    /// </summary>
    [FriendlyNamedFact("IT-404"), Priority(20)]
    public async Task User_cannot_login_with_Windows_Authentication_when_custom_validators_are_not_met()
    {
        var command = new UserLogin.Request
        {
            Tenant = "PARKER INDUSTRIES"
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", command, credentials: "tony.stark");

        // Assert
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Credenciais inválidas");
    }

    /// <summary>
    /// With Windows Authentication user cannot login if it passes a custom login validator but doesn't exist in database
    /// </summary>
    [FriendlyNamedFact("IT-405"), Priority(30)]
    public async Task User_cannot_login_with_Windows_Authentication_when_custom_validators_are_met_but_user_does_not_exist()
    {
        var command = new UserLogin.Request
        {
            Tenant = "PARKER INDUSTRIES"
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", command, credentials: "lucius.fox");

        // Assert
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Credenciais inválidas");
    }

    /// <summary>
    /// With Windows Authentication, user can still login as superuser
    /// </summary>
    [FriendlyNamedFact("IT-497"), Priority(40)]
    public async Task User_can_login_with_superuser_when_using_Windows_Authentication()
    {
        var command = new UserLogin.Request
        {
            Username = "superuser",
            Password = "admin"
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
        var allowedTenants = tokenData.Claims.Where(claim => claim.Type == JwtClaimIdentifiers.AllowedTenants).ToList();

        tenant.ShouldBeEmpty();
        username1.ShouldBe("superuser");
        username2.ShouldBe("superuser");
        allowedTenants.Count.ShouldBe(1);
        allowedTenants[0].Value.ToString().ShouldBeEmpty();
    }

    /// <summary>
    /// User can refresh token with valid data
    /// </summary>
    [FriendlyNamedFact("IT-???"), Priority(50)]
    public async Task User_Can_Refresh_Token_With_Valid_Data()
    {
        // Prepare
        var loginRequest = new UserLogin.Request
        {
            Tenant = "PARKER INDUSTRIES"
        };
        var authDataResponse = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", loginRequest, credentials: Environment.UserName);

        authDataResponse.ShouldBeSuccess();

        // To allow for the token data to change between calls
        Thread.Sleep(1500);

        // Act
        var request = new RenewAccessToken.Request
        {
            RefreshToken = authDataResponse.Data.RefreshToken
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/refresh-token", request, credentials: null);

        // Assert
        response.ShouldBeSuccess();
        response.Data.AccessToken.ShouldNotBe(authDataResponse.Data.AccessToken);

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

    [FriendlyNamedFact("IT-???"), Priority(60)]
    public async Task User_Cannot_Login_Using_Credentials()
    {
        // Precondition checks
        var user = _serverFixture.Context.Set<User>().Single(x => x.Username == Environment.UserName && x.TenantId == "PARKER INDUSTRIES");

        // Prepare
        var loginRequest = new UserLogin.Request
        {
            Username = Environment.UserName,
            Password = "123",
            Tenant = "buzios"
        };
        var loginResponse = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", loginRequest, credentials: null);
        loginResponse.ShouldHaveErrors(HttpStatusCode.Unauthorized);
    }
}

