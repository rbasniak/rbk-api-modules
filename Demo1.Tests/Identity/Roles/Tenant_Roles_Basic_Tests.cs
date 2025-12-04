using Demo1.Tests;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Identity.Core;
using rbkApiModules.Identity.Core.DataTransfer;

namespace rbkApiModules.Identity.Tests.Roles;

/// <summary>
/// In general, the local admin can only manage roles that are tenant wide,
/// they can also create roles with the same name of application roles and these
/// will be overwritten for that tenant
/// </summary>
[NotInParallel(nameof(Tenant_Roles_Basic_DependentTests))]
public class Tenant_Roles_Basic_DependentTests 
{
    [ClassDataSource<Demo1TestingServer>(Shared = SharedType.PerClass)]
    public required Demo1TestingServer TestingServer { get; set; } = default!;

    private static Dictionary<string, JwtToken> _tokens = new();

    [Test, NotInParallel(Order = 1)]
    public async Task Seed()
    {
        _tokens.Add("admin1-bz", new JwtToken((await TestingServer.LoginAsync("admin1", "123", "buzios")).Data!.AccessToken));
        _tokens.Add("admin1-bs", new JwtToken((await TestingServer.LoginAsync("admin1", "123", "un-bs")).Data!.AccessToken));
        _tokens.Add("superuser", new JwtToken((await TestingServer.LoginAsync("superuser", "admin", null)).Data!.AccessToken));
    }

    #region tables

    /// -------------------------------------        ------------------------------------- 
    /// | ID | TENANT | NAME                |        | ID | TENANT | NAME                | 
    /// -------------------------------------   >>   ------------------------------------- 
    /// |    | BUZIOS | Tenant Role         |        |    | BUZIOS | Tenant Role         |
    /// -------------------------------------        -------------------------------------

    #endregion
    /// <summary>
    /// The local admin should be able to create a new tenant wide role
    /// DEPENDENCIES: other tests depends on this
    /// </summary>
    [Test, NotInParallel(Order = 2)]
    public async Task Local_Admin_Can_Create_Tenant_Role()
    {
        // Prepare 
        var request = new CreateRole.Request
        {
            Name = "Tenant Role",
        };

        // Act
        var response = await TestingServer.PostAsync<RoleDetails>("api/authorization/roles", request, _tokens["admin1-bz"]);

        // Assert the response
        response.ShouldBeSuccess(out var result);

        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe("Tenant Role");
        result.Claims.ShouldNotBeNull();
        result.Claims.Length.ShouldBe(0);

        // Assert the database
        var role = TestingServer.CreateContext().Set<Role>()
            .Include(x => x.Claims)
            .FirstOrDefault(x => x.Id == result.Id);

        role.ShouldNotBeNull();
        role.Name.ShouldBe("Tenant Role");
        role.Claims.ShouldNotBeNull();
        role.Claims.Count().ShouldBe(0);
    }

    #region tables

    /// -------------------------------------        ------------------------------------- 
    /// | ID | TENANT | NAME                |        | ID | TENANT | NAME                | 
    /// -------------------------------------   >>   ------------------------------------- 
    /// |    | BUZIOS | Tenant Role         |        |    | BUZIOS | Tenant Role         |
    /// |    |        |                     |        |    | UN-BS  | Tenant Role         |
    /// -------------------------------------        -------------------------------------

    #endregion
    /// <summary>
    /// The local admin should be able to create a new tenant wide role even if another tenant has a role with the same name
    /// DEPENDENCIES: 'Tenant Role' created on  IT-029
    /// </summary>
    [Test, NotInParallel(Order = 3)]
    public async Task Local_Admin_Can_Create_Tenant_Role_Even_If_There_Is_Another_With_Same_Name_In_Another_Tenant()
    {
        // Prepare 
        var request = new CreateRole.Request
        {
            Name = "Tenant Role",
        };

        // Act
        var response = await TestingServer.PostAsync<RoleDetails>("api/authorization/roles", request, _tokens["admin1-bs"]);

        // Assert the response
        response.ShouldBeSuccess(out var result);

        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe("Tenant Role");
        result.Claims.ShouldNotBeNull();
        result.Claims.Length.ShouldBe(0);

        // Assert the database
        var roles = TestingServer.CreateContext().Set<Role>()
            .Include(x => x.Claims)
            .Where(x => x.Name == "Tenant Role")
            .ToList();

        roles.Count.ShouldBe(2);

        var tenants = roles.Select(x => x.TenantId).ToArray();
        tenants.Contains("BUZIOS").ShouldBeTrue();
        tenants.Contains("UN-BS").ShouldBeTrue();
    }

