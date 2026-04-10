using Demo1.Tests;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Identity.Core;
using rbkApiModules.Identity.Core.DataTransfer;

namespace rbkApiModules.Identity.Tests.Roles;

[HumanFriendlyDisplayName]
public class Tenant_Roles_Basic_Independent_Tests
{
    [ClassDataSource<Demo1TestingServer>(Shared = SharedType.PerClass)]
    public required Demo1TestingServer TestingServer { get; set; } = default!;

    [Test, NotInParallel(Order = 1)]
    public async Task Seed()
    {
        await TestingServer.CacheCredentialsAsync("admin1", "123", "buzios");
    }

    /// <summary>
    /// The local admin should not be able to create a new tenant wide role wihtout a proper name
    /// *** DEPENDENCIES: none
    /// </summary>
    [Arguments(null)]
    [Arguments("")]
    [Test, NotInParallel(Order = 2)]
    public async Task Local_Admin_Cannot_Create_Tenant_Role_Without_Proper_Name(string name)
    {
        // Prepare 
        var request = new CreateRole.Request
        {
            Name = name,
        };

        // Act
        var response = await TestingServer.PostAsync<RoleDetails>("api/authorization/roles", request, "admin1");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "'Name' must not be empty.");
    }

    /// <summary>
    /// The local admin should not be able to update a role that does not exist
    /// *** DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 3)]
    public async Task Local_Admin_Cannot_Rename_Role_That_Does_Not_Exist()
    {
        // Prepare
        var request = new RenameRole.Request
        {
            Id = Guid.NewGuid(),
            Name = "New fake name",
        };

        // Act
        var response = await TestingServer.PutAsync<RoleDetails>("api/authorization/roles", request, "admin1");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Role not found");
    }

    /// <summary>
    /// A local admin should not be able to delete a tenant role that is being used by an user, when
    /// there is no application role to replace it
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 4)]
    public async Task Local_Admin_Cannot_Delete_Tenant_Role_Associated_With_User_When_There_Is_No_Replacement_Application_Role()
    {
        // Prepare 
        var context = TestingServer.CreateContext();

        var tenantRole = context.Add(new Role("BUZIOS", "Test Role 99")).Entity;

        var user = context.Set<User>().Include(x => x.Roles).First(x => x.Username == "admin1" && x.TenantId == "BUZIOS");
        user.AddRole(tenantRole);

        context.SaveChanges();

        tenantRole = TestingServer.CreateContext().Set<Role>().Include(x => x.Users).First(x => x.Id == tenantRole.Id);
        tenantRole.Users.Count().ShouldBe(1);

        // Act
        var response = await TestingServer.DeleteAsync($"api/authorization/roles/{tenantRole.Id}", "admin1");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Role associated with one or more users");
    }


    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }
}


