using Demo1.Tests;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Identity.Core;
using rbkApiModules.Identity.Core.DataTransfer.Users;

namespace rbkApiModules.Identity.Tests.Users;

[NotInParallel(nameof(User_Management_Tests))]
public class User_Management_Tests
{
    [ClassDataSource<Demo1TestingServer>(Shared = SharedType.PerClass)]
    public required Demo1TestingServer TestingServer { get; set; } = default!;


    private static Dictionary<string, JwtToken> _tokens = new();

    /// <summary>
    /// Login call, just to store the access tokens for future tests to use
    /// </summary>
    [Test, NotInParallel(Order = 1)]
    public async Task Seed()
    {
        // To register the LastLogin property
        _tokens.Add("john.doe", new JwtToken((await TestingServer.LoginAsync("john.doe", "123", "buzios")).Data!.AccessToken));
        _tokens.Add("jane.doe", new JwtToken((await TestingServer.LoginAsync("jane.doe", "123", "buzios")).Data!.AccessToken));
        _tokens.Add("admin1-bz", new JwtToken((await TestingServer.LoginAsync("admin1", "123", "buzios")).Data!.AccessToken));
        _tokens.Add("admin1-bs", new JwtToken((await TestingServer.LoginAsync("admin1", "123", "un-bs")).Data!.AccessToken));
        
        await TestingServer.CacheCredentialsAsync("superuser", "admin", null);
    }

    /// <summary>
    /// The global admin should not be able to see the list of users, only tenant admins can see their own users
    /// </summary>
    [Test, NotInParallel(Order = 2)]
    public async Task Global_Admin_Cannot_View_List_Of_Users()
    {
        // Act
        var response = await TestingServer.GetAsync<UserDetails>("api/authorization/users", "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// A local admin should be able to see the list of their users 
    /// DEPENDENCIES: Seed data
    /// </summary>
    [Test, NotInParallel(Order = 3)]
    public async Task Local_Admin_Cannot_View_List_Of_Users()
    {
        // Act
        var response = await TestingServer.GetAsync<UserDetails[]>("api/authorization/users", _tokens["admin1-bz"]);

        // Assert the response
        response.ShouldBeSuccess(out var result);

        var userJohn = result.FirstOrDefault(x => x.Username == "john.doe");
        var userJane = result.FirstOrDefault(x => x.Username == "jane.doe");

        userJohn.ShouldNotBeNull();
        userJohn.Roles.Count().ShouldBe(1);
        userJohn.LastLogin.ShouldNotBeNull();
        userJohn.Roles[0].Name.ShouldBe("Employee");

        userJane.ShouldNotBeNull();
        userJane.LastLogin.ShouldNotBeNull();
        userJane.Roles.Count().ShouldBe(2);
        userJane.Roles.FirstOrDefault(x => x.Name == "Employee").ShouldNotBeNull();
        userJane.Roles.FirstOrDefault(x => x.Name == "Manager").ShouldNotBeNull();
    }

    /// <summary>
    /// A local admin should be able to set the user's list of roles
    /// DEPENDENCIES: Seed data
    /// </summary>
    [Test, NotInParallel(Order = 4)]
    public async Task Local_Admin_Can_Set_List_Of_Roles()
    {
        // Prepare
        var user = TestingServer.CreateContext().Set<User>().Include(x => x.Roles).SingleOrDefault(x => x.Username == "tony.stark" && x.TenantId == "UN-BS");
        user.ShouldNotBeNull();
        user.Roles.ToList().Count().ShouldBe(0);

        var role1 = TestingServer.CreateContext().Set<Role>().Single(x => x.Name == "Employee" && x.TenantId == null);
        var role2 = TestingServer.CreateContext().Set<Role>().Single(x => x.Name == "Manager" && x.TenantId == null);

        var request = new ReplaceUserRoles.Request 
        { 
            RoleIds = new[] { role1.Id, role2.Id } ,
            Username = user.Username
        };

        // Act
        var response = await TestingServer.PostAsync<UserDetails>("api/authorization/users/set-roles", request, _tokens["admin1-bs"]);

        // Assert the response
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.Roles.Length.ShouldBe(2);
        response.Data.Roles.SingleOrDefault(x => x.Id == role1.Id).ShouldNotBeNull();
        response.Data.Roles.SingleOrDefault(x => x.Id == role2.Id).ShouldNotBeNull();

        // Assert the database
        user = TestingServer.CreateContext().Set<User>().Include(x => x.Roles).SingleOrDefault(x => x.Username == "tony.stark" && x.TenantId == "UN-BS");
        user.ShouldNotBeNull();
        user.Roles.ToList().Count().ShouldBe(2);
        user.Roles.SingleOrDefault(x => x.RoleId == role1.Id).ShouldNotBeNull();
        user.Roles.SingleOrDefault(x => x.RoleId == role2.Id).ShouldNotBeNull();
    }

    /// <summary>
    /// A local admin should be able to set the user's list of roles using an empty array, thus removing all user's access
    /// DEPENDENCIES: IT-170
    /// </summary>
    [Test, NotInParallel(Order = 5)]
    public async Task Local_Admin_Can_Remove_All_Roles_From_User()
    {
        // Prepare
        var user = TestingServer.CreateContext().Set<User>().Include(x => x.Roles).SingleOrDefault(x => x.Username == "tony.stark" && x.TenantId == "UN-BS");
        user.ShouldNotBeNull();
        user.Roles.ToList().Count().ShouldBe(2);

        var request = new ReplaceUserRoles.Request
        {
            RoleIds = new Guid[0],
            Username = user.Username
        };

        // Act
        var response = await TestingServer.PostAsync<UserDetails>("api/authorization/users/set-roles", request, _tokens["admin1-bs"]);

        // Assert the response
        response.ShouldBeSuccess(); 

        response.Data.ShouldNotBeNull();
        response.Data.Roles.Length.ShouldBe(0);

        // Assert the database
        user = TestingServer.CreateContext().Set<User>().Include(x => x.Roles).SingleOrDefault(x => x.Username == "tony.stark" && x.TenantId == "UN-BS");
        user.ShouldNotBeNull();
        user.Roles.ToList().Count().ShouldBe(0);
    }

    /// <summary>
    /// A local admin should not be able to set the user's list of roles without user
    /// DEPENDENCIES: Seed data
    /// </summary>
    [Test, NotInParallel(Order = 6)]
    [Arguments(null)]
    [Arguments("")]
    public async Task Local_Admin_Cannot_Set_List_Of_Roles_With_Empty_User(string? username)
    {
        // Prepare  
        var role1 = TestingServer.CreateContext().Set<Role>().Single(x => x.Name == "Employee" && x.TenantId == null);
        var role2 = TestingServer.CreateContext().Set<Role>().Single(x => x.Name == "Manager" && x.TenantId == null);

        var request = new ReplaceUserRoles.Request
        {
            RoleIds = new[] { role1.Id, role2.Id },
            Username = username!
        };

        // Act
        var response = await TestingServer.PostAsync<UserDetails>("api/authorization/users/set-roles", request, _tokens["admin1-bs"]);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "'Username' must not be empty.");
    }

    /// <summary>
    /// A local admin should not be able to set the user's list of roles if the user does not exist
    /// DEPENDENCIES: Seed data
    /// </summary>
    [Test, NotInParallel(Order = 7)]
    public async Task Local_Admin_Cannot_Set_List_Of_Roles_With_Non_Existant_User()
    {
        // Prepare
        var role1 = TestingServer.CreateContext().Set<Role>().Single(x => x.Name == "Employee" && x.TenantId == null);
        var role2 = TestingServer.CreateContext().Set<Role>().Single(x => x.Name == "Manager" && x.TenantId == null);

        var request = new ReplaceUserRoles.Request
        {
            RoleIds = new[] { role1.Id, role2.Id },
            Username = "tony.stark"
        };

        // Act
        var response = await TestingServer.PostAsync<UserDetails>("api/authorization/users/set-roles", request, _tokens["admin1-bz"]);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "User not found");
    }

