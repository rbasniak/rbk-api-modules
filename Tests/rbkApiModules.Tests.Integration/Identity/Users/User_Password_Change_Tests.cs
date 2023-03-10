namespace rbkApiModules.Tests.Integration.Identity;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class UserPasswordChangeTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public UserPasswordChangeTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;

        _serverFixture.ClearMailsFolder();
    }

    /// <summary>
    /// User cannot change password without a password reset code
    /// DEPENDENCIES: none
    /// </summary>
    [InlineData("")]
    [InlineData(null)]
    [FriendlyNamedTheory("IT-190"), Priority(10)]
    public async Task User_Cannot_Reset_Password_Without_Reset_Code(string code)
    {
        // Prepare
        var request = new RedefinePassword.Request
        {
            Code = code,
            Password = "xyz"
        };

        // Act
        var response = await _serverFixture.PostAsync($"api/authentication/redefine-password", request, token: null);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "O campo 'Password reset code' não pode ser vazio");
    }

    /// <summary>
    /// User cannot change password with an invalid password reset code
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-191"), Priority(20)]
    public async Task User_Cannot_Reset_Password_With_Invalid_Reset_Code()
    {
        // Prepare
        var request = new RedefinePassword.Request
        {
            Code = "xxxxxxxxxxxxxxxxxxxxxxxxx",
            Password = "xyz"
        };

        // Act
        var response = await _serverFixture.PostAsync($"api/authentication/redefine-password", request, token: null);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The password reset is code expired or was already used.");
    }

    /// <summary>
    /// User cannot change password with an empty password
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-192"), Priority(30)]
    public async Task User_Cannot_Reset_Password_With_Empty_Password()
    {
        User GetUser()
        {
            var user = _serverFixture.Context.Set<User>().Single(x => x.Username == "admin1" && x.TenantId == "BUZIOS");
            return user;
        }

        // Prepare
        var user = GetUser();
        var resetRequest = new RequestPasswordReset.Request
        {
            Email = user.Email,
            Tenant = user.TenantId.ToLower()
        };
        var resetResponse = await _serverFixture.PostAsync($"api/authentication/reset-password", resetRequest, token: null);
        resetResponse.ShouldBeSuccess();
        
        user = GetUser();
        user.PasswordRedefineCode.ShouldNotBeNull();
        String.IsNullOrEmpty(user.PasswordRedefineCode.Hash).ShouldBeFalse();

        var request = new RedefinePassword.Request
        {
            Code = user.PasswordRedefineCode.Hash,
            Password = ""
        };

        // Act
        var response = await _serverFixture.PostAsync($"api/authentication/redefine-password", request, token: null);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "O campo 'Password' não pode ser vazio");
    }

    /// <summary>
    /// User can still login with old password after all failed attempts
    /// DEPENDENCIES: IT-192
    /// </summary>
    [FriendlyNamedFact("IT-193"), Priority(40)]
    public async Task User_Can_Still_Login_With_Old_Password()
    {
        // Prepare
        var user = _serverFixture.Context.Set<User>().Single(x => x.Username == "admin1" && x.TenantId == "BUZIOS");
        user.PasswordRedefineCode.ShouldNotBeNull();

        var request = new UserLogin.Request
        {
            Username = "admin1",
            Password = "123",
            Tenant = "buzios"
        };

        // Act
        var response = await _serverFixture.PostAsync($"api/authentication/login", request, token: null);

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        user = _serverFixture.Context.Set<User>().Single(x => x.Username == "admin1" && x.TenantId == "BUZIOS");
        user.PasswordRedefineCode.ShouldBeNull();
    }

    /// <summary>
    /// User should be able to change password after a reset request
    /// DEPENDENCIES: IT-193
    /// </summary>
    [FriendlyNamedFact("IT-194"), Priority(50)]
    public async Task User_Can_Reset_Password_After_Requesting_It()
    {
        User GetUser()
        {
            var user = _serverFixture.Context.Set<User>().Single(x => x.Username == "admin1" && x.TenantId == "BUZIOS");
            return user;
        }

        // Prepare
        var user = GetUser();
        var resetRequest = new RequestPasswordReset.Request
        {
            Email = user.Email,
            Tenant = user.TenantId.ToLower()
        };
        var resetResponse = await _serverFixture.PostAsync($"api/authentication/reset-password", resetRequest, token: null);
        resetResponse.ShouldBeSuccess();

        user = GetUser();
        user.PasswordRedefineCode.ShouldNotBeNull();
        String.IsNullOrEmpty(user.PasswordRedefineCode.Hash).ShouldBeFalse();

        var request = new RedefinePassword.Request
        {
            Code = user.PasswordRedefineCode.Hash,
            Password = "xyz"
        };

        _serverFixture.ClearMailsFolder();

        // Act
        var response = await _serverFixture.PostAsync($"api/authentication/redefine-password", request, token: null);

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        user = GetUser();
        user.PasswordRedefineCode.ShouldBeNull();

        // Assert the e-mail
        _serverFixture.ShouldHaveSentEmail(options => options
            .ToAddress(user.Email)
            .WithTileContaining("Redefinição de senha bem sucedida")
        );
    }

    /// <summary>
    /// User cannot login with old password after password reset
    /// DEPENDENCIES: IT-194
    /// </summary>
    [FriendlyNamedFact("IT-195"), Priority(60)]
    public async Task User_Cannot_Login_With_Old_Password()
    {
        // Prepare
        var request = new UserLogin.Request
        {
            Username = "admin1",
            Password = "123",
            Tenant = "buzios"
        };

        // Act
        var response = await _serverFixture.PostAsync($"api/authentication/login", request, token: null);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Invalid credentials");
    }

    /// <summary>
    /// User can login with new password after password reset
    /// DEPENDENCIES: IT-194
    /// </summary>
    [FriendlyNamedFact("IT-195"), Priority(70)]
    public async Task User_Can_Login_With_New_Password()
    {
        // Prepare
        var request = new UserLogin.Request
        {
            Username = "admin1",
            Password = "xyz",
            Tenant = "buzios"
        };

        // Act
        var response = await _serverFixture.PostAsync($"api/authentication/login", request, token: null);

        // Assert the response
        response.ShouldBeSuccess();
    }
}
