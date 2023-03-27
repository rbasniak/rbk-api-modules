using rbkApiModules.Identity.Core.DataTransfer.Roles;

namespace rbkApiModules.Demo1.Tests.Integration.Identity;

/// <summary>
/// In general, the global admin can only manage roles that are application wide,
/// for tenant wide roles, only the local admins are able to manage
/// </summary>
[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class ApplicationRolesBasicDependentTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public ApplicationRolesBasicDependentTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    /// <summary>
    /// Login call, just to store the access tokens for future tests to use
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-000"), Priority(-1)]
    public async Task Login()
    {
        await _serverFixture.GetAccessTokenAsync("superuser", "admin", null);
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
    [FriendlyNamedFact("IT-014"), Priority(10)]
    public async Task Global_Admin_Can_Create_Application_Role()
    {
        // Prepare 
        var request = new CreateRole.Request
        {
            Name = "General",
        };

        // Act
        var response = await _serverFixture.PostAsync<RoleDetails>("api/authorization/roles", request, authenticated: true);

        // Assert the response
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.Id.ShouldNotBe(Guid.Empty.ToString());
        response.Data.Name.ShouldBe("General");
        response.Data.Claims.ShouldNotBeNull();
        response.Data.Claims.Length.ShouldBe(0);

        // Assert the database
        var role = _serverFixture.Context.Set<Role>()
            .Include(x => x.Claims)
            .FirstOrDefault(x => x.Id == new Guid(response.Data.Id));

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
    [FriendlyNamedFact("IT-040"), Priority(20)]
    public async Task Local_Admin_Cannot_Delete_An_Application_Role()
    {
        // Prepare 
        var role = _serverFixture.Context.Set<Role>().Single(x => x.Name == "General");
        role.IsApplicationWide.ShouldBeTrue();

        var token = await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios");

        // Act
        var response = await _serverFixture.DeleteAsync($"api/authorization/roles/{role.Id}", token);

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
    [FriendlyNamedFact("IT-015"), Priority(30)]
    public async Task Global_Admin_Can_Rename_Application_Role()
    {
        // Prepare
        var role = _serverFixture.Context.Set<Role>().First(x => x.Name == "General");

        var request = new RenameRole.Request
        {
            Id = role.Id,
            Name = "General User"
        };

        // Act
        var response = await _serverFixture.PutAsync<RoleDetails>("api/authorization/roles", request, authenticated: true);

        // Assert the response
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.Id.ShouldBe(role.Id.ToString());
        response.Data.Name.ShouldBe("General User");
        response.Data.Claims.ShouldNotBeNull();
        response.Data.Claims.Length.ShouldBe(0);

        // Assert the database
        role = _serverFixture.Context.Set<Role>()
            .Include(x => x.Claims)
            .FirstOrDefault(x => x.Id == new Guid(response.Data.Id));

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
    [FriendlyNamedFact("IT-018"), Priority(40)]
    public async Task Global_Admin_Cannot_Create_Application_Role_With_Duplicated_Name()
    {
        // Prepare
        var request = new CreateRole.Request
        {
            Name = "GENERAL USER",
        };

        // Act
        var response = await _serverFixture.PostAsync<RoleDetails>("api/authorization/roles", request, authenticated: true);

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
    [FriendlyNamedFact("IT-048"), Priority(45)]
    public async Task Local_Admin_Cannot_Rename_Application_Role()
    {
        // Prepare
        var preExistingRole = _serverFixture.Context.Set<Role>().Single(x => x.Name == "General User");
        preExistingRole.ShouldNotBeNull();

        var request = new RenameRole.Request
        {
            Name = "RENAMED GENERAL USER",
        };

        // Act
        var response = await _serverFixture.PutAsync<RoleDetails>("api/authorization/roles", request, await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios"));

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
    [FriendlyNamedFact("IT-019"), Priority(50)]
    public async Task Global_Admin_Can_Query_Application_Roles()
    {
        // Prepare
        var body = new CreateRole.Request
        {
            Name = "Tenant Role"
        };
        var preResponse = await _serverFixture.PostAsync<RoleDetails>("api/authorization/roles", body, await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios"));
        preResponse.ShouldBeSuccess();

        // Act
        var response = await _serverFixture.GetAsync<RoleDetails[]>("api/authorization/roles", await _serverFixture.GetAccessTokenAsync("superuser", "admin", null));

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
    [FriendlyNamedTheory("IT-017"), Priority(60)]
    [InlineData(null)]
    [InlineData("")]
    public async Task Global_Admin_Cannot_Rename_Role_With_Empty_Or_Null_Name(string name)
    {
        // Prepare
        var role = _serverFixture.Context.Set<Role>().First(x => x.Name == "General User");

        var request = new RenameRole.Request
        {
            Id = role.Id,
            Name = name,
        };

        // Act
        var response = await _serverFixture.PutAsync<RoleDetails>("api/authorization/roles", request, authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The field 'Name' cannot be empty");
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
    [FriendlyNamedFact("IT-037"), Priority(70)]
    public async Task Global_Admin_Can_Create_Application_Role_Even_If_Exists_In_a_Tenant()
    {
        // Prepare 
        var preExistingRole = _serverFixture.Context.Set<Role>().First(x => x.Name == "Tenant Role");

        var request = new CreateRole.Request
        {
            Name = "Tenant Role",
        };

        // Act
        var response = await _serverFixture.PostAsync<RoleDetails>("api/authorization/roles", request, authenticated: true);

        // Assert the response
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.Id.ShouldNotBe(Guid.Empty.ToString());
        response.Data.Id.ShouldNotBe(preExistingRole.Id.ToString());
        response.Data.Name.ShouldBe("Tenant Role");
        response.Data.Source.ShouldBe(RoleSource.Global);
        response.Data.Claims.ShouldNotBeNull();
        response.Data.Claims.Length.ShouldBe(0);

        // Assert the database
        var roles = _serverFixture.Context.Set<Role>()
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
    [FriendlyNamedFact("IT-054"), Priority(75)]
    public async Task Global_Admin_Cannot_Rename_Application_Role_If_Name_Already_Exist_Even_With_Different_Casing()
    {
        // Prepare
        var role1 = _serverFixture.Context.Set<Role>().Single(x => x.Name == "General User" && x.TenantId == null);
        var role2 = _serverFixture.Context.Set<Role>().Single(x => x.Name == "Tenant Role" && x.TenantId == null);

        var renameCommand = new RenameRole.Request
        {
            Id = role2.Id,
            Name = "GENERAL USER"
        };

        // Act
        var response = await _serverFixture.PutAsync<RoleDetails>($"api/authorization/roles", renameCommand, authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Name already used");

        // Assert the database
        var check = _serverFixture.Context.Set<Role>().SingleOrDefault(x => x.Id == role2.Id);
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
    [FriendlyNamedFact("IT-020"), Priority(80)]
    public async Task Global_Admin_Can_Delete_Application_Roles()
    {
        // Prepare
        var role = _serverFixture.Context.Set<Role>().First(x => x.Name == "General User");

        // Act
        var response = await _serverFixture.DeleteAsync($"api/authorization/roles/{role.Id}", authenticated: true);

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        var check = _serverFixture.Context.Set<Role>().FirstOrDefault(x => x.Id == role.Id);

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
    [FriendlyNamedFact("IT-053"), Priority(85)]
    public async Task Global_Admin_Can_Rename_Application_Role_Even_If_Exists_As_Tenant_Role()
    {
        // Prepare
        var applicationRole = _serverFixture.Context.Set<Role>().Single(x => x.Name == "Tenant Role" && x.TenantId == null);
        var tenantRole = _serverFixture.Context.Set<Role>().Single(x => x.Name == "Tenant Role" && x.TenantId == "BUZIOS");

        // *** Test 1 *** Rename to something else
        {
            var renameCommand = new RenameRole.Request
            {
                Id = applicationRole.Id,
                Name = "Renamed Tenant Role"
            };

            // Act
            var response = await _serverFixture.PutAsync<RoleDetails>($"api/authorization/roles", renameCommand, authenticated: true);

            // Assert the response
            response.ShouldBeSuccess();

            // Assert the database
            var check = _serverFixture.Context.Set<Role>().SingleOrDefault(x => x.Id == applicationRole.Id);
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
            var response = await _serverFixture.PutAsync<RoleDetails>($"api/authorization/roles", renameCommand, authenticated: true);

            // Assert the response
            response.ShouldBeSuccess();

            // Assert the database
            var check = _serverFixture.Context.Set<Role>().SingleOrDefault(x => x.Id == tenantRole.Id);
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
    [FriendlyNamedFact("IT-044"), Priority(90)]
    public async Task Global_Admin_Cannot_Delete_Tenant_Roles()
    {
        // Prepare
        var applicationRole = _serverFixture.Context.Set<Role>().Single(x => x.Name == "Tenant Role" && x.TenantId == null);
        var tenantRole = _serverFixture.Context.Set<Role>().Single(x => x.Name == "Tenant Role" && x.TenantId == "BUZIOS");

        // Act
        var response = await _serverFixture.DeleteAsync($"api/authorization/roles/{tenantRole.Id}", authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Role not found");

        // Assert the database
        tenantRole = _serverFixture.Context.Set<Role>().SingleOrDefault(x => x.Name == "Tenant Role" && x.TenantId == null);
        applicationRole = _serverFixture.Context.Set<Role>().SingleOrDefault(x => x.Name == "Tenant Role" && x.TenantId == "BUZIOS");

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
    [FriendlyNamedFact("IT-043"), Priority(100)]
    public async Task Global_Admin_Can_Delete_Application_Role_That_Already_Exist_As_A_Tenant_Role()
    {
        // Prepare
        var applicationRole = _serverFixture.Context.Set<Role>().Single(x => x.Name == "Tenant Role" && x.TenantId == null);
        var tenantRole = _serverFixture.Context.Set<Role>().Single(x => x.Name == "Tenant Role" && x.TenantId == "BUZIOS");

        // Act
        var response = await _serverFixture.DeleteAsync($"api/authorization/roles/{applicationRole.Id}", authenticated: true);

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        applicationRole = _serverFixture.Context.Set<Role>().SingleOrDefault(x => x.Name == "Tenant Role" && x.TenantId == null);
        tenantRole = _serverFixture.Context.Set<Role>().SingleOrDefault(x => x.Name == "Tenant Role" && x.TenantId == "BUZIOS");

        applicationRole.ShouldBeNull();
        tenantRole.ShouldNotBeNull();
        tenantRole.IsApplicationWide.ShouldBe(false);
        tenantRole.TenantId.ShouldBe("BUZIOS");
    }
}

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class ApplicationRolesBasicIndependentTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public ApplicationRolesBasicIndependentTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    /// <summary>
    /// Login call, just to store the access tokens for future tests to use
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-000"), Priority(-1)]
    public async Task Login()
    {
        await _serverFixture.GetAccessTokenAsync("superuser", "admin", null);
    }

    /// <summary>
    /// The global admin should not be able to update a role that does not exist
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-016"), Priority(10)]
    public async Task Global_Admin_Cannot_Rename_Role_That_Does_Not_Exist()
    {
        // Prepare
        var request = new RenameRole.Request
        {
            Id = Guid.NewGuid(),
            Name = "New fake name",
        };

        // Act
        var response = await _serverFixture.PutAsync<RoleDetails>("api/authorization/roles", request, authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Role not found");
    }

    /// <summary>
    /// A global admin cannot delete an application role that doesn't exist
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-041"), Priority(20)]
    public async Task Global_Admin_Cannot_Delete_An_Application_Role_That_Doesnt_Exist()
    {
        // Act
        var response = await _serverFixture.DeleteAsync($"api/authorization/roles/{Guid.NewGuid()}", authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Role not found");
    }

    /// <summary>
    /// The global admin should not be able to create a new application wide role without a proper name
    /// DEPENDENCIES: none
    /// </summary>
    [InlineData(null)]
    [InlineData("")]
    [FriendlyNamedTheory("IT-036"), Priority(30)]
    public async Task Global_Admin_Cannot_Create_Application_Role_Without_Name(string name)
    {
        // Prepare 
        var request = new CreateRole.Request
        {
            Name = name,
        };

        // Act
        var response = await _serverFixture.PostAsync<RoleDetails>("api/authorization/roles", request, authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The field 'Role' cannot be empty");
    }

    /// <summary>
    /// The global admin should not be able to delete an application role that is being used by an user
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-222"), Priority(40)]
    public async Task Global_Admin_Cannot_Delete_Application_Role_Associated_With_User()
    {
        // Prepare 
        var context = _serverFixture.Context;

        var role = context.Add(new Role("Test Role 99")).Entity;

        var user = context.Set<User>().Include(x => x.Roles).First(x => x.Username == "admin1" && x.TenantId == "BUZIOS");
        user.AddRole(role);

        context.SaveChanges();

        role = _serverFixture.Context.Set<Role>().Include(x => x.Users).First(x => x.Id == role.Id);
        role.Users.Count().ShouldBe(1);

        // Act
        var response = await _serverFixture.DeleteAsync($"api/authorization/roles/{role.Id}", authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Role associated with one or more users");
    }

} 