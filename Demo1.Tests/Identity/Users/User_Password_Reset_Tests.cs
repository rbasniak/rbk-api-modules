using Demo1.Tests;
using rbkApiModules.Identity.Core;

namespace rbkApiModules.Identity.Tests.Users;

[NotInParallel(nameof(User_Password_Reset_Tests))]
public class User_Password_Reset_Tests
{
    [ClassDataSource<Demo1TestingServer>(Shared = SharedType.PerClass)]
    public required Demo1TestingServer TestingServer { get; set; } = default!;

    /// <summary>
    /// User cannot change password without a password reset code
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 1)]
    [Arguments(null)]
    [Arguments("")]
    public async Task User_Cannot_Reset_Password_Without_Reset_Code(string? code)
    {
        TestingServer.ClearMailsFolder();

        // Prepare
        var request = new RedefinePassword.Request
        {
            Code = code!,
            Password = "xyz"
        };

        // Act
        var response = await TestingServer.PostAsync($"api/authentication/redefine-password", request);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "'Code' must not be empty.");
    }

    /// <summary>
    /// User cannot change password with an invalid password reset code
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 2)]
    public async Task User_Cannot_Reset_Password_With_Invalid_Reset_Code()
    {
        // Prepare
        var request = new RedefinePassword.Request
        {
            Code = "xxxxxxxxxxxxxxxxxxxxxxxxx",
            Password = "xyz"
        };

        // Act
        var response = await TestingServer.PostAsync($"api/authentication/redefine-password", request);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The password reset is code expired or was already used");
    }

    /// <summary>
    /// User cannot change password with an empty password
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 3)]
    public async Task User_Cannot_Reset_Password_With_Empty_Password()
    {
        User GetUser()
        {
            var user = TestingServer.CreateContext().Set<User>().Single(x => x.Username == "admin1" && x.TenantId == "BUZIOS");
            return user;
        }

        // Prepare
        var user = GetUser();

        user.Email.ShouldNotBeNull();
        user.TenantId.ShouldNotBeEmpty();

        var resetRequest = new RequestPasswordReset.Request
        {
            Email = user.Email,
            Tenant = user.TenantId.ToLower()
        };
        var resetResponse = await TestingServer.PostAsync($"api/authentication/reset-password", resetRequest);
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
        var response = await TestingServer.PostAsync($"api/authentication/redefine-password", request);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "'Password' must not be empty.");
    }

    /// <summary>
    /// User can still login with old password after all failed attempts
    /// DEPENDENCIES: IT-192
    /// </summary>
    [Test, NotInParallel(Order = 4)]
    public async Task User_Can_Still_Login_With_Old_Password()
    {
        // Prepare
        var user = TestingServer.CreateContext().Set<User>().Single(x => x.Username == "admin1" && x.TenantId == "BUZIOS");
        user.PasswordRedefineCode.ShouldNotBeNull();

        var request = new UserLogin.Request
        {
            Username = "admin1",
            Password = "123",
            Tenant = "buzios"
        };

        // Act
        var response = await TestingServer.PostAsync($"api/authentication/login", request);

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        user = TestingServer.CreateContext().Set<User>().Single(x => x.Username == "admin1" && x.TenantId == "BUZIOS");
        user.PasswordRedefineCode.ShouldBeNull();
    }

    /// <summary>
    /// User should be able to change password after a reset request
    /// DEPENDENCIES: IT-193
    /// </summary>
    [Test, NotInParallel(Order = 5)]
    public async Task User_Can_Reset_Password_After_Requesting_It()
    {
        User GetUser()
        {
            var user = TestingServer.CreateContext().Set<User>().Single(x => x.Username == "admin1" && x.TenantId == "BUZIOS");
            return user;
        }

        // Prepare
        var user = GetUser();

        user.Email.ShouldNotBeNull();
        user.TenantId.ShouldNotBeEmpty();

        var resetRequest = new RequestPasswordReset.Request
        {
            Email = user.Email,
            Tenant = user.TenantId.ToLower()
        };
        var resetResponse = await TestingServer.PostAsync($"api/authentication/reset-password", resetRequest);
        resetResponse.ShouldBeSuccess();

        user = GetUser();
        user.PasswordRedefineCode.ShouldNotBeNull();
        String.IsNullOrEmpty(user.PasswordRedefineCode.Hash).ShouldBeFalse();

        var request = new RedefinePassword.Request
        {
            Code = user.PasswordRedefineCode.Hash,
            Password = "xyz"
        };

        TestingServer.ClearMailsFolder();

        // Act
        var response = await TestingServer.PostAsync($"api/authentication/redefine-password", request);

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        user = GetUser();
        user.Email.ShouldNotBeNull();
        user.PasswordRedefineCode.ShouldBeNull();

        // Assert the e-mail
        TestingServer.ShouldHaveSentEmail(options => options
            .ToAddress(user.Email)
            .WithTileContaining("Password successfully reset")
        );
    }

    /// <summary>
    /// User cannot login with old password after password reset
    /// DEPENDENCIES: IT-194
    /// </summary>
    [Test, NotInParallel(Order = 6)]
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
        var response = await TestingServer.PostAsync($"api/authentication/login", request);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Invalid credentials");
    }

    /// <summary>
    /// User can login with new password after password reset
    /// DEPENDENCIES: IT-194
    /// </summary>
        [Test, NotInParallel(Order = 7)]
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
        var response = await TestingServer.PostAsync($"api/authentication/login", request);

        // Assert the response
        response.ShouldBeSuccess();
    }

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }
}
