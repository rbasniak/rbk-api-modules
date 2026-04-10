using Demo1.Tests;
using rbkApiModules.Identity.Core;

namespace rbkApiModules.Identity.Tests.Users;

[HumanFriendlyDisplayName]
public class User_Request_Password_Reset_Tests
{
    [ClassDataSource<Demo1TestingServer>(Shared = SharedType.PerClass)]
    public required Demo1TestingServer TestingServer { get; set; } = default!;

    [Test, NotInParallel(Order = 1)]
    public void Seed()
    {
        TestingServer.ClearMailsFolder();
    }

    /// <summary>
    /// User cannot be request password reset if it's not confirmed yet
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 2)]
    public async Task User_Cannot_Request_Password_Reset_If_It_Is_Not_Confirmed()
    {
        // Prepare
        var context = TestingServer.CreateContext();
        context.Set<User>().Add(new User("BUZIOS", "new_user", "new_user@company.com", "123456", "", "Newcomer", AuthenticationMode.Credentials));
        context.SaveChanges();

        var user = TestingServer.CreateContext().Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeFalse();
        user.ActivationCode.ShouldNotBe(null);
        user.ActivationCode.ShouldNotBe("");
        user.Email.ShouldNotBeNull();
        user.TenantId.ShouldNotBeNull();

        var request = new RequestPasswordReset.Request
        {
            Tenant = user.TenantId.ToLower(),
            Email = user.Email
        };

        // Act
        var response = await TestingServer.PostAsync($"api/authentication/reset-password", request);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "E-mail not confirmed");
    }

    /// <summary>
    /// User cannot be request password reset if it's not confirmed yet
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 3)]
    [Arguments(null)]
    [Arguments("")]
    public async Task User_Cannot_Request_Password_Reset_Without_Tenant(string? tenant)
    {
        // Prepare
        var user = TestingServer.CreateContext().Set<User>().First(x => x.Username == "admin1" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeTrue();
        user.Email.ShouldNotBeNull();

        var request = new RequestPasswordReset.Request
        {
            Tenant = tenant!,
            Email = user.Email
        };

        // Act
        var response = await TestingServer.PostAsync($"api/authentication/reset-password", request);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "'Tenant' must not be empty.");
    }

    /// <summary>
    /// User cannot be request password reset if it's registered
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 4)]
    public async Task User_Cannot_Request_Password_Reset_If_It_Is_Not_Registered()
    {
        // Prepare
        var request = new RequestPasswordReset.Request
        {
            Tenant = "buzios",
            Email = "incognito@company.com"
        };

        // Act
        var response = await TestingServer.PostAsync($"api/authentication/reset-password", request);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "E-mail not found");
    }

    /// <summary>
    /// User cannot be request password reset with the wrong tenant
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 5)]
    public async Task User_Cannot_Request_Password_Reset_With_Wrong_Tenant()
    {
        // Prepare
        var user = TestingServer.CreateContext().Set<User>().First(x => x.Username == "admin1" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeTrue();
        user.Email.ShouldNotBeNull();

        var request = new RequestPasswordReset.Request
        {
            Tenant = "XXXX",
            Email = user.Email
        };

        // Act
        var response = await TestingServer.PostAsync($"api/authentication/reset-password", request);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "E-mail not found");
    }

    /// <summary>
    /// User cannot be request password reset without email
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 6)]
    [Arguments(null)]
    [Arguments("")]
    public async Task User_Cannot_Request_Password_Reset_Without_Email(string? email)
    {
        // Prepare
        var user = TestingServer.CreateContext().Set<User>().First(x => x.Username == "admin1" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeTrue();
        user.TenantId.ShouldNotBeNull();

        var request = new RequestPasswordReset.Request
        {
            Tenant = user.TenantId,
            Email = email!
        };

        // Act
        var response = await TestingServer.PostAsync($"api/authentication/reset-password", request);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "'Email' must not be empty.");
    }

    /// <summary>
    /// User should be able to request password reset when everything is correct
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 7)]
    public async Task User_Can_Request_Password_Reset_When_State_Is_Correct()
    {
        // Prepare
        var user = TestingServer.CreateContext().Set<User>().First(x => x.Username == "admin1" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeTrue();
        user.Email.ShouldNotBeNull();
        user.TenantId.ShouldNotBeNull();

        var request = new RequestPasswordReset.Request
        {
            Tenant = user.TenantId,
            Email = user.Email
        };

        // Act
        var response = await TestingServer.PostAsync($"api/authentication/reset-password", request);

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        user = TestingServer.CreateContext().Set<User>().First(x => x.Username == "admin1" && x.TenantId == "BUZIOS");
        user.Email.ShouldNotBeNull();
        user.PasswordRedefineCode.ShouldNotBeNull();
        user.PasswordRedefineCode.CreationDate.ShouldNotBeNull();
        user.PasswordRedefineCode.CreationDate.Value.ShouldBeLessThan(DateTime.UtcNow);
        user.PasswordRedefineCode.Hash.ShouldNotBeNull();

        // Assert the e-mail
        TestingServer.ShouldHaveSentEmail(options => options
            .ToAddress(user.Email)
            .WithTileContaining("Password reset"));
    }

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }
}
