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
    /// User cannot login with Windows Authentication when if user doesn't exist
    /// </summary>
    [FriendlyNamedFact("IT-XXXX"), Priority(10)]
    public async Task User_Cannot_Login_With_Windows_Authentication_When_User_Does_Not_Exist()
    {
        var command = new UserLogin.Request
        {
            Tenant = "no tenant"
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", command, credentials: Environment.UserName);

        // Assert
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Invalid credentials");
    }

    /// <summary>
    /// User cann login with Windows Authentication when it's enabled
    /// </summary>
    [FriendlyNamedFact("IT-XXXX"), Priority(20)]
    public async Task User_Can_Login_With_Windows_Authentication_When_User_Exists()
    {
        var command = new UserLogin.Request
        { 
            Tenant = "wayne inc"
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", command, credentials: Environment.UserName);

        // Assert
        response.ShouldBeSuccess();
    }
}

