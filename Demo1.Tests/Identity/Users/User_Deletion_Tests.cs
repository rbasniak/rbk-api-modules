using Demo1.Tests;
using rbkApiModules.Identity.Core;

namespace rbkApiModules.Identity.Tests.Users;

[HumanFriendlyDisplayName]
public class User_Deletion_Tests
{
    [ClassDataSource<Demo1TestingServer>(Shared = SharedType.PerClass)]
    public required Demo1TestingServer TestingServer { get; set; } = default!;

    [Test, NotInParallel(Order = 1)]
    public async Task Seed()
    {
        var context = TestingServer.CreateContext();

        context.Set<Tenant>().Add(new Tenant("WAYNE INC", "Wayne Industries"));

        var admin = context.Set<User>().Add(new User("WAYNE INC", "admin", "admin@wayne-inc.com", "admin123", "Avatar", "Admin", AuthenticationMode.Credentials)).Entity;
        admin.Confirm();
        admin.AddClaim(context.Set<Claim>().First(x => x.Identification == AuthenticationClaims.MANAGE_USERS), ClaimAccessType.Allow);

        var user = context.Set<User>().Add(new User("WAYNE INC", "user", "user@wayne-inc.com", "user123", "Avatar", "User", AuthenticationMode.Credentials)).Entity;
        user.Confirm();
        user.AddClaim(context.Set<Claim>().First(x => x.Identification == AuthenticationClaims.MANAGE_TENANT_SPECIFIC_ROLES), ClaimAccessType.Allow);
        user.AddClaim(context.Set<Claim>().First(x => x.Identification == AuthenticationClaims.MANAGE_CLAIMS), ClaimAccessType.Block);
        var role1 = context.Set<Role>().Add(new Role("WAYNE INC", "Role1")).Entity;
        var role2 = context.Set<Role>().Add(new Role("WAYNE INC", "Role2")).Entity;
        user.AddRole(role1);
        user.AddRole(role2);

        context.SaveChanges();

        // Default user for all tests
        await TestingServer.CacheCredentialsAsync("user", "user123", "wayne inc");
        await TestingServer.CacheCredentialsAsync("admin", "admin123", "wayne inc");

        var users = TestingServer.CreateContext().Set<User>().Where(x => x.TenantId == "WAYNE INC").ToList();
        users.Count.ShouldBe(2);
    }

    /// <summary>
    /// User cannot be deleted when username is null or empty
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 2)]
    [Arguments(null)]
    [Arguments("")]
    public async Task User_cannot_be_deleted_when_username_is_null_or_empty(string? username)
    {
        // Prepare
        var request = new DeleteUser.Request
        {
            Username = username!,
        };

        // Act
        var response = await TestingServer.PostAsync("api/authentication/users/delete", request, "admin");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "'Username' must not be empty.");
    }

    /// <summary>
    /// User cannot delete itself
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 3)]
    public async Task User_cannot_delete_itself()
    {
        // Prepare
        var request = new DeleteUser.Request
        {
            Username = "admin",
        };

        // Act
        var response = await TestingServer.PostAsync("api/authentication/users/delete", request, "admin");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "User cannot delete itself");
    }

    /// <summary>
    /// User cannot be deleted when it does not exist
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 4)]
    public async Task User_cannot_be_deleted_when_it_does_not_exist()
    {
        // Prepare
        var request = new DeleteUser.Request
        {
            Username = "unknown-user",
        };

        // Act
        var response = await TestingServer.PostAsync("api/authentication/users/delete", request, "admin");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "User not found");
    }

    /// <summary>
    /// User can be deleted
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 5)]
    public async Task User_can_be_deleted()
    {
        var user = TestingServer.CreateContext().Set<User>().FirstOrDefault(x => x.TenantId == "WAYNE INC" && x.Username == "user");
        user.ShouldNotBeNull();

        // Prepare
        var request = new DeleteUser.Request
        {
            Username = "user",
        };

        // Act
        var response = await TestingServer.PostAsync("api/authentication/users/delete", request, "admin");

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        var result = TestingServer.CreateContext().Set<User>().FirstOrDefault(x => x.Id == user.Id);
        result.ShouldBeNull();
    }

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }
}
