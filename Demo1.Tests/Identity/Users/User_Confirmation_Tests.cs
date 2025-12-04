using Demo1.Tests;
using rbkApiModules.Identity.Core;

namespace rbkApiModules.Identity.Tests.Users;

[NotInParallel(nameof(User_Confirmation_Tests))]
public class User_Confirmation_Tests
{
    [ClassDataSource<Demo1TestingServer>(Shared = SharedType.PerClass)]
    public required Demo1TestingServer TestingServer { get; set; } = default!;

    [Test, NotInParallel(Order = 1)]
    public async Task Seed()
    {
        TestingServer.ClearMailsFolder();

        await TestingServer.CacheCredentialsAsync("admin1", "123", "buzios");
    }

    /// <summary>
    /// User cannot be confirmed with wrong code
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 2)]
    public async Task User_Cannot_Be_Confirmed_With_Wrong_Code()
    {
        // Prepare
        var context = TestingServer.CreateContext();
        context.Set<User>().Add(new User("BUZIOS", "new_user", "new_user@company.com", "123456", "avatar", "Newcomer", AuthenticationMode.Credentials));
        context.SaveChanges();

        var user = TestingServer.CreateContext().Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeFalse();
        user.ActivationCode.ShouldNotBe(null);
        user.ActivationCode.ShouldNotBe("");

        // Act
        var response = await TestingServer.GetAsync($"api/authentication/confirm-email?email={user.Email}&code=wrong-code&tenant={user.TenantId}");

        // Assert the response
        response.ShouldRedirect("/registration/fail");

        // Assert the database
        user = TestingServer.CreateContext().Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeFalse();
    }

    /// <summary>
    /// User cannot be confirmed with wrong code
    /// DEPENDENCIES: IT-151 (user created and should fail to confirm)
    /// </summary>
    [Test, NotInParallel(Order = 3)]
    public async Task User_Cannot_Be_Confirmed_With_No_Code()
    {
        // Prepare
        var user = TestingServer.CreateContext().Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeFalse();
        user.ActivationCode.ShouldNotBe(null);
        user.ActivationCode.ShouldNotBe("");

        // Act
        var response = await TestingServer.GetAsync($"api/authentication/confirm-email?email={user.Email}&tenant={user.TenantId}");

        // Assert the response
        response.ShouldRedirect("/registration/fail");

        // Assert the database
        user = TestingServer.CreateContext().Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeFalse();
    }

    /// <summary>
    /// User cannot be confirmed with wrong code
    /// DEPENDENCIES: IT-152 (user should fail to confirm)
    /// </summary>
    [Test, NotInParallel(Order = 4)]
    public async Task User_Cannot_Be_Confirmed_With_No_Email()
    {
        // Prepare
        var user = TestingServer.CreateContext().Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeFalse();
        user.ActivationCode.ShouldNotBe(null);
        user.ActivationCode.ShouldNotBe("");

        // Act
        var response = await TestingServer.GetAsync($"api/authentication/confirm-email?code={user.ActivationCode}&tenant={user.TenantId}");

        // Assert the response
        response.ShouldRedirect("/registration/fail");

        // Assert the database
        user = TestingServer.CreateContext().Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeFalse();
    }

    /// <summary>
    /// User cannot be confirmed with wrong code
    /// DEPENDENCIES: IT-153 (user should fail to confirm)
    /// </summary>
    [Test, NotInParallel(Order = 5)]
    public async Task User_Cannot_Be_Confirmed_With_No_Tenant()
    {
        // Prepare
        var user = TestingServer.CreateContext().Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeFalse();
        user.ActivationCode.ShouldNotBe(null);
        user.ActivationCode.ShouldNotBe("");

        // Act
        var response = await TestingServer.GetAsync($"api/authentication/confirm-email?email={user.Email}&code={user.ActivationCode}");

        // Assert the response
        response.ShouldRedirect("/registration/fail");

        // Assert the database
        user = TestingServer.CreateContext().Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeFalse();
    }

    /// <summary>
    /// User cannot be confirmed with wrong email
    /// DEPENDENCIES: IT-154 (user should fail to confirm)
    /// </summary>
    [Test, NotInParallel(Order = 6)]
    public async Task User_Cannot_Be_Confirmed_With_Wrong_Email()
    {
        // Prepare
        var user = TestingServer.CreateContext().Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeFalse();
        user.ActivationCode.ShouldNotBe(null);
        user.ActivationCode.ShouldNotBe("");

        // Act
        var response = await TestingServer.GetAsync($"api/authentication/confirm-email?email=wrong_email@company.com&code={user.ActivationCode}&tenant={user.TenantId}");

        // Assert the response
        response.ShouldRedirect("/registration/fail");

        // Assert the database
        user = TestingServer.CreateContext().Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeFalse();
    }

    /// <summary>
    /// User should be able to receive confirmation e-mail again before confirming its e-mail
    /// DEPENDENCIES: IT-155 (user should fail to confirm)
    /// </summary>
    [Test, NotInParallel(Order = 7)]
    public async Task User_Can_Receive_Confirmation_Email_Again_If_Not_Yet_Confirmed()
    {
        // Prepare
        var user = TestingServer.CreateContext().Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.ShouldNotBeNull();
        user.Email.ShouldNotBeNull();
        user.IsConfirmed.ShouldBeFalse();
        user.ActivationCode.ShouldNotBe(null);
        user.ActivationCode.ShouldNotBe("");

        var request = new ResendEmailConfirmation.Request
        {
            Email = user.Email,
        };

        // Act
        var response = await TestingServer.PostAsync($"api/authentication/resend-confirmation", request, "admin1");

        // Assert the response
        response.ShouldBeSuccess();

        TestingServer.ShouldHaveSentEmail(options => options
            .ToAddress(user.Email)
            .WithTileContaining("Registration confirmation")
        );

        // Assert the database
        user = TestingServer.CreateContext().Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeFalse();
    }

    /// <summary>
    /// User can be confirmed correctly 
    /// DEPENDENCIES: IT-159 (user should fail to confirm)
    /// </summary>
    [Test, NotInParallel(Order = 8)]
    public async Task User_Can_Be_Confirmed_Correctly()
    {
        var user = TestingServer.CreateContext().Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeFalse();
        user.ActivationCode.ShouldNotBe(null);
        user.ActivationCode.ShouldNotBe("");

        var response = await TestingServer.GetAsync($"api/authentication/confirm-email?email={user.Email}&code={user.ActivationCode}&tenant=buzios");

        // Assert the response
        response.ShouldRedirect("/registration/success");

        user = TestingServer.CreateContext().Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeTrue();
        user.ActivationCode.ShouldBeNull();
        user.Email.ShouldNotBeNull();

        TestingServer.ShouldHaveSentEmail(options => options
            .ToAddress(user.Email)
            .WithTileContaining(user.DisplayName)
            .WithTileContaining("Welcome"));
    }

    /// <summary>
    /// Cannot resend confirmation e-mail is the user is already confirmed
    /// DEPENDENCIES: IT-150 (user should be confirmed)
    /// </summary>
    [Test, NotInParallel(Order = 9)]
    public async Task User_Cannot_Receive_Confirmation_Email_If_It_Is_Already_Confirmed()
    {
        // Prepare
        var user = TestingServer.CreateContext().Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.Email.ShouldNotBeNull();
        user.IsConfirmed.ShouldBeTrue();
        user.ActivationCode.ShouldBeNull();

        var request = new ResendEmailConfirmation.Request
        {
            Email = user.Email,
        };

        var response = await TestingServer.PostAsync($"api/authentication/resend-confirmation", request, "admin1");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "E-mail already confirmed");
    }

    /// <summary>
    /// Cannot resend confirmation e-mail is the user e-mail is not registered
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 10)]
    public async Task User_Cannot_Receive_Confirmation_Email_If_Email_Does_Not_Exist()
    {
        // Act
        var request = new ResendEmailConfirmation.Request
        {
            Email = "incognito@company.com",
        };

        var response = await TestingServer.PostAsync($"api/authentication/resend-confirmation", request, "admin1");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "E-mail not found");
    }

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }
}
