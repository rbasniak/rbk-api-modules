using System.Net.Http;
using System.Text;

namespace rbkApiModules.Tests.Integration.Identity;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class LocalUserCanLoginWithWindowsAuthenticatinIfEnabled : SequentialTest, IClassFixture<NtlmServerFixture>
{
    private NtlmServerFixture _serverFixture;

    public LocalUserCanLoginWithWindowsAuthenticatinIfEnabled(NtlmServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    /// <summary>
    /// User cann login with Windows Authentication when it's enabled
    /// </summary>
    [FriendlyNamedFact("IT-XXXX"), Priority(10)]
    public async Task User_Can_Login_With_Windows_Authentication()
    {
        var command = new UserLogin.Request
        { 
            Tenant = "buzios"
        };

        // Act
        var responseTemp = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", command, token: null);

        //// Assert
        //response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Invalid credentials");

        var handler = new HttpClientHandler()
        {
            UseDefaultCredentials = true,
            AllowAutoRedirect = false,
            PreAuthenticate = true,
        };

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost:44349")
        };
        
        httpClient.DefaultRequestHeaders.Add("Authorization", "NTLM " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Environment.UserName}:rbBrun@2012")));

        var request = new HttpRequestMessage(HttpMethod.Post, "api/authentication/login");

        // Act
        var response = await httpClient.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }
}

