using rbkApiModules.Commons.Core;
using rbkApiModules.Identity.Core.DataTransfer.Users;

namespace rbkApiModules.Demo1.Tests.Integration.Identity;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class UserChangePasswordTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public UserChangePasswordTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture; 
    }

    [FriendlyNamedFact("IT-000"), Priority(-1)]
    public async Task Seed()
    {
        var context = _serverFixture.Context;

        context.Set<Tenant>().Add(new Tenant("WAYNE INC", "Wayne Industries"));

        var admin = context.Set<User>().Add(new User("WAYNE INC", "admin", "admin@wayne-inc.com", "admin123", String.Empty, "Admin", AuthenticationMode.Credentials)).Entity;
        admin.Confirm();

        context.SaveChanges();

        // Default user for all tests
        await _serverFixture.GetAccessTokenAsync("admin", "admin123", "wayne inc");
    }

    /// <summary>
    /// User cannot change password if old password doesn't match with the new one
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-420"), Priority(10)]
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
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/change-password", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Old password does not match");
    }

    /// <summary>
    /// User cannot change password if new and confirmation passwords don't match
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-421"), Priority(20)]
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
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/change-password", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Password and confirmation must match");
    }

    /// <summary>
    /// User cannot change password if custom policies aren't met
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-422"), Priority(30)]
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
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/change-password", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The password must have at least 3 characteres");
    }

    /// <summary>
    /// User can change his password
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-425"), Priority(40)]
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
        var response = await _serverFixture.PostAsync("api/authentication/change-password", request, _serverFixture.GetDefaultAccessToken());

        // Assert the response
        response.ShouldBeSuccess();

        var user = _serverFixture.Context.Set<User>().First(x => x.Username == "admin");

        PasswordHasher.VerifyPassword("new password", user.Password).ShouldBeTrue();
    }
}
