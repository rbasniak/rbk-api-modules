namespace rbkApiModules.Tests.Integration.Identity;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class LocalUserCanLoginWithWindowsAuthenticatinIfEnabled : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public LocalUserCanLoginWithWindowsAuthenticatinIfEnabled(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    /// <summary>
    /// User cann login with Windows Authentication when it's enabled
    /// </summary>
    [FriendlyNamedFact("IT-XXXX"), Priority(10)]
    public async Task User_Can_Login_With_Windows_Authentication()
    {
        // Prepare
        var request = new UserLogin.Request
        {
            Tenant = "buzios"
        };

        var additionalHeaders = new Dictionary<string, string>
        {
            { "withCredentials", "true" }
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", request, token: null, additionalHeaders);

        // Assert
        response.ShouldHaveErrors(HttpStatusCode.Unauthorized, "Invalid credentials");
    }
}