    /// <summary>
    /// A local admin should be able to set the user's list of roles using invalid roles
    /// DEPENDENCIES: Seed data
    /// </summary>
    [Test, NotInParallel(Order = 8)]
    public async Task Local_Admin_Cannot_Set_List_Of_Roles_With_Invalid_Roles()
    {
        // Prepare
        var user = TestingServer.CreateContext().Set<User>().Include(x => x.Roles).SingleOrDefault(x => x.Username == "tony.stark" && x.TenantId == "UN-BS");
        user.ShouldNotBeNull();
        user.Roles.ToList().Count().ShouldBe(0);

        var request = new ReplaceUserRoles.Request
        {
            RoleIds = new[] { Guid.NewGuid() },
            Username = user.Username
        };

        // Act
        var response = await TestingServer.PostAsync<UserDetails>("api/authorization/users/set-roles", request, _tokens["admin1-bs"]);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Role not found");
    }

    /// <summary>
    /// A local admin should not be able to set the user's roles with a null list
    /// DEPENDENCIES: Seed data
    /// </summary>
    [Test, NotInParallel(Order = 9)]
    public async Task Local_Admin_Cannot_Set_List_Of_Roles_With_Null_List()
    {
        // Prepare
        var user = TestingServer.CreateContext().Set<User>().Include(x => x.Roles).SingleOrDefault(x => x.Username == "tony.stark" && x.TenantId == "UN-BS");
        user.ShouldNotBeNull();
        user.Roles.ToList().Count().ShouldBe(0);

        var request = new ReplaceUserRoles.Request
        {
            RoleIds = null!,
            Username = user.Username
        };

        // Act
        var response = await TestingServer.PostAsync<UserDetails>("api/authorization/users/set-roles", request, _tokens["admin1-bs"]);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The list of roles must have at least one item");
    }

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }
}
