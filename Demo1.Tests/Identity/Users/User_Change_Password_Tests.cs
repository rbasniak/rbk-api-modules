using Demo1.Tests;
using rbkApiModules.Commons.Core;
using rbkApiModules.Identity.Core;
using rbkApiModules.Identity.Core.DataTransfer.Users;

namespace rbkApiModules.Identity.Tests.Users;

[HumanFriendlyDisplayName]
public class User_Change_Password_Tests
{
    [ClassDataSource<Demo1TestingServer>(Shared = SharedType.PerClass)]
    public required Demo1TestingServer TestingServer { get; set; } = default!;

    [Test, NotInParallel(Order = 1)]
    public async Task Seed()
    {
        var context = TestingServer.CreateContext();

        context.Set<Tenant>().Add(new Tenant("WAYNE INC", "Wayne Industries"));

        var admin = context.Set<User>().Add(new User("WAYNE INC", "admin", "admin@wayne-inc.com", "admin123", "avatar", "Admin", AuthenticationMode.Credentials)).Entity;
        admin.Confirm();

        context.SaveChanges();

        // Default user for all tests
        await TestingServer.CacheCredentialsAsync("admin", "admin123", "wayne inc");
    }

    /// <summary>
    /// User cannot change password if old password doesn't match with the new one
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 2)]
    public async Task User_Cannot_Change_Password_If_Old_Password_Doesnt_Match()
    {
        // Prepare
        var request = new ChangePassword.Request
        {
            OldPassword = "old password",
            NewPassword = "new password",
            PasswordConfirmation = "new password"
        };

        // Act
        var response = await TestingServer.PostAsync<UserDetails>("api/authentication/change-password", request, "admin");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Old password does not match");
    }

    /// <summary>
    /// User cannot change password if new and confirmation passwords don't match
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 3)]
    public async Task User_Cannot_Change_Password_If_New_And_COnfirmation_Passwords_Dont_Match()
    {
        // Prepare
        var request = new ChangePassword.Request
        {
            OldPassword = "admin123",
            NewPassword = "new password",
            PasswordConfirmation = "new p@55w0rd"
        };

        // Act
        var response = await TestingServer.PostAsync<UserDetails>("api/authentication/change-password", request, "admin");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Password and confirmation must match");
    }

    /// <summary>
    /// User cannot change password if custom policies aren't met
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 4)]
    public async Task User_Cannot_Change_Password_If_Custom_Policies_Arent_Met()
    {
        // Prepare
        var request = new ChangePassword.Request
        {
            OldPassword = "admin123",
            NewPassword = "pw",
            PasswordConfirmation = "pw"
        };

        // Act
        var response = await TestingServer.PostAsync<UserDetails>("api/authentication/change-password", request, "admin");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The password must have at least 3 characteres");
    }

    /// <summary>
    /// User can change his password
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 5)]
    public async Task User_Can_Change_His_Password()
    {
        // Prepare
        var request = new ChangePassword.Request
        {
            OldPassword = "admin123",
            NewPassword = "new password",
            PasswordConfirmation = "new password"
        };

        // Act
        var response = await TestingServer.PostAsync("api/authentication/change-password", request, "admin");

        // Assert the response
        response.ShouldBeSuccess();

        var user = TestingServer.CreateContext().Set<User>().First(x => x.Username == "admin");
        user.Password.ShouldNotBeNull();
        PasswordHasher.VerifyPassword("new password", user.Password).ShouldBeTrue();
    }

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }
}