    #region tables

    /// -------------------------------------        ------------------------------------- 
    /// | ID | TENANT | NAME                |        | ID | TENANT | NAME                | 
    /// -------------------------------------   >>   ------------------------------------- 
    /// |    | BUZIOS | Tenant Role         |        |    | BUZIOS | Renamed Tenant Role |
    /// |    | UN-BS  | Tenant Role         |        |    | UN-BS  | Tenant Role         |
    /// -------------------------------------        -------------------------------------

    #endregion
    /// <summary>
    /// The local admin should be able to rename an existing tenant wide role
    /// DEPENDENCIES: 'Tenant Role' created on IT-029
    /// </summary>
    [Test, NotInParallel(Order = 4)]
    public async Task Local_Admin_Can_Rename_Tenant_Role()
    {
        // Prepare
        var role = TestingServer.CreateContext().Set<Role>().First(x => x.Name == "Tenant Role");

        var request = new RenameRole.Request
        {
            Id = role.Id,
            Name = "Renamed Tenant Role"
        };

        // Act
        var response = await TestingServer.PutAsync<RoleDetails>("api/authorization/roles", request, _tokens["admin1-bz"]);

        // Assert the response
        response.ShouldBeSuccess(out var result);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(role.Id);
        result.Name.ShouldBe("Renamed Tenant Role");
        result.Claims.ShouldNotBeNull();
        result.Claims.Length.ShouldBe(0);

        // Assert the database
        role = TestingServer.CreateContext().Set<Role>()
            .Include(x => x.Claims)
            .FirstOrDefault(x => x.Id == result.Id);

        role.ShouldNotBeNull();
        role.Name.ShouldBe("Renamed Tenant Role");
        role.Claims.ShouldNotBeNull();
        role.Claims.Count().ShouldBe(0);
    }

    #region tables

    /// ------------------------------------- 
    /// | ID | TENANT | NAME                | 
    /// ------------------------------------- 
    /// |    | BUZIOS | Renamed Tenant Role |
    /// |    | UN-BS  | Tenant Role         |
    /// -------------------------------------

