using Demo1.Tests;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Core;
using rbkApiModules.Identity.Core;
using rbkApiModules.Identity.Core.DataTransfer.Users;

namespace rbkApiModules.Identity.Tests.Users;

[HumanFriendlyDisplayName]
public class User_Creation_Tests
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

        var role1 = context.Set<Role>().Add(new Role("WAYNE INC", "Role1")).Entity;

        context.SaveChanges();

        // Default user for all tests
        await TestingServer.CacheCredentialsAsync("admin", "admin123", "wayne inc");
    }

    /// <summary>
    /// User cannot be created if password is not supplied
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 2)]
    [Arguments(null)]
    [Arguments("")]
    public async Task User_cannot_be_created_when_password_is_null_or_empty(string? password)
    {
        var role1 = TestingServer.CreateContext().Set<Role>().First(x => x.Name == "Role1");

        // Prepare
        var request = new CreateUser.Request
        {
            Username = "new-user",
            DisplayName = "John Doe",
            Email = "new-user@wayne-inc.com",
            RoleIds = new[] { role1.Id },
            Password = password!,
            Metadata = new Dictionary<string, string>
            {
                { "sector", "Research" },
                { "age", "18" }
            }
        };

        // Act
        var response = await TestingServer.PostAsync<UserDetails>("api/authentication/users/create", request, "admin");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Password is required");
    }

    /// <summary>
    /// User cannot be created if passwords do not metch
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 3)]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("123456")]
    public async Task User_cannot_be_created_when_passwords_do_not_match(string? password)
    {
        var role1 = TestingServer.CreateContext().Set<Role>().First(x => x.Name == "Role1");

        // Prepare
        var request = new CreateUser.Request
        {
            Username = "new-user",
            DisplayName = "John Doe",
            Email = "new-user@wayne-inc.com",
            RoleIds = new[] { role1.Id },
            Password = "abc123",
            PasswordConfirmation = password!,
            Metadata = new Dictionary<string, string>
            {
                { "sector", "Research" },
                { "age", "18" }
            }
        };

        // Act
        var response = await TestingServer.PostAsync<UserDetails>("api/authentication/users/create", request, "admin");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Password and confirmation must match");
    }

    /// <summary>
    /// User cannot be created if custom password policies are not met
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 4)]
    public async Task User_cannot_be_created_when_password_policies_are_not_met()
    {
        var role1 = TestingServer.CreateContext().Set<Role>().First(x => x.Name == "Role1");

        // Prepare
        var request = new CreateUser.Request
        {
            Username = "new-user",
            DisplayName = "John Doe",
            Email = "new-user@wayne-inc.com",
            RoleIds = new[] { role1.Id },
            Password = "a",
            PasswordConfirmation = "a",
            Metadata = new Dictionary<string, string>
            {
                { "sector", "Research" },
                { "age", "18" }
            }
        };

        // Act
        var response = await TestingServer.PostAsync<UserDetails>("api/authentication/users/create", request, "admin");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The password must have at least 3 characteres");
    }

    /// <summary>
    /// User can be created
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 5)]
    public async Task User_can_be_created_without_picture()
    {
        var role1 = TestingServer.CreateContext().Set<Role>().First(x => x.Name == "Role1");

        // Prepare
        var request = new CreateUser.Request
        {
            Username = "new-user",
            DisplayName = "John Doe",
            Email = "new-user@wayne-inc.com",
            RoleIds = new[] { role1.Id },
            Password = "admin123",
            PasswordConfirmation = "admin123",
            Metadata = new Dictionary<string, string>
            {
                { "sector", "Research" },
                { "age", "18" }
            }
        };

        // Act
        var response = await TestingServer.PostAsync<UserDetails>("api/authentication/users/create", request, "admin");

        // Assert the response
        response.ShouldBeSuccess();
        response.Data.ShouldNotBeNull();
        response.Data.ShouldBeOfType<UserDetails>();
        response.Data.DisplayName.ShouldBe("John Doe");
        response.Data.Username.ShouldBe("new-user");
        response.Data.Email.ShouldBe("new-user@wayne-inc.com");
        response.Data.Avatar.ShouldNotBeNull();
        response.Data.IsConfirmed.ShouldBeTrue();
        response.Data.Roles.Length.ShouldBe(1);
        response.Data.Claims.Length.ShouldBe(0);
        response.Data.OverridedClaims.Length.ShouldBe(0);

        // Assert the database
        var user = TestingServer.CreateContext().Set<User>()
            .Include(x => x.Claims)
            .Include(x => x.Roles)
            .First(x => x.Username == "new-user" && x.TenantId == "WAYNE INC"); ;

        user.Password.ShouldNotBeNull();

        PasswordHasher.VerifyPassword("admin123", user.Password).ShouldBeTrue();
        user.ActivationCode.ShouldBeNull();
        user.Avatar.ShouldNotBeNull();
        user.Claims.Count().ShouldBe(0);
        user.DisplayName.ShouldBe("John Doe");
        user.Email.ShouldBe("new-user@wayne-inc.com");
        user.HasTenant.ShouldBeTrue();
        user.IsActive.ShouldBeTrue();
        user.IsConfirmed.ShouldBeTrue();
        user.Metadata.ShouldNotBeNull();
        user.Metadata["sector"].ShouldBe("Research");
        user.Metadata["age"].ShouldBe("18");
        user.PasswordRedefineCode.ShouldBeNull();
        user.TenantId.ShouldBe("WAYNE INC");
        user.Username.ShouldBe("new-user");
        user.Roles.Count().ShouldBe(1);
        user.AuthenticationMode.ShouldBe(AuthenticationMode.Credentials);
        (DateTime.UtcNow - user.CreationDate).TotalSeconds.ShouldBeLessThan(5);
    }

    /// <summary>
    /// User can be deleted
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 6)]
    public async Task User_can_be_deleted()
    {
        var role1 = TestingServer.CreateContext().Set<Role>().First(x => x.Name == "Role1");

        // Prepare
        var request = new DeleteUser.Request
        {
            Username = "new-user" 
        };

        // Act
        var response = await TestingServer.PostAsync("api/authentication/users/delete", request, "admin");

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        var user = TestingServer.CreateContext().Set<User>()
            .Include(x => x.Claims)
            .Include(x => x.Roles)
            .FirstOrDefault(x => x.Username == "new-user" && x.TenantId == "WAYNE INC");

        user.ShouldBeNull();
    }

    /// <summary>
    /// User can be created with url for the avatar
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 7)]
    public async Task User_can_be_created_with_url_picture()
    {
        var role1 = TestingServer.CreateContext().Set<Role>().First(x => x.Name == "Role1");

        // Prepare
        var request = new CreateUser.Request
        {
            Username = "new-user",
            DisplayName = "John Doe",
            Email = "new-user@wayne-inc.com",
            RoleIds = new[] { role1.Id },
            Password = "admin123",
            PasswordConfirmation = "admin123",
            Picture = "https://my-domain.com/avatar/new-user.png",
            Metadata = new Dictionary<string, string>
            {
                { "sector", "Research" },
                { "age", "18" }
            }
        };

        // Act
        var response = await TestingServer.PostAsync<UserDetails>("api/authentication/users/create", request, "admin");

        // Assert the response
        response.ShouldBeSuccess();
        response.Data.ShouldNotBeNull();
        response.Data.ShouldBeOfType<UserDetails>();
        response.Data.DisplayName.ShouldBe("John Doe");
        response.Data.Username.ShouldBe("new-user");
        response.Data.Email.ShouldBe("new-user@wayne-inc.com");
        response.Data.Avatar.ShouldNotBeNull();
        response.Data.IsConfirmed.ShouldBeTrue();
        response.Data.Roles.Length.ShouldBe(1);
        response.Data.Claims.Length.ShouldBe(0);
        response.Data.OverridedClaims.Length.ShouldBe(0);

        // Assert the database
        var context = TestingServer.CreateContext();
        
        var user = context.Set<User>()
            .Include(x => x.Claims)
            .Include(x => x.Roles)
            .First(x => x.Username == "new-user" && x.TenantId == "WAYNE INC"); 

        user.Password.ShouldNotBeNull();

        PasswordHasher.VerifyPassword("admin123", user.Password).ShouldBeTrue();
        user.ActivationCode.ShouldBeNull();
        user.Avatar.ShouldNotBeNull();
        user.Claims.Count().ShouldBe(0);
        user.DisplayName.ShouldBe("John Doe");
        user.Avatar.ShouldBe("https://my-domain.com/avatar/new-user.png");
        user.Email.ShouldBe("new-user@wayne-inc.com");
        user.HasTenant.ShouldBeTrue();
        user.IsActive.ShouldBeTrue();
        user.IsConfirmed.ShouldBeTrue();
        user.Metadata.ShouldNotBeNull();
        user.Metadata["sector"].ShouldBe("Research");
        user.Metadata["age"].ShouldBe("18");
        user.PasswordRedefineCode.ShouldBeNull();
        user.TenantId.ShouldBe("WAYNE INC");
        user.Username.ShouldBe("new-user");
        user.Roles.Count().ShouldBe(1);
        user.AuthenticationMode.ShouldBe(AuthenticationMode.Credentials);
        (DateTime.UtcNow - user.CreationDate).TotalSeconds.ShouldBeLessThan(5);

        context.RemoveRange(user.Roles);
        context.RemoveRange(user.Claims);
        context.Remove(user);
        context.SaveChanges();
    }

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }
}
