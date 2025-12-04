using Demo1.Tests;
using rbkApiModules.Identity.Core;
using rbkApiModules.Identity.Core.DataTransfer.Users;

namespace rbkApiModules.Identity.Tests.Users;

[NotInParallel(nameof(User_Activation_Tests))]
public class User_Activation_Tests
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
        admin.AddClaim(context.Set<Claim>().First(x => x.Identification == AuthenticationClaims.MANAGE_USERS), ClaimAccessType.Allow);


        var user = context.Set<User>().Add(new User("WAYNE INC", "user", "user@wayne-inc.com", "user123", "avatar", "User", AuthenticationMode.Credentials)).Entity;
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
    /// User cannot be deactivated when username is null or empty
    /// DEPENDENCIES: none
    /// </summary>
    [Arguments(null)]
    [Arguments("")]
    [Test, NotInParallel(Order = 2)]
    public async Task User_cannot_be_deactivated_when_username_is_null_or_empty(string? username)
    {
        // Prepare
        var request = new DeativateUser.Request
        {
            Username = username!,
        };

        // Act
        var response = await TestingServer.PostAsync("api/authentication/users/deactivate", request, "admin");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "'Username' must not be empty.");
    }

    /// <summary>
    /// User cannot deactivate itself
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 3)]
    public async Task User_cannot_deactivate_itself()
    {
        // Prepare
        var request = new DeativateUser.Request
        {
            Username = "admin",
        };

        // Act
        var response = await TestingServer.PostAsync("api/authentication/users/deactivate", request, "admin");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "User cannot deactivate itself");
    }

    /// <summary>
    /// User cannot be deactivated when it does not exist
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 4)]
    public async Task User_cannot_be_deactivated_when_it_does_not_exist()
    {
        // Prepare
        var request = new DeativateUser.Request
        {
            Username = "unknown-user",
        };

        // Act
        var response = await TestingServer.PostAsync("api/authentication/users/deactivate", request, "admin");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "User not found");
    }

    /// <summary>
    /// User can be deactivated
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 5)]
    public async Task User_can_be_deactivated()
    {
        var user = TestingServer.CreateContext().Set<User>().FirstOrDefault(x => x.TenantId == "WAYNE INC" && x.Username == "user");
        user.ShouldNotBeNull();
        user.IsActive.ShouldBeTrue();

        // Prepare
        var request = new DeativateUser.Request
        {
            Username = "user",
        };

        // Act
        var response = await TestingServer.PostAsync<UserDetails>("api/authentication/users/deactivate", request, "admin");

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        var result = TestingServer.CreateContext().Set<User>().FirstOrDefault(x => x.Id == user.Id);
        result.ShouldNotBeNull();
        result.IsActive.ShouldBeFalse();
    }

    /// <summary>
    /// User cannot login when it is deactivated
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 6)]
    public async Task User_cannot_login_when_it_is_deactivated()
    {
        // Prepare
        var request = new UserLogin.Request
        {
            Username = "user",
            Password = "user123",
            Tenant = "wayne inc",
        };

        // Act
        var response = await TestingServer.PostAsync<JwtResponse>("api/authentication/login", request);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Account is deactivated");
    }

    /// <summary>
    /// User cannot be activated when username is null or empty
    /// DEPENDENCIES: none
    /// </summary>
    [Arguments(null)]
    [Arguments("")]
    [Test, NotInParallel(Order = 7)]
    public async Task User_cannot_be_activated_when_username_is_null_or_empty(string? username)
    {
        // Prepare
        var request = new ActivateUser.Request
        {
            Username = username!,
        };

        // Act
        var response = await TestingServer.PostAsync("api/authentication/users/activate", request, "admin");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "'Username' must not be empty.");
    }

    /// <summary>
    /// User cannot be activated when it does not exist
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 8)]
    public async Task User_cannot_be_activated_when_it_does_not_exist()
    {
        // Prepare
        var request = new ActivateUser.Request
        {
            Username = "unknown-user",
        };

        // Act
        var response = await TestingServer.PostAsync("api/authentication/users/activate", request, "admin");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "User not found");
    }

    /// <summary>
    /// User can be reactivated
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 9)]
    public async Task User_can_be_reactivated()
    {
        var user = TestingServer.CreateContext().Set<User>().FirstOrDefault(x => x.TenantId == "WAYNE INC" && x.Username == "user");
        user.ShouldNotBeNull();
        user.IsActive.ShouldBeFalse();

        // Prepare
        var request = new ActivateUser.Request
        {
            Username = "user",
        };

        // Act
        var response = await TestingServer.PostAsync("api/authentication/users/activate", request, "admin");

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        var result = TestingServer.CreateContext().Set<User>().FirstOrDefault(x => x.Id == user.Id);
        result.ShouldNotBeNull();
        result.IsActive.ShouldBeTrue();
    }

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }
}
