
using Demo1.Tests;
using rbkApiModules.Identity.Core;

namespace rbkApiModules.Identity.Tests.Login;

[NotInParallel(nameof(Custom_Login_Policies_Tests))]
public class Custom_Login_Policies_Tests
{
    [ClassDataSource<Demo1TestingServer>(Shared = SharedType.PerClass)]
    public required Demo1TestingServer TestingServer { get; set; } = default!;

    /// <summary>
    /// Check if the custom login policies are being loaded properly
    /// </summary>
    [Test, NotInParallel(Order = 1)]
    public async Task User_Cannot_Login_If_Custom_Policy_Did_Not_Allow_1()
    {
        // Prepare
        var request = new UserLogin.Request
        {
            Username = "forbidden",
            Password = "zzzzz"
        };

        // Act
        var response = await TestingServer.PostAsync<JwtResponse>("api/authentication/login", request);

        // Assert
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "You tried to login with the forbidden username");
    }

    /// <summary>
    /// Check if the custom login policies are being loaded properly
    /// </summary>
    [Test, NotInParallel(Order = 2)]
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
        var response = await TestingServer.PostAsync<JwtResponse>("api/authentication/login", request);

        // Assert
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "You tried to login with the forbidden tenant");
    }

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }
}