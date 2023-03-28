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
}

