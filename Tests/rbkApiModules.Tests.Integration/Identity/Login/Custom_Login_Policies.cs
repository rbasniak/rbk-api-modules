namespace rbkApiModules.Tests.Integration.Identity;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class CustomLoginPoliciesTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public CustomLoginPoliciesTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    /// <summary>
    /// Check if the custom login policies are being loaded properly
    /// </summary>
    [FriendlyNamedFact("IT-213"), Priority(10)]
    public async Task User_Cannot_Login_If_Custom_Policy_Did_Not_Allow_1()
    {
        // Prepare
        var request = new UserLogin.Request
        {
            Username = "forbidden",
            Password = "zzzzz"
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", request, token: null);

        // Assert
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "You tried to login with the forbidden username");
    }

    /// <summary>
    /// Check if the custom login policies are being loaded properly
    /// </summary>
    [FriendlyNamedFact("IT-214"), Priority(10)]
    public async Task User_Cannot_Login_If_Custom_Policy_Did_Not_Allow_2()
    {
        // Prepare
        var request = new UserLogin.Request
        {
            Tenant = "FORBIDDEN",
            Username = "admin1",
            Password = "123"
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", request, token: null);

        // Assert
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "You tried to login with the forbidden tenant");
    }
}