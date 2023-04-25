namespace rbkApiModules.Demo1.Tests.Integration.Identity;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class UserDeletionTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public UserDeletionTests(ServerFixture serverFixture)
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
        admin.AddClaim(context.Set<Claim>().First(x => x.Identification == AuthenticationClaims.MANAGE_USERS), ClaimAccessType.Allow);

        var user = context.Set<User>().Add(new User("WAYNE INC", "user", "user@wayne-inc.com", "user123", String.Empty, "User", AuthenticationMode.Credentials)).Entity;
        user.Confirm();
        user.AddClaim(context.Set<Claim>().First(x => x.Identification == AuthenticationClaims.MANAGE_TENANT_SPECIFIC_ROLES), ClaimAccessType.Allow);
        user.AddClaim(context.Set<Claim>().First(x => x.Identification == AuthenticationClaims.MANAGE_CLAIMS), ClaimAccessType.Block);
        var role1 = context.Set<Role>().Add(new Role("WAYNE INC", "Role1")).Entity;
        var role2 = context.Set<Role>().Add(new Role("WAYNE INC", "Role2")).Entity;
        user.AddRole(role1);
        user.AddRole(role2);

        context.SaveChanges();

        // Default user for all tests
        await _serverFixture.GetAccessTokenAsync("user", "user123", "wayne inc");

        var users = _serverFixture.Context.Set<User>().Where(x => x.TenantId == "WAYNE INC").ToList();
        users.Count.ShouldBe(2);
    }

    /// <summary>
    /// User cannot be deleted when username is null or empty
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedTheory("IT-503"), Priority(10)]
    [InlineData(null)]
    [InlineData("")]
    public async Task User_cannot_be_deleted_when_username_is_null_or_empty(string username)
    {
        // Prepare
        var request = new DeleteUser.Request
        {
            Username = username,
        };

        // Act
        var response = await _serverFixture.PostAsync("api/authentication/user/delete", request, await _serverFixture.GetAccessTokenAsync("admin", "admin123", "wayne inc"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The field 'User' cannot be empty");
    }

    /// <summary>
    /// User cannot delete itself
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-496"), Priority(20)]
    public async Task User_cannot_delete_itself()
    {
        // Prepare
        var request = new DeleteUser.Request
        {
            Username = "admin",
        };

        // Act
        var response = await _serverFixture.PostAsync("api/authentication/user/delete", request, await _serverFixture.GetAccessTokenAsync("admin", "admin123", "wayne inc"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "User cannot delete itself");
    }

    /// <summary>
    /// User cannot be deleted when it does not exist
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-494"), Priority(30)]
    public async Task User_cannot_be_deleted_when_it_does_not_exist()
    {
        // Prepare
        var request = new DeleteUser.Request
        {
            Username = "unknown-user",
        };

        // Act
        var response = await _serverFixture.PostAsync("api/authentication/user/delete", request, await _serverFixture.GetAccessTokenAsync("admin", "admin123", "wayne inc"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "User not found");
    }

    /// <summary>
    /// User can be deleted
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-491"), Priority(40)]
    public async Task User_can_be_deleted()
    {
        var user = _serverFixture.Context.Set<User>().FirstOrDefault(x => x.TenantId == "WAYNE INC" && x.Username == "user");

        // Prepare
        var request = new DeleteUser.Request
        {
            Username = "user",
        };

        // Act
        var response = await _serverFixture.PostAsync("api/authentication/user/delete", request, await _serverFixture.GetAccessTokenAsync("admin", "admin123", "wayne inc"));

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        var result = _serverFixture.Context.Set<User>().FirstOrDefault(x => x.Id == user.Id);
        result.ShouldBeNull();
    }
}
