
using Demo1.Tests;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Identity.Core;
using rbkApiModules.Identity.Core.DataTransfer;

namespace rbkApiModules.Identity.Tests.Roles;

/// <summary>
/// In general, the global admin can only manage roles that are application wide,
/// for tenant wide roles, only the local admins are able to manage
/// </summary>
[HumanFriendlyDisplayName]
public class Application_Role_Basic_Dependent_Tests  
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
        await TestingServer.CacheCredentialsAsync("admin1", "123", "buzios");
    }

    #region tables

    /// -------------------------------------        ------------------------------------- 
    /// | ID | TENANT | NAME                |        | ID | TENANT | NAME                | 
    /// -------------------------------------   >>   ------------------------------------- 
    /// |    |        |                     |        |    | NULL   | General             |
    /// --------------- ---------------------        -------------------------------------

    #endregion
    /// <summary>
    /// The global admin should be able to create a new application wide role
    /// DEPENDENCIES: others depend on this test
    /// </summary>
    [Test, NotInParallel(Order = 2)]
    public async Task Global_Admin_Can_Create_Application_Role()
    {
        // Prepare 
        var request = new CreateRole.Request
        {
            Name = "General",
        };

        // Act
        var response = await TestingServer.PostAsync<RoleDetails>("api/authorization/roles", request, "superuser");

        // Assert the response
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.Id.ShouldNotBe(Guid.Empty);
        response.Data.Name.ShouldBe("General");
        response.Data.Claims.ShouldNotBeNull();
        response.Data.Claims.Length.ShouldBe(0);

        // Assert the database
        var role = TestingServer.CreateContext().Set<Role>()
            .Include(x => x.Claims)
            .FirstOrDefault(x => x.Id == response.Data.Id);

        role.ShouldNotBeNull();
        role.Name.ShouldBe("General");
        role.Claims.ShouldNotBeNull();
        role.Claims.Count().ShouldBe(0);
    }

    #region tables

    /// -------------------------------------    
    /// | ID | TENANT | NAME                |    
    /// -------------------------------------    
    /// |    | NULL   | General             |    
    /// --------------- ---------------------    

    #endregion
    /// <summary>
    /// A local admin should not be able to delete an application wide role
    /// DEPENDENCIES: role 'General' created on IT-014
    /// </summary>
    [Test, NotInParallel(Order = 3)]
    public async Task Local_Admin_Cannot_Delete_An_Application_Role()
    {
        // Prepare 
        var role = TestingServer.CreateContext().Set<Role>().Single(x => x.Name == "General");
        role.IsApplicationWide.ShouldBeTrue();

        // Act
        var response = await TestingServer.DeleteAsync($"api/authorization/roles/{role.Id}", "admin1");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Role not found");
    }

    #region tables

    /// -------------------------------------        ------------------------------------- 
    /// | ID | TENANT | NAME                |        | ID | TENANT | NAME                | 
    /// -------------------------------------   >>   ------------------------------------- 
    /// |    | NULL   | General             |        |    | NULL   | General User        |
    /// --------------- ---------------------        -------------------------------------

    #endregion
    /// <summary>
    /// The global admin should be able to rename an existing application wide role
    /// DEPENDENCIES: IT-014 and IT-040 ('General' role)
    /// </summary>
    [Test, NotInParallel(Order = 4)]
    public async Task Global_Admin_Can_Rename_Application_Role()
    {
        // Prepare
        var role = TestingServer.CreateContext().Set<Role>().First(x => x.Name == "General");

        var request = new RenameRole.Request
        {
            Id = role.Id,
            Name = "General User"
        };

        // Act
        var response = await TestingServer.PutAsync<RoleDetails>("api/authorization/roles", request, "superuser");

        // Assert the response
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.Id.ShouldBe(role.Id);
        response.Data.Name.ShouldBe("General User");
        response.Data.Claims.ShouldNotBeNull();
        response.Data.Claims.Length.ShouldBe(0);

        // Assert the database
        role = TestingServer.CreateContext().Set<Role>()
            .Include(x => x.Claims)
            .FirstOrDefault(x => x.Id == response.Data.Id);

        role.ShouldNotBeNull();
        role.Name.ShouldBe("General User");
        role.Claims.ShouldNotBeNull();
        role.Claims.Count().ShouldBe(0);
    }

    #region tables

    /// ------------------------------------- 
    /// | ID | TENANT | NAME                | 
    /// ------------------------------------- 
    /// |    | NULL   | General User        |
    /// -------------------------------------

    #endregion
    /// <summary>
    /// Application roles with the same name are not allowed, so we should not be able to create them
    /// DEPENDENCIES: 'General User' role, renamed on IT-015 
    /// </summary>
    [Test, NotInParallel(Order = 5)]
    public async Task Global_Admin_Cannot_Create_Application_Role_With_Duplicated_Name()
    {
        // Prepare
        var request = new CreateRole.Request
        {
            Name = "GENERAL USER",
        };

        // Act
        var response = await TestingServer.PostAsync<RoleDetails>("api/authorization/roles", request, "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Name already used");
    }

    #region tables

    /// ------------------------------------- 
    /// | ID | TENANT | NAME                | 
    /// ------------------------------------- 
    /// |    | NULL   | General User        |
    /// -------------------------------------

    #endregion
    /// <summary>
    /// Local admins are only allowed to edit tenant wide roles
    /// DEPENDENCIES: 'General User' role, renamed on IT-015 
    /// </summary>
    [Test, NotInParallel(Order = 6)]
    public async Task Local_Admin_Cannot_Rename_Application_Role()
    {
        // Prepare
        var preExistingRole = TestingServer.CreateContext().Set<Role>().Single(x => x.Name == "General User");
        preExistingRole.ShouldNotBeNull();

        var request = new RenameRole.Request
        {
            Name = "RENAMED GENERAL USER",
        };

        // Act
        var response = await TestingServer.PutAsync<RoleDetails>("api/authorization/roles", request, "admin1");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Role not found");
    }

    #region tables

    /// -------------------------------------        ------------------------------------- 
    /// | ID | TENANT | NAME                |        | ID | TENANT | NAME                | 
    /// -------------------------------------   >>   ------------------------------------- 
    /// |    | NULL   | General             |        |    | NULL   | General User        |
    /// |    |        |                     |        |    | BUZIOS | Tenant Role         |
    /// --------------- ---------------------        -------------------------------------

    #endregion
    /// <summary>
    /// Global admin can see a list of application roles only
    /// DEPENDENCIES: 'General User' role, renamed on IT-015 
    /// </summary>
    [Test, NotInParallel(Order = 7)]
    public async Task Global_Admin_Can_Query_Application_Roles()
    {
        // Prepare
        var body = new CreateRole.Request
        {
            Name = "Tenant Role"
        };
        var preResponse = await TestingServer.PostAsync<RoleDetails>("api/authorization/roles", body, "admin1");
        preResponse.ShouldBeSuccess();

        // Act
        var response = await TestingServer.GetAsync<RoleDetails[]>("api/authorization/roles", "superuser");

        // Assert the response
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.Length.ShouldBeGreaterThan(0);

        // Find the entity created within these tests and asset it
        var role1 = response.Data.SingleOrDefault(x => x.Name == "General User");
        role1.ShouldNotBeNull();
        role1.Source.ShouldBe(RoleSource.Global);

        // Should not return any tenant specific role, like the one we created in the preparation phase
        var role2 = response.Data.SingleOrDefault(x => x.Name == "Tenant Role");
        role2.ShouldBeNull();
    }

    #region tables

    /// ------------------------------------- 
    /// | ID | TENANT | NAME                | 
    /// ------------------------------------- 
    /// |    | NULL   | General User        |
    /// |    | BUZIOS | Tenant Role         |
    /// -------------------------------------

    #endregion
    /// <summary>
    /// The global admin should have validation error if application role name is empty or null
    /// DEPENDENCIES: 'General User' role, renamed on IT-015 
    /// </summary>
    [Test, NotInParallel(Order = 8)]
    [Arguments(null)]
    [Arguments("")]
    public async Task Global_Admin_Cannot_Rename_Role_With_Empty_Or_Null_Name(string? name)
    {
        // Prepare
        var role = TestingServer.CreateContext().Set<Role>().First(x => x.Name == "General User");

        var request = new RenameRole.Request
        {
            Id = role.Id,
            Name = name!,
        };

        // Act
        var response = await TestingServer.PutAsync<RoleDetails>("api/authorization/roles", request, "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "'Name' must not be empty.");
    }

    #region tables

    /// -------------------------------------        ------------------------------------- 
    /// | ID | TENANT | NAME                |        | ID | TENANT | NAME                | 
    /// -------------------------------------   >>   ------------------------------------- 
    /// |    | NULL   | General             |        |    | NULL   | General User        |
    /// |    | BUZIOS | Tenant Role         |        |    | BUZIOS | Tenant Role         |
    /// |    |        |                     |        |    | NULL   | Tenant Role         |
    /// -------------------------------------        -------------------------------------

    #endregion
    /// <summary>
    /// Global admin can create a role even if the name is already used in any tenants
    /// DEPENDENCIES: the 'Tenant Role' created on IT-019 ***
    /// </summary>
    [Test, NotInParallel(Order = 9)]
    public async Task Global_Admin_Can_Create_Application_Role_Even_If_Exists_In_a_Tenant()
    {
        // Prepare 
        var preExistingRole = TestingServer.CreateContext().Set<Role>().First(x => x.Name == "Tenant Role");

        var request = new CreateRole.Request
        {
            Name = "Tenant Role",
        };

        // Act
        var response = await TestingServer.PostAsync<RoleDetails>("api/authorization/roles", request, "superuser");

        // Assert the response
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.Id.ShouldNotBe(Guid.Empty);
        response.Data.Id.ShouldNotBe(preExistingRole.Id);
        response.Data.Name.ShouldBe("Tenant Role");
        response.Data.Source.ShouldBe(RoleSource.Global);
        response.Data.Claims.ShouldNotBeNull();
        response.Data.Claims.Length.ShouldBe(0);

        // Assert the database
        var roles = TestingServer.CreateContext().Set<Role>()
            .Include(x => x.Claims)
            .Where(x => x.Name == "Tenant Role")
            .ToList();

        roles.Count.ShouldBe(2);
    }

    #region tables

    /// -------------------------------------  
    /// | ID | TENANT | NAME                |  
    /// -------------------------------------  
    /// |    | NULL   | General User        |  
    /// |    | BUZIOS | Tenant Role         |  
    /// |    | NULL   | Tenant Role         |  
    /// -------------------------------------  

    #endregion
    /// <summary>
    /// Global admin cannot rename an application role, if the name is already used by another application role, even with different casing
    /// DEPENDENCIES: the 'General User' renamed on IT-015 and 'Tenant Role' created on IT-037
    /// </summary>
    [Test, NotInParallel(Order = 10)]
    public async Task Global_Admin_Cannot_Rename_Application_Role_If_Name_Already_Exist_Even_With_Different_Casing()
    {
        // Prepare
        var role1 = TestingServer.CreateContext().Set<Role>().Single(x => x.Name == "General User" && x.TenantId == null);
        var role2 = TestingServer.CreateContext().Set<Role>().Single(x => x.Name == "Tenant Role" && x.TenantId == null);

        var renameCommand = new RenameRole.Request
        {
            Id = role2.Id,
            Name = "GENERAL USER"
        };

        // Act
        var response = await TestingServer.PutAsync<RoleDetails>($"api/authorization/roles", renameCommand, "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Name already used");

        // Assert the database
        var check = TestingServer.CreateContext().Set<Role>().SingleOrDefault(x => x.Id == role2.Id);
        check.ShouldNotBeNull();
        check.Name.ShouldBe("Tenant Role");
    }

    #region tables

    /// -------------------------------------        ------------------------------------- 
    /// | ID | TENANT | NAME                |        | ID | TENANT | NAME                | 
    /// -------------------------------------   >>   ------------------------------------- 
    /// |    | NULL   | General User        |        |    |        |                     |
    /// |    | BUZIOS | Tenant Role         |        |    | BUZIOS | Tenant Role         |
    /// |    | NULL   | Tenant Role         |        |    | NULL   | Tenant Role         |
    /// -------------------------------------        -------------------------------------

    #endregion
    /// <summary>
    /// Global admin can delete application roles when they are not used yet
    /// DEPENDENCIES: 'General User' role, renamed on IT-015 
    /// </summary>
    [Test, NotInParallel(Order = 11)]
    public async Task Global_Admin_Can_Delete_Application_Roles()
    {
        // Prepare
        var role = TestingServer.CreateContext().Set<Role>().First(x => x.Name == "General User");

        // Act
        var response = await TestingServer.DeleteAsync($"api/authorization/roles/{role.Id}", "superuser");

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        var check = TestingServer.CreateContext().Set<Role>().FirstOrDefault(x => x.Id == role.Id);

        check.ShouldBeNull();
    }

    #region tables

    /// -------------------------------------        -------------------------------------       -------------------------------------
    /// | ID | TENANT | NAME                |        | ID | TENANT | NAME                |       | ID | TENANT | NAME                |
    /// -------------------------------------        -------------------------------------       -------------------------------------
    /// |    | NULL   | General             |   >>   |    |        |                     |  >>   |    | NULL   | General             |
    /// |    | BUZIOS | Tenant Role         |        |    | BUZIOS | Tenant Role         |       |    | BUZIOS | Tenant Role         |
    /// |    | NULL   | Tenant Role         |        |    | NULL   | Renamed Tenant Role |       |    | NULL   | Tenant Role         |
    /// -------------------------------------        -------------------------------------       -------------------------------------

    #endregion
    /// <summary>
    /// Global admin can rename an application role, even if it exists and a tenant role
    /// DEPENDENCIES: the 'Tenant Role' created on IT-019 ***
    /// </summary>
    [Test, NotInParallel(Order = 12)]
    public async Task Global_Admin_Can_Rename_Application_Role_Even_If_Exists_As_Tenant_Role()
    {
        // Prepare
        var applicationRole = TestingServer.CreateContext().Set<Role>().Single(x => x.Name == "Tenant Role" && x.TenantId == null);
        var tenantRole = TestingServer.CreateContext().Set<Role>().Single(x => x.Name == "Tenant Role" && x.TenantId == "BUZIOS");

        // *** Test 1 *** Rename to something else
        {
            var renameCommand = new RenameRole.Request
            {
                Id = applicationRole.Id,
                Name = "Renamed Tenant Role"
            };

            // Act
            var response = await TestingServer.PutAsync<RoleDetails>($"api/authorization/roles", renameCommand, "superuser");

            // Assert the response
            response.ShouldBeSuccess();

            // Assert the database
            var check = TestingServer.CreateContext().Set<Role>().SingleOrDefault(x => x.Id == applicationRole.Id);
            check.ShouldNotBeNull();
            check.Name.ShouldBe("Renamed Tenant Role");
        }

        // *** Test 2 *** Rename back to the original name
        {
            var renameCommand = new RenameRole.Request
            {
                Id = applicationRole.Id,
                Name = "Tenant Role"
            };

            // Act
            var response = await TestingServer.PutAsync<RoleDetails>($"api/authorization/roles", renameCommand, "superuser");

            // Assert the response
            response.ShouldBeSuccess();

            // Assert the database
            var check = TestingServer.CreateContext().Set<Role>().SingleOrDefault(x => x.Id == tenantRole.Id);
            check.ShouldNotBeNull();
            check.Name.ShouldBe("Tenant Role");
        }
    }

    #region tables

    /// ------------------------------------- 
    /// | ID | TENANT | NAME                | 
    /// ------------------------------------- 
    /// |    | BUZIOS | Tenant Role         |
    /// |    | NULL   | Tenant Role         |
    /// -------------------------------------

    #endregion
    /// <summary>
    /// Global admin cannot delete tenant roles 
    /// DEPENDENCIES: 'Tenant Role' roles, created on IT-019 and IT-037 (one is application wide, other is tenant wide)
    /// </summary>
    [Test, NotInParallel(Order = 13)]
    public async Task Global_Admin_Cannot_Delete_Tenant_Roles()
    {
        // Prepare
        var applicationRole = TestingServer.CreateContext().Set<Role>().Single(x => x.Name == "Tenant Role" && x.TenantId == null);
        var tenantRole = TestingServer.CreateContext().Set<Role>().Single(x => x.Name == "Tenant Role" && x.TenantId == "BUZIOS");

        // Act
        var response = await TestingServer.DeleteAsync($"api/authorization/roles/{tenantRole.Id}", "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Role not found");

        // Assert the database
        tenantRole = TestingServer.CreateContext().Set<Role>().SingleOrDefault(x => x.Name == "Tenant Role" && x.TenantId == null);
        applicationRole = TestingServer.CreateContext().Set<Role>().SingleOrDefault(x => x.Name == "Tenant Role" && x.TenantId == "BUZIOS");

        applicationRole.ShouldNotBeNull();
        tenantRole.ShouldNotBeNull();
    }

    #region tables

    /// -------------------------------------        ------------------------------------- 
    /// | ID | TENANT | NAME                |        | ID | TENANT | NAME                | 
    /// -------------------------------------   >>   ------------------------------------- 
    /// |    | BUZIOS | Tenant Role         |        |    | BUZIOS | Tenant Role         |
    /// |    | NULL   | Tenant Role         |        |    |        |                     |
    /// -------------------------------------        -------------------------------------

    #endregion
    /// <summary>
    /// Global admin can delete application roles even if they exist as tenant roles
    /// DEPENDENCIES: 'Tenant Role' roles, created on IT-019 and IT-037 (one is application wide, other is tenant wide)
    /// </summary>
    [Test, NotInParallel(Order = 14)]
    public async Task Global_Admin_Can_Delete_Application_Role_That_Already_Exist_As_A_Tenant_Role()
    {
        // Prepare
        var applicationRole = TestingServer.CreateContext().Set<Role>().Single(x => x.Name == "Tenant Role" && x.TenantId == null);
        var tenantRole = TestingServer.CreateContext().Set<Role>().Single(x => x.Name == "Tenant Role" && x.TenantId == "BUZIOS");

        // Act
        var response = await TestingServer.DeleteAsync($"api/authorization/roles/{applicationRole.Id}", "superuser");

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        applicationRole = TestingServer.CreateContext().Set<Role>().SingleOrDefault(x => x.Name == "Tenant Role" && x.TenantId == null);
        tenantRole = TestingServer.CreateContext().Set<Role>().SingleOrDefault(x => x.Name == "Tenant Role" && x.TenantId == "BUZIOS");

        applicationRole.ShouldBeNull();
        tenantRole.ShouldNotBeNull();
        tenantRole.IsApplicationWide.ShouldBe(false);
        tenantRole.TenantId.ShouldBe("BUZIOS");
    }

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }
}