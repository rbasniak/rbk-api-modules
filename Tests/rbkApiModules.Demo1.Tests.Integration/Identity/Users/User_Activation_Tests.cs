using rbkApiModules.Identity.Core.DataTransfer.Users;

namespace rbkApiModules.Demo1.Tests.Integration.Identity;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class User_Activation_Tests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public User_Activation_Tests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    [FriendlyNamedFact("IT-000"), Priority(-1)]
    public async Task Seed()
    {
        var context = _serverFixture.Context;

        context.Set<Tenant>().Add(new Tenant("WAYNE INC", "Wayne Industries"));

        var admin = context.Set<User>().Add(new User("WAYNE INC", "admin", "admin@wayne-inc.com", "admin123", String.Empty, "Admin")).Entity;
        admin.Confirm();
        admin.AddClaim(context.Set<Claim>().First(x => x.Identification == AuthenticationClaims.MANAGE_USERS), ClaimAccessType.Allow);


        var user = context.Set<User>().Add(new User("WAYNE INC", "user", "user@wayne-inc.com", "user123", String.Empty, "User")).Entity;
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
    /// User cannot be deactivated when username is null or empty
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedTheory("IT-502"), Priority(10)]
    [InlineData(null)]
    [InlineData("")]
    public async Task User_cannot_be_deactivated_when_username_is_null_or_empty(string username)
    {
        // Prepare
        var request = new DeativateUser.Request
        {
            Username = username,
        };

        // Act
        var response = await _serverFixture.PostAsync("api/authentication/user/deactivate", request, await _serverFixture.GetAccessTokenAsync("admin", "admin123", "wayne inc"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The field 'User' cannot be empty");
    }

    /// <summary>
    /// User cannot deactivate itself
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-495"), Priority(20)]
    public async Task User_cannot_deactivate_itself()
    {
        // Prepare
        var request = new DeativateUser.Request
        {
            Username = "admin",
        };

        // Act
        var response = await _serverFixture.PostAsync("api/authentication/user/deactivate", request, await _serverFixture.GetAccessTokenAsync("admin", "admin123", "wayne inc"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "User cannot deactivate itself");
    }

    /// <summary>
    /// User cannot be deactivated when it does not exist
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-493"), Priority(30)]
    public async Task User_cannot_be_deactivated_when_it_does_not_exist()
    {
        // Prepare
        var request = new DeativateUser.Request
        {
            Username = "unknown-user",
        };

        // Act
        var response = await _serverFixture.PostAsync("api/authentication/user/deactivate", request, await _serverFixture.GetAccessTokenAsync("admin", "admin123", "wayne inc"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "User not found");
    }

    /// <summary>
    /// User can be deactivated
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-498"), Priority(40)]
    public async Task User_can_be_deactivated()
    {
        var user = _serverFixture.Context.Set<User>().FirstOrDefault(x => x.TenantId == "WAYNE INC" && x.Username == "user");
        user.IsActive.ShouldBeTrue();

        // Prepare
        var request = new DeativateUser.Request
        {
            Username = "user",
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/user/deactivate", request, await _serverFixture.GetAccessTokenAsync("admin", "admin123", "wayne inc"));

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        var result = _serverFixture.Context.Set<User>().FirstOrDefault(x => x.Id == user.Id);
        result.ShouldNotBeNull();
        result.IsActive.ShouldBeFalse();
    }

    /// <summary>
    /// User cannot login when it is deactivated
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-499"), Priority(50)]
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
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", request, authenticated: false);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Account is deactivated");
    }

    /// <summary>
    /// User cannot be activated when username is null or empty
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedTheory("IT-504"), Priority(60)]
    [InlineData(null)]
    [InlineData("")]
    public async Task User_cannot_be_activated_when_username_is_null_or_empty(string username)
    {
        // Prepare
        var request = new ActivateUser.Request
        {
            Username = username,
        };

        // Act
        var response = await _serverFixture.PostAsync("api/authentication/user/activate", request, await _serverFixture.GetAccessTokenAsync("admin", "admin123", "wayne inc"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The field 'User' cannot be empty");
    }

    /// <summary>
    /// User cannot be activated when it does not exist
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-500"), Priority(70)]
    public async Task User_cannot_be_activated_when_it_does_not_exist()
    {
        // Prepare
        var request = new ActivateUser.Request
        {
            Username = "unknown-user",
        };

        // Act
        var response = await _serverFixture.PostAsync("api/authentication/user/activate", request, await _serverFixture.GetAccessTokenAsync("admin", "admin123", "wayne inc"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "User not found");
    }

    /// <summary>
    /// User can be reactivated
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-501"), Priority(80)]
    public async Task User_can_be_reactivated()
    {
        var user = _serverFixture.Context.Set<User>().FirstOrDefault(x => x.TenantId == "WAYNE INC" && x.Username == "user");
        user.IsActive.ShouldBeFalse();

        // Prepare
        var request = new ActivateUser.Request
        {
            Username = "user",
        };

        // Act
        var response = await _serverFixture.PostAsync("api/authentication/user/activate", request, await _serverFixture.GetAccessTokenAsync("admin", "admin123", "wayne inc"));

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        var result = _serverFixture.Context.Set<User>().FirstOrDefault(x => x.Id == user.Id);
        result.ShouldNotBeNull();
        result.IsActive.ShouldBeTrue();
    }
}