    #endregion
    /// <summary>
    /// The local admin should have validation error if tenant role name is empty or null
    /// DEPENDENCIES: 'Renamed Tenant Role' renamed on IT-030
    /// </summary>
    [Arguments(null)]
    [Arguments("")]
    [Test, NotInParallel(Order = 5)]
    public async Task Local_Admin_Cannot_Rename_Role_With_Empty_Or_Null_Name(string? name)
    {
        // Prepare
        var role = TestingServer.CreateContext().Set<Role>().First(x => x.Name == "Renamed Tenant Role");

        var request = new RenameRole.Request
        {
            Id = role.Id,
            Name = name!,
        };

        // Act
        var response = await TestingServer.PutAsync<RoleDetails>("api/authorization/roles", request, _tokens["admin1-bz"]);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "'Name' must not be empty.");
    }

    #region tables

    /// ------------------------------------- 
    /// | ID | TENANT | NAME                | 
    /// ------------------------------------- 
    /// |    | BUZIOS | Renamed Tenant Role |
    /// |    | UN-BS  | Tenant Role         |
    /// -------------------------------------

    #endregion
    /// <summary>
    /// Tenant roles with the same name are not allowed, so we should not be able to create them\
    /// DEPENDENCIES: 'Renamed Tenant Role' renamed on IT-030
    /// </summary>
    [Test, NotInParallel(Order = 6)]
    public async Task Local_Admin_Cannot_Create_Tenant_Role_With_Duplicated_Name()
    {
        // Prepare
        var request = new CreateRole.Request
        {
            Name = "Renamed Tenant Role",
        };

        // Act
        var response = await TestingServer.PostAsync<RoleDetails>("api/authorization/roles", request, _tokens["admin1-bz"]);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Name already used");
    }

    #region tables

    /// -------------------------------------        ---------------------------------------------------------
    /// | ID | TENANT | NAME                |        | ID | TENANT | NAME                                    |
    /// -------------------------------------        ---------------------------------------------------------
    /// |    | BUZIOS | Renamed Tenant Role |        |    | BUZIOS | Renamed Tenant Role                     |
    /// |    | UN-BS  | Tenant Role         |   >>   |    | UN-BS  | Tenant Role                             |
    /// |    |        |                     |        |    | NULL   | Application Wide Role To Be Overwritten |
    /// |    |        |                     |        |    | BUZIOS | Application Wide Role To Be Overwritten |
    /// |    |        |                     |        |    | NULL   | Trully Application Wide Role            |
    /// -------------------------------------        ---------------------------------------------------------

    #endregion
    /// <summary>
    /// Local admin can see a list of application roles plus those that were overritten by them
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 7)]
    public async Task Local_Admin_Can_Query_Application_Roles_And_Overwritten_Tenant_Roles()
    {
        // Prepare
        var body1 = new CreateRole.Request
        { 
            Name = "Application Wide Role To Be Overwritten"
        };
        var preResponse1 = await TestingServer.PostAsync<RoleDetails>("api/authorization/roles", body1, _tokens["superuser"]);
        preResponse1.ShouldNotBeNull();
        preResponse1.ShouldBeSuccess();

        var body2 = new CreateRole.Request
        {
            Name = "Application Wide Role To Be Overwritten"
        };
        var preResponse2 = await TestingServer.PostAsync<RoleDetails>("api/authorization/roles", body2, _tokens["admin1-bz"]);
        preResponse2.ShouldNotBeNull();
        preResponse2.ShouldBeSuccess();

        var body3 = new CreateRole.Request
        {
            Name = "Trully Application Wide Role"
        };
        var preResponse3 = await TestingServer.PostAsync<RoleDetails>("api/authorization/roles", body3, _tokens["superuser"]);
        preResponse3.ShouldNotBeNull();
        preResponse3.ShouldBeSuccess();

        // Act
        var response = await TestingServer.GetAsync<RoleDetails[]>("api/authorization/roles", _tokens["admin1-bz"]);

        // Assert the response
        response.ShouldBeSuccess(out var result);

        result.ShouldNotBeNull();
        result.Length.ShouldBeGreaterThan(0);
        
        // Find the entity created within these tests and assert it
        var role1 = result.SingleOrDefault(x => x.Name == "Renamed Tenant Role");
        role1.ShouldNotBeNull();
        role1.Source.ShouldBe(RoleSource.Local);
        role1.ShouldNotBeNull();

        // Find the entity created in this tests and assert it, ensuring it's not the application wide one
        var role2 = result.SingleOrDefault(x => x.Name == "Application Wide Role To Be Overwritten");
        role2.ShouldNotBeNull();
        role2.Source.ShouldBe(RoleSource.Local);
        role2.Id.ShouldNotBe(preResponse1.Data!.Id);

        // Find the application wide entity created in this tests and assert it
        var role3 = result.SingleOrDefault(x => x.Name == "Trully Application Wide Role");
        role3.ShouldNotBeNull();
        role3.Source.ShouldBe(RoleSource.Global);
        role3.Id.ShouldBe(preResponse3.Data!.Id);
    }

    #region tables

    /// ---------------------------------------------------------        ---------------------------------------------------------
    /// | ID | TENANT | NAME                                    |        | ID | TENANT | NAME                                    |
    /// ---------------------------------------------------------        ---------------------------------------------------------
    /// |    | BUZIOS | Renamed Tenant Role                     |        |    |        |                                         |
    /// |    | UN-BS  | Tenant Role                             |   >>   |    | UN-BS  | Tenant Role                             |
    /// |    | NULL   | Application Wide Role To Be Overwritten |        |    | NULL   | Application Wide Role To Be Overwritten |
    /// |    | BUZIOS | Application Wide Role To Be Overwritten |        |    | BUZIOS | Application Wide Role To Be Overwritten |
    /// |    | NULL   | Trully Application Wide Role            |        |    | NULL   | Trully Application Wide Role            |
    /// ---------------------------------------------------------        ---------------------------------------------------------

    #endregion
    /// <summary>
    /// Local admin can delete tenant roles when they are not used yet
    /// DEPENDENCIES: 'Renamed Tenant Role' renamed on IT-030
    /// </summary>
    [Test, NotInParallel(Order = 8)]
    public async Task Local_Admin_Can_Delete_Tenant_Roles()
    {
        // Prepare
        var role = TestingServer.CreateContext().Set<Role>().First(x => x.Name == "Renamed Tenant Role"); 

        // Act
        var response = await TestingServer.DeleteAsync($"api/authorization/roles/{role.Id}", _tokens["admin1-bz"]);

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        var check = TestingServer.CreateContext().Set<Role>().FirstOrDefault(x => x.Id == role.Id);

        check.ShouldBeNull();
    }

    #region tables

    /// ---------------------------------------------------------
    /// | ID | TENANT | NAME                                    |
    /// ---------------------------------------------------------
    /// |    | UN-BS  | Tenant Role                             |
    /// |    | NULL   | Application Wide Role To Be Overwritten |
    /// |    | BUZIOS | Application Wide Role To Be Overwritten |
    /// |    | NULL   | Trully Application Wide Role            |
    /// ---------------------------------------------------------

    #endregion
    /// <summary>
    /// Local admin cannot delete tenant role that does not exist for him
    /// DEPENDENCIES: 'Tenant Role' created to UN-BS on IT-039
    /// </summary>
    [Test, NotInParallel(Order = 9)]
    public async Task Local_Admin_Cannot_Delete_Tenant_Roles_That_Doest_Exist_For_Him()
    {
        // Prepare
        var existingRole = TestingServer.CreateContext().Set<Role>().Single(x => x.Name == "Tenant Role");
        existingRole.ShouldNotBeNull();

        // Act
        var response = await TestingServer.DeleteAsync($"api/authorization/roles/{existingRole.Id}", _tokens["admin1-bz"]);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Role not found");

        // Assert the database
        existingRole = TestingServer.CreateContext().Set<Role>().Single(x => x.Name == "Tenant Role");
        existingRole.ShouldNotBeNull();
    }

    #region tables

    /// ---------------------------------------------------------       ---------------------------------------------------------
    /// | ID | TENANT | NAME                                    |       | ID | TENANT | NAME                                    |
    /// ---------------------------------------------------------       ---------------------------------------------------------
    /// |    | UN-BS  | Tenant Role                             |       |    | UN-BS  | Tenant Role                             |
    /// |    | BUZIOS | Tenant Role                             |   >>  |    |        |                                         |
    /// |    | NULL   | Application Wide Role To Be Overwritten |       |    | NULL   | Application Wide Role To Be Overwritten |
    /// |    | BUZIOS | Application Wide Role To Be Overwritten |       |    | BUZIOS | Application Wide Role To Be Overwritten |
    /// |    | NULL   | Trully Application Wide Role            |       |    | NULL   | Trully Application Wide Role            |
    /// ---------------------------------------------------------       ---------------------------------------------------------

    #endregion
    /// <summary>
    /// Local admin can delete a role that exists with the same name in different tenants
    /// DEPENDENCIES: 'Tenant Role' created to UN-BS on IT-039
    /// </summary>
    [Test, NotInParallel(Order = 10)]
    public async Task Local_Admin_Can_Delete_Tenant_Role_That_Does_Exist_For_Him_And_Others()
    {
        // Prepare
        var request = new CreateRole.Request
        {
            Name = "Tenant Role"
        };

        var createResponse = await TestingServer.PostAsync<RoleDetails>("api/authorization/roles", request, _tokens["admin1-bz"]);
        createResponse.ShouldBeSuccess();

        var role1 = TestingServer.CreateContext().Set<Role>().Single(x => x.Name == "Tenant Role" && x.TenantId == "UN-BS");
        var role2 = TestingServer.CreateContext().Set<Role>().Single(x => x.Name == "Tenant Role" && x.TenantId == "BUZIOS");
        role1.ShouldNotBeNull();
        role2.ShouldNotBeNull();

        // Act
        var response = await TestingServer.DeleteAsync($"api/authorization/roles/{role2.Id}", _tokens["admin1-bz"]);

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        role1 = TestingServer.CreateContext().Set<Role>().SingleOrDefault(x => x.Name == "Tenant Role" && x.TenantId == "UN-BS");
        role2 = TestingServer.CreateContext().Set<Role>().SingleOrDefault(x => x.Name == "Tenant Role" && x.TenantId == "BUZIOS");
        role1.ShouldNotBeNull();
        role2.ShouldBeNull();
    }

    #region tables

    /// ---------------------------------------------------------       ---------------------------------------------------------
    /// | ID | TENANT | NAME                                    |       | ID | TENANT | NAME                                    |
    /// ---------------------------------------------------------       ---------------------------------------------------------
    /// |    | UN-BS  | Tenant Role                             |       |    | UN-BS  | Tenant Role                             |
    /// |    | NULL   | Application Wide Role To Be Overwritten |   >>  |    | NULL   | Application Wide Role To Be Overwritten |
    /// |    | BUZIOS | Application Wide Role To Be Overwritten |       |    |        |                                         |
    /// |    | NULL   | Trully Application Wide Role            |       |    | NULL   | Trully Application Wide Role            |
    /// ---------------------------------------------------------       ---------------------------------------------------------

    #endregion
    /// <summary>
    /// Local admin can delete a role that exists with the same name as application wide role
    /// DEPENDENCIES: 'Application Wide Role To Be Overwritten' created to UN-BS on IT-034
    /// </summary>
    [Test, NotInParallel(Order = 11)]
    public async Task Local_Admin_Can_Delete_Tenant_Role_That_Does_Exist_As_Application_Role()
    {
        // Prepare
        var applicationRole = TestingServer.CreateContext().Set<Role>().Single(x => x.Name == "Application Wide Role To Be Overwritten" && x.TenantId == null);
        var tenantRole = TestingServer.CreateContext().Set<Role>().Single(x => x.Name == "Application Wide Role To Be Overwritten" && x.TenantId == "BUZIOS");
        applicationRole.ShouldNotBeNull();
        tenantRole.ShouldNotBeNull();

        // Act
        var response = await TestingServer.DeleteAsync($"api/authorization/roles/{tenantRole.Id}", _tokens["admin1-bz"]);

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        applicationRole = TestingServer.CreateContext().Set<Role>().SingleOrDefault(x => x.Name == "Application Wide Role To Be Overwritten" && x.TenantId == null);
        tenantRole = TestingServer.CreateContext().Set<Role>().SingleOrDefault(x => x.Name == "Application Wide Role To Be Overwritten" && x.TenantId == "BUZIOS");
        applicationRole.ShouldNotBeNull();
        tenantRole.ShouldBeNull();
    }

    #region tables

    /// ---------------------------------------------------------
    /// | ID | TENANT | NAME                                    |
    /// ---------------------------------------------------------
    /// |    | UN-BS  | Tenant Role                             |
    /// |    | NULL   | Application Wide Role To Be Overwritten |
    /// |    | NULL   | Trully Application Wide Role            |
    /// ---------------------------------------------------------

    #endregion
    /// <summary>
    /// Global admin cannot rename a tenant role
    /// DEPENDENCIES: 'Application Wide Role To Be Overwritten' created to UN-BS on IT-034
    /// </summary>
    [Test, NotInParallel(Order = 12)]
    public async Task Global_Admin_Cannot_Rename_Tenant_Role()
    {
        // Prepare
        var tenantRole = TestingServer.CreateContext().Set<Role>().Single(x => x.Name == "Tenant Role" && x.TenantId == "UN-BS");
        tenantRole.ShouldNotBeNull();

        var request = new RenameRole.Request 
        { 
            Id = tenantRole.Id,
            Name = "New name"
        };

        // Act
        var response = await TestingServer.PutAsync<RoleDetails>($"api/authorization/roles/", request, _tokens["superuser"]);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Role not found");

        // Assert the database
        tenantRole = TestingServer.CreateContext().Set<Role>().Single(x => x.Name == "Tenant Role" && x.TenantId == "UN-BS");
        tenantRole.ShouldNotBeNull();
    }

    #region tables

    /// ---------------------------------------------------------      ---------------------------------------------------------
    /// | ID | TENANT | NAME                                    |      | ID | TENANT | NAME                                    |
    /// ---------------------------------------------------------      ---------------------------------------------------------
    /// |    | UN-BS  | Tenant Role                             |  >>  |    | UN-BS  | Tenant Role                             |
    /// |    |        |                                         |      |    | NULL   | Tenant Role                             |
    /// |    | NULL   | Application Wide Role To Be Overwritten |      |    | NULL   | Application Wide Role To Be Overwritten |
    /// |    | NULL   | Trully Application Wide Role            |      |    | NULL   | Trully Application Wide Role            |
    /// ---------------------------------------------------------      ---------------------------------------------------------

    #endregion
    /// <summary>
    /// Global admin cannot rename a tenant role, even if it exists as application role
    /// DEPENDENCIES: 'Tenant Role' created to UN-BS on IT-029
    /// </summary>
    [Test, NotInParallel(Order = 13)]
    public async Task Global_Admin_Cannot_Rename_Tenant_Role_Even_If_It_Exists_As_Application_Role()
    {
        // Prepare
        var createRequest = new CreateRole.Request
        {
            Name = "Tenant Role"
        };

        var createResponse = await TestingServer.PostAsync<RoleDetails>($"api/authorization/roles/", createRequest, _tokens["superuser"]);

        var applicationRole = TestingServer.CreateContext().Set<Role>().Single(x => x.Name == "Tenant Role" && x.TenantId == null);
        var tenantRole = TestingServer.CreateContext().Set<Role>().Single(x => x.Name == "Tenant Role" && x.TenantId == "UN-BS");
        applicationRole.ShouldNotBeNull();
        tenantRole.ShouldNotBeNull();

        var request = new RenameRole.Request
        {
            Id = tenantRole.Id,
            Name = "New name"
        };

        // Act
        var response = await TestingServer.PutAsync<RoleDetails>($"api/authorization/roles/", request, _tokens["superuser"]);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Role not found");

        // Assert the database
        tenantRole = TestingServer.CreateContext().Set<Role>().Single(x => x.Name == "Tenant Role" && x.TenantId == "UN-BS");
        tenantRole.ShouldNotBeNull();
    }

    #region tables

    /// ---------------------------------------------------------      ---------------------------------------------------------
    /// | ID | TENANT | NAME                                    |      | ID | TENANT | NAME                                    |
    /// ---------------------------------------------------------      ---------------------------------------------------------
    /// |    | UN-BS  | Tenant Role                             |  >>  |    | UN-BS  | Tenant Role                             |
    /// |    | NULL   | Tenant Role                             |      |    | NULL   | Tenant Role                             |
    /// |    | NULL   | Application Wide Role To Be Overwritten |      |    | NULL   | Application Wide Role To Be Overwritten |
    /// |    | NULL   | Trully Application Wide Role            |      |    | NULL   | Trully Application Wide Role            |
    /// |    |        |                                         |      |    | BUZIOS | Tenant Role                             |
    /// |    |        |                                         |      |    | BUZIOS | Role to be Renamed                      |
    /// ---------------------------------------------------------      ---------------------------------------------------------

    #endregion
    /// <summary>
    /// Local admin cannot rename a tenant role, if it already exists within that tenant, even with different casing
    /// DEPENDENCIES: 'Tenant Role' created to UN-BS on IT-029
    /// </summary>
    [Test, NotInParallel(Order = 14)]
    public async Task Local_Admin_Cannot_Rename_Tenant_Role_With_a_Name_Already_Existing()
    {
        // Prepare
        async Task<(Role OtherTenantRole, Role CurrentTenantRole, Role RoleToBeRenamed)> fetchData()
        {
            var createRequest1 = new CreateRole.Request { Name = "Tenant Role" };
            var createResponse1 = await TestingServer.PostAsync<RoleDetails>($"api/authorization/roles", createRequest1, _tokens["admin1-bz"]);

            var createRequest2 = new CreateRole.Request { Name = "Role to be Renamed" };
            var createResponse2 = await TestingServer.PostAsync<RoleDetails>($"api/authorization/roles", createRequest2, _tokens["admin1-bz"]);

            var otherTenantRole = TestingServer.CreateContext().Set<Role>().SingleOrDefault(x => x.Name == "Tenant Role" && x.TenantId == "UN-BS");
            var currentTenantRole = TestingServer.CreateContext().Set<Role>().SingleOrDefault(x => x.Name == "Tenant Role" && x.TenantId == "BUZIOS");
            var roleToBeRenamed = TestingServer.CreateContext().Set<Role>().SingleOrDefault(x => x.Name == "Role to be Renamed" && x.TenantId == "BUZIOS");

            otherTenantRole.ShouldNotBeNull();
            currentTenantRole.ShouldNotBeNull();
            roleToBeRenamed.ShouldNotBeNull();

            return (otherTenantRole, currentTenantRole, roleToBeRenamed);
        }

        var entitiesBefore = await fetchData();

        entitiesBefore.OtherTenantRole.ShouldNotBeNull();
        entitiesBefore.CurrentTenantRole.ShouldNotBeNull();
        entitiesBefore.RoleToBeRenamed.ShouldNotBeNull();

        var request = new RenameRole.Request
        {
            Id = entitiesBefore.RoleToBeRenamed.Id,
            Name = "tENANT rOLE"
        };

        // Act
        var response = await TestingServer.PutAsync<RoleDetails>($"api/authorization/roles", request, _tokens["admin1-bz"]);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Name already used");

        // Assert the database
        var entitiesAfter = await fetchData();
        entitiesAfter.OtherTenantRole.Name.ShouldBe("Tenant Role");
        entitiesAfter.CurrentTenantRole.Name.ShouldBe("Tenant Role");
        entitiesAfter.RoleToBeRenamed.Name.ShouldBe("Role to be Renamed");
    }

    #region tables

    /// ---------------------------------------------------------      ---------------------------------------------------------
    /// | ID | TENANT | NAME                                    |      | ID | TENANT | NAME                                    |
    /// ---------------------------------------------------------      ---------------------------------------------------------
    /// |    | UN-BS  | Tenant Role                             |      |    | UN-BS  | Tenant Role                             |
    /// |    | NULL   | Tenant Role                             |      |    | NULL   | Tenant Role                             |
    /// |    | NULL   | Application Wide Role To Be Overwritten |      |    | NULL   | Application Wide Role To Be Overwritten |
    /// |    | NULL   | Trully Application Wide Role            |      |    | NULL   | Trully Application Wide Role            |
    /// |    | BUZIOS | Tenant Role                             |      |    | BUZIOS | Tenant Role                             |
    /// |    | BUZIOS | Role to be Renamed                      |      |    | BUZIOS | Trully Application Wide Role            |
    /// ---------------------------------------------------------      ---------------------------------------------------------

    #endregion
    /// <summary>
    /// Local admin can rename a tenant role even if the name already exists as an application wide role
    /// DEPENDENCIES: 'Trully Application Wide Role' created to UN-BS on IT-034
    /// </summary>
    [Test, NotInParallel(Order = 15)]
    public async Task Local_Admin_Can_Rename_Tenant_Role_Event_If_There_Is_An_Application_Role_With_The_Same_Name()
    {
        // Prepare
        var applicationRole = TestingServer.CreateContext().Set<Role>().SingleOrDefault(x => x.Name == "Trully Application Wide Role" && x.TenantId == null);
        var roleToBeRenamed = TestingServer.CreateContext().Set<Role>().SingleOrDefault(x => x.Name == "Role to be Renamed" && x.TenantId == "BUZIOS");

        applicationRole.ShouldNotBeNull();
        roleToBeRenamed.ShouldNotBeNull();

        var request = new RenameRole.Request
        {
            Id = roleToBeRenamed.Id,
            Name = "Trully Application Wide Role"
        };

        // Act
        var response = await TestingServer.PutAsync<RoleDetails>($"api/authorization/roles", request, _tokens["admin1-bz"]);

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        var renamedRole = TestingServer.CreateContext().Set<Role>().SingleOrDefault(x => x.Name == "Trully Application Wide Role" && x.TenantId == "BUZIOS");

        renamedRole.ShouldNotBeNull();
        renamedRole.Id.ShouldBe(roleToBeRenamed.Id);
    }

    /// <summary>
    /// When an admin overrides an application role, all users using that application role must be switched to the new role
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 16)]
    public async Task When_Admin_Overwrites_An_Application_Role_All_Users_Must_Be_Switched_To_The_New_Role()
    {
        User GetUser1(DbContext context) => context.Set<User>().Include(x => x.Roles).ThenInclude(x => x.Role).First(x => x.Username == "john.doe" && x.TenantId == "BUZIOS");
        User GetUser2(DbContext context) => context.Set<User>().Include(x => x.Roles).ThenInclude(x => x.Role).First(x => x.Username == "jane.doe" && x.TenantId == "BUZIOS");
        User GetUser3(DbContext context) => context.Set<User>().Include(x => x.Roles).ThenInclude(x => x.Role).First(x => x.Username == "jane.doe" && x.TenantId == "UN-BS");

        // Prepare 
        var context = TestingServer.CreateContext();

        var applicationRole = context.Add(new Role("CEO")).Entity;

        var user1 = GetUser1(context);
        user1.AddRole(applicationRole);

        var user2 = GetUser2(context);
        user2.AddRole(applicationRole);

        var user3 = GetUser3(context);
        user3.AddRole(applicationRole);

        context.SaveChanges();

        user1 = GetUser1(TestingServer.CreateContext());
        user1.Roles.SingleOrDefault(x => x.RoleId == applicationRole.Id).ShouldNotBeNull();

        user2 = GetUser2(TestingServer.CreateContext());
        user2.Roles.SingleOrDefault(x => x.RoleId == applicationRole.Id).ShouldNotBeNull();

        user3 = GetUser3(TestingServer.CreateContext());
        user3.Roles.SingleOrDefault(x => x.RoleId == applicationRole.Id).ShouldNotBeNull();

        // Now we have the CEO role as an application wide role, but overwritted in the tenant.
        // Two tenant users using it and one user from another tenant

        var request = new CreateRole.Request
        {
            Name = "CEO"
        };

        // Act
        var response = await TestingServer.PostAsync<RoleDetails>($"api/authorization/roles", request, _tokens["admin1-bz"]);

        // Assert the response
        response.ShouldBeSuccess();
        response.Data.ShouldNotBeNull();

        var tenantRoleId = response.Data.Id;

        // Check if the roles in the tenant user have been updated, but not for the other tenant's user
        user1 = GetUser1(TestingServer.CreateContext());
        user1.Roles.SingleOrDefault(x => x.RoleId == tenantRoleId).ShouldNotBeNull();

        user2 = GetUser2(TestingServer.CreateContext());
        user2.Roles.SingleOrDefault(x => x.RoleId == tenantRoleId).ShouldNotBeNull();

        user3 = GetUser3(TestingServer.CreateContext());
        user3.Roles.SingleOrDefault(x => x.RoleId == applicationRole.Id).ShouldNotBeNull();
    }

    /// <summary>
    /// When an admin deletes an overrided application role, all users using that application role must be switched to the new role
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 17)]
    public async Task When_Admin_Deletes_An_Overwritten_Application_Role_All_Users_Must_Be_Switched_To_The_Old_Role()
    {
        User GetUser1(DbContext context) => context.Set<User>().Include(x => x.Roles).First(x => x.Username == "john.doe" && x.TenantId == "BUZIOS");
        User GetUser2(DbContext context) => context.Set<User>().Include(x => x.Roles).First(x => x.Username == "jane.doe" && x.TenantId == "BUZIOS");
        User GetUser3(DbContext context) => context.Set<User>().Include(x => x.Roles).First(x => x.Username == "jane.doe" && x.TenantId == "UN-BS");

        // Prepare 
        var context = TestingServer.CreateContext();

        var applicationRole = context.Set<Role>().SingleOrDefault(x => x.Name == "CEO" && x.TenantId == null);
        applicationRole.ShouldNotBeNull();

        var tenantRole = context.Set<Role>().SingleOrDefault(x => x.Name == "CEO" && x.TenantId == "BUZIOS");
        tenantRole.ShouldNotBeNull();

        GetUser1(context).Roles.SingleOrDefault(x => x.RoleId == tenantRole.Id).ShouldNotBeNull();
        GetUser2(context).Roles.SingleOrDefault(x => x.RoleId == tenantRole.Id).ShouldNotBeNull();
        GetUser3(context).Roles.SingleOrDefault(x => x.RoleId == applicationRole.Id).ShouldNotBeNull();

        // Act
        var response = await TestingServer.DeleteAsync($"api/authorization/roles/{tenantRole.Id}", _tokens["admin1-bz"]);

        // Assert the response
        response.ShouldBeSuccess();

        // Check if the roles in the tenant user have been updated, but not for the other tenant's user
        GetUser1(TestingServer.CreateContext()).Roles.SingleOrDefault(x => x.RoleId == applicationRole.Id).ShouldNotBeNull();
        GetUser2(TestingServer.CreateContext()).Roles.SingleOrDefault(x => x.RoleId == applicationRole.Id).ShouldNotBeNull();
        GetUser3(TestingServer.CreateContext()).Roles.SingleOrDefault(x => x.RoleId == applicationRole.Id).ShouldNotBeNull();
    }

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }
}


