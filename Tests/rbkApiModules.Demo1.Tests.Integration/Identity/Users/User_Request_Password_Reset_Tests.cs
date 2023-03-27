namespace rbkApiModules.Demo1.Tests.Integration.Identity;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class UserRequestPasswordResetTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public UserRequestPasswordResetTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;

        _serverFixture.ClearMailsFolder();
    }

    /// <summary>
    /// User cannot be request password reset if it's not confirmed yet
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-160"), Priority(10)]
    public async Task User_Cannot_Request_Password_Reset_If_It_Is_Not_Confirmed()
    {
        // Prepare
        var context = _serverFixture.Context;
        context.Set<User>().Add(new User("BUZIOS", "new_user", "new_user@company.com", "123456", "", "Newcomer"));
        context.SaveChanges();

        var user = _serverFixture.Context.Set<User>().First(x => x.Username == "new_user" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeFalse();
        user.ActivationCode.ShouldNotBe(null);
        user.ActivationCode.ShouldNotBe("");

        var request = new RequestPasswordReset.Request
        {
            Tenant = user.TenantId.ToLower(),
            Email = user.Email
        };

        // Act
        var response = await _serverFixture.PostAsync($"api/authentication/reset-password", request, credentials: null);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "E-mail not confirmed");
    }

    /// <summary>
    /// User cannot be request password reset if it's not confirmed yet
    /// DEPENDENCIES: none
    /// </summary>
    [InlineData(null)]
    [InlineData("")]
    [FriendlyNamedTheory("IT-162"), Priority(20)]
    public async Task User_Cannot_Request_Password_Reset_Without_Tenant(string tenant)
    {
        // Prepare
        var user = _serverFixture.Context.Set<User>().First(x => x.Username == "admin1" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeTrue();

        var request = new RequestPasswordReset.Request
        {
            Tenant = tenant,
            Email = user.Email
        };

        // Act
        var response = await _serverFixture.PostAsync($"api/authentication/reset-password", request, credentials: null);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The field 'Tenant' cannot be empty");
    }

    /// <summary>
    /// User cannot be request password reset if it's registered
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-161"), Priority(30)]
    public async Task User_Cannot_Request_Password_Reset_If_It_Is_Not_Registered()
    {
        // Prepare
        var request = new RequestPasswordReset.Request
        {
            Tenant = "buzios",
            Email = "incognito@company.com"
        };

        // Act
        var response = await _serverFixture.PostAsync($"api/authentication/reset-password", request, credentials: null);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "E-mail not found");
    }

    /// <summary>
    /// User cannot be request password reset with the wrong tenant
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-163"), Priority(40)]
    public async Task User_Cannot_Request_Password_Reset_With_Wrong_Tenant()
    {
        // Prepare
        var user = _serverFixture.Context.Set<User>().First(x => x.Username == "admin1" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeTrue();

        var request = new RequestPasswordReset.Request
        {
            Tenant = "XXXX",
            Email = user.Email
        };

        // Act
        var response = await _serverFixture.PostAsync($"api/authentication/reset-password", request, credentials: null);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "E-mail not found");
    }

    /// <summary>
    /// User cannot be request password reset without email
    /// DEPENDENCIES: none
    /// </summary>
    [InlineData("")]
    [InlineData(null)]
    [FriendlyNamedTheory("IT-164"), Priority(40)]
    public async Task User_Cannot_Request_Password_Reset_Without_Email(string email)
    {
        // Prepare
        var user = _serverFixture.Context.Set<User>().First(x => x.Username == "admin1" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeTrue();

        var request = new RequestPasswordReset.Request
        {
            Tenant = user.TenantId,
            Email = email
        };

        // Act
        var response = await _serverFixture.PostAsync($"api/authentication/reset-password", request, credentials: null);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The field 'E-mail' cannot be empty");
    }

    /// <summary>
    /// User should be able to request password reset when everything is correct
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-165"), Priority(50)]
    public async Task User_Can_Request_Password_Reset_When_State_Is_Correct()
    {
        // Prepare
        var user = _serverFixture.Context.Set<User>().First(x => x.Username == "admin1" && x.TenantId == "BUZIOS");
        user.IsConfirmed.ShouldBeTrue();

        var request = new RequestPasswordReset.Request
        {
            Tenant = user.TenantId,
            Email = user.Email
        };

        // Act
        var response = await _serverFixture.PostAsync($"api/authentication/reset-password", request, credentials: null);

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        user = _serverFixture.Context.Set<User>().First(x => x.Username == "admin1" && x.TenantId == "BUZIOS");
        user.PasswordRedefineCode.ShouldNotBeNull();
        user.PasswordRedefineCode.CreationDate.ShouldNotBeNull();
        user.PasswordRedefineCode.CreationDate.Value.ShouldBeLessThan(DateTime.UtcNow);
        user.PasswordRedefineCode.Hash.ShouldNotBeNull();

        // Assert the e-mail
        _serverFixture.ShouldHaveSentEmail(options => options
            .ToAddress(user.Email)
            .WithTileContaining("Password reset"));
    }
}
