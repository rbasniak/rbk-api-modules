namespace rbkApiModules.Demo1.Tests.Integration.Identity;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class UserConfirmationTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public UserConfirmationTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;

        _serverFixture.ClearMailsFolder();
    }

    /// <summary>
    /// User cannot be confirmed with wrong code
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-151"), Priority(10)]
    public async Task User_Cannot_Be_Confirmed_With_Wrong_Code()
    {
        // Prepare
        var context = _serverFixture.Context;
        context.Set<User>().Add(new User("BUZIOS", "new_user", "new_user@company.com", "123456", "", "Newcomer"));
        context.SaveChanges();

        var user = _serverFixture.Context.Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeFalse();
        user.ActivationCode.ShouldNotBe(null);
        user.ActivationCode.ShouldNotBe("");

        // Act
        var response = await _serverFixture.GetAsync($"api/authentication/confirm-email?email={user.Email}&code=wrong-code&tenant={user.TenantId}", credentials: null);

        // Assert the response
        response.ShouldRedirect("/registration/fail");

        // Assert the database
        user = _serverFixture.Context.Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeFalse();
    }

    /// <summary>
    /// User cannot be confirmed with wrong code
    /// DEPENDENCIES: IT-151 (user created and should fail to confirm)
    /// </summary>
    [FriendlyNamedFact("IT-152"), Priority(50)]
    public async Task User_Cannot_Be_Confirmed_With_No_Code()
    {
        // Prepare
        var user = _serverFixture.Context.Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeFalse();
        user.ActivationCode.ShouldNotBe(null);
        user.ActivationCode.ShouldNotBe("");

        // Act
        var response = await _serverFixture.GetAsync($"api/authentication/confirm-email?email={user.Email}&tenant={user.TenantId}", credentials: null);

        // Assert the response
        response.ShouldRedirect("/registration/fail");

        // Assert the database
        user = _serverFixture.Context.Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeFalse();
    }

    /// <summary>
    /// User cannot be confirmed with wrong code
    /// DEPENDENCIES: IT-152 (user should fail to confirm)
    /// </summary>
    [FriendlyNamedFact("IT-153"), Priority(50)]
    public async Task User_Cannot_Be_Confirmed_With_No_Email()
    {
        // Prepare
        var user = _serverFixture.Context.Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeFalse();
        user.ActivationCode.ShouldNotBe(null);
        user.ActivationCode.ShouldNotBe("");

        // Act
        var response = await _serverFixture.GetAsync($"api/authentication/confirm-email?code={user.ActivationCode}&tenant={user.TenantId}", credentials: null);

        // Assert the response
        response.ShouldRedirect("/registration/fail");

        // Assert the database
        user = _serverFixture.Context.Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeFalse();
    }

    /// <summary>
    /// User cannot be confirmed with wrong code
    /// DEPENDENCIES: IT-153 (user should fail to confirm)
    /// </summary>
    [FriendlyNamedFact("IT-154"), Priority(50)]
    public async Task User_Cannot_Be_Confirmed_With_No_Tenant()
    {
        // Prepare
        var user = _serverFixture.Context.Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeFalse();
        user.ActivationCode.ShouldNotBe(null);
        user.ActivationCode.ShouldNotBe("");

        // Act
        var response = await _serverFixture.GetAsync($"api/authentication/confirm-email?email={user.Email}&code={user.ActivationCode}", credentials: null);

        // Assert the response
        response.ShouldRedirect("/registration/fail");

        // Assert the database
        user = _serverFixture.Context.Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeFalse();
    }

    /// <summary>
    /// User cannot be confirmed with wrong email
    /// DEPENDENCIES: IT-154 (user should fail to confirm)
    /// </summary>
    [FriendlyNamedFact("IT-155"), Priority(50)]
    public async Task User_Cannot_Be_Confirmed_With_Wrong_Email()
    {
        // Prepare
        var user = _serverFixture.Context.Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeFalse();
        user.ActivationCode.ShouldNotBe(null);
        user.ActivationCode.ShouldNotBe("");

        // Act
        var response = await _serverFixture.GetAsync($"api/authentication/confirm-email?email=wrong_email@company.com&code={user.ActivationCode}&tenant={user.TenantId}", credentials: null);

        // Assert the response
        response.ShouldRedirect("/registration/fail");

        // Assert the database
        user = _serverFixture.Context.Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeFalse();
    }

    /// <summary>
    /// User should be able to receive confirmation e-mail again before confirming its e-mail
    /// DEPENDENCIES: IT-155 (user should fail to confirm)
    /// </summary>
    [FriendlyNamedFact("IT-159"), Priority(55)]
    public async Task User_Can_Receive_Confirmation_Email_Again_If_Not_Yet_Confirmed()
    {
        // Prepare
        var user = _serverFixture.Context.Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeFalse();
        user.ActivationCode.ShouldNotBe(null);
        user.ActivationCode.ShouldNotBe("");

        var request = new ResendEmailConfirmation.Request
        {
            Email = user.Email,
        };

        // Act
        var response = await _serverFixture.PostAsync($"api/authentication/resend-confirmation", request, await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios"));

        // Assert the response
        response.ShouldBeSuccess();

        _serverFixture.ShouldHaveSentEmail(options => options
            .ToAddress(user.Email)
            .WithTileContaining("Registration confirmation")
        );

        // Assert the database
        user = _serverFixture.Context.Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeFalse();
    }

    /// <summary>
    /// User can be confirmed correctly 
    /// DEPENDENCIES: IT-159 (user should fail to confirm)
    /// </summary>
    [FriendlyNamedFact("IT-156"), Priority(100)]
    public async Task User_Can_Be_Confirmed_Correctly()
    {
        var user = _serverFixture.Context.Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeFalse();
        user.ActivationCode.ShouldNotBe(null);
        user.ActivationCode.ShouldNotBe("");

        var response = await _serverFixture.GetAsync($"api/authentication/confirm-email?email={user.Email}&code={user.ActivationCode}&tenant=buzios", credentials: null);

        // Assert the response
        response.ShouldRedirect("/registration/success");

        user = _serverFixture.Context.Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeTrue();
        user.ActivationCode.ShouldBeNull();

        _serverFixture.ShouldHaveSentEmail(options => options
            .ToAddress(user.Email)
            .WithTileContaining(user.DisplayName)
            .WithTileContaining("Welcome"));
    }

    /// <summary>
    /// Cannot resend confirmation e-mail is the user is already confirmed
    /// DEPENDENCIES: IT-150 (user should be confirmed)
    /// </summary>
    [FriendlyNamedFact("IT-157"), Priority(110)]
    public async Task User_Cannot_Receive_Confirmation_Email_If_It_Is_Already_Confirmed()
    {
        // Prepare
        var user = _serverFixture.Context.Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeTrue();
        user.ActivationCode.ShouldBeNull();

        var request = new ResendEmailConfirmation.Request
        {
            Email = user.Email,
        };

        var response = await _serverFixture.PostAsync($"api/authentication/resend-confirmation", request, await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "E-mail already confirmed");
    }

    /// <summary>
    /// Cannot resend confirmation e-mail is the user e-mail is not registered
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-158"), Priority(110)]
    public async Task User_Cannot_Receive_Confirmation_Email_If_Email_Does_Not_Exist()
    {
        // Act
        var request = new ResendEmailConfirmation.Request
        {
            Email = "incognito@company.com",
        };

        var response = await _serverFixture.PostAsync($"api/authentication/resend-confirmation", request, await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "E-mail not found");
    }
}
