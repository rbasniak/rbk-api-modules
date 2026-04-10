
using Demo1.Tests;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Identity.Core;
using rbkApiModules.Identity.Core.DataTransfer;

namespace rbkApiModules.Identity.Tests.Roles;

[HumanFriendlyDisplayName]
public class Application_Role_Basic_Independent_Tests
{
    [ClassDataSource<Demo1TestingServer>(Shared = SharedType.PerClass)]
    public required Demo1TestingServer TestingServer { get; set; } = default!;

    /// <summary>
    /// Login call, just to store the access tokens for future tests to use
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 1)]
    public async Task Login()
    {
        await TestingServer.CacheCredentialsAsync("superuser", "admin", null);
    }

    /// <summary>
    /// The global admin should not be able to update a role that does not exist
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 2)]
    public async Task Global_Admin_Cannot_Rename_Role_That_Does_Not_Exist()
    {
        // Prepare
        var request = new RenameRole.Request
        {
            Id = Guid.NewGuid(),
            Name = "New fake name",
        };

        // Act
        var response = await TestingServer.PutAsync<RoleDetails>("api/authorization/roles", request, "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Role not found");
    }

    /// <summary>
    /// A global admin cannot delete an application role that doesn't exist
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 3)]
    public async Task Global_Admin_Cannot_Delete_An_Application_Role_That_Doesnt_Exist()
    {
        // Act
        var response = await TestingServer.DeleteAsync($"api/authorization/roles/{Guid.NewGuid()}", "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Role not found");
    }

    /// <summary>
    /// The global admin should not be able to create a new application wide role without a proper name
    /// DEPENDENCIES: none
    /// </summary>
    [Arguments(null)]
    [Arguments("")]
    [Test, NotInParallel(Order = 4)]
    public async Task Global_Admin_Cannot_Create_Application_Role_Without_Name(string? name)
    {
        // Prepare 
        var request = new CreateRole.Request
        {
            Name = name!,
        };

        // Act
        var response = await TestingServer.PostAsync<RoleDetails>("api/authorization/roles", request, "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "'Name' must not be empty.");
    }

    /// <summary>
    /// The global admin should not be able to delete an application role that is being used by an user
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 5)]
    public async Task Global_Admin_Cannot_Delete_Application_Role_Associated_With_User()
    {
        // Prepare 
        var context = TestingServer.CreateContext();

        var role = context.Add(new Role("Test Role 99")).Entity;

        var user = context.Set<User>().Include(x => x.Roles).First(x => x.Username == "admin1" && x.TenantId == "BUZIOS");
        user.AddRole(role);

        context.SaveChanges();

        role = TestingServer.CreateContext().Set<Role>().Include(x => x.Users).First(x => x.Id == role.Id);
        role.Users.Count().ShouldBe(1);

        // Act
        var response = await TestingServer.DeleteAsync($"api/authorization/roles/{role.Id}", "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Role associated with one or more users");
    }

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }

}