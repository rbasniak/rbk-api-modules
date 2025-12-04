using Demo1.Tests;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Identity.Core;
using rbkApiModules.Identity.Core.DataTransfer;

namespace rbkApiModules.Identity.Tests.Roles;

/// <summary>
/// In general, the global admin can only manage roles that are application wide,
/// for tenant wide roles, only the local admins are able to manage
/// These tests deal with the update of list of claims for the roles
/// </summary>
[NotInParallel(nameof(Application_Role_Claim_Association_Tests))]
public class Application_Role_Claim_Association_Tests 
{
    [ClassDataSource<Demo1TestingServer>(Shared = SharedType.PerClass)]
    public required Demo1TestingServer TestingServer { get; set; } = default!;

    #region tables

    /// -------------------------------------   -------------------------------------  
    /// |              ROLES                |   |              CLAIMS               |  
    /// -------------------------------------   -------------------------------------  
    /// | ID | TENANT | NAME                |   | ID |  NAME                        |  
    /// -------------------------------------   -------------------------------------  
    /// |    | BUZIOS | Local Tenant Role   |   |    |  CAN_FILL_TIMESHEETS         |  
    /// |    | NULL   | General User        |   |    |  CAN_REQUEST_MATERIALS       |  
    /// |    | BUZIOS | General User        |   |    |  CAN_SEND_EMAILS             |  
    /// -------------------------------------   -------------------------------------  
    ///
    /// ---------------------------------------------------------   
    /// |                    ROLES TO CLAIMS                    |   
    /// ---------------------------------------------------------   
    /// | ROLE                  |  CLAIM                        |   
    /// ---------------------------------------------------------   
    /// |                       |                               |   
    /// ---------------------------------------------------------   
    ///

    #endregion

    /// <summary>
    /// Database setup for all tests in this class
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 1)]
    public async Task Setup()
    {
        // Prepare the default access token
        await TestingServer.CacheCredentialsAsync("superuser", "admin", null);
        await TestingServer.CacheCredentialsAsync("admin1", "123", "buzios");

        var createClaimCommands = new[]
        {
            new CreateClaim.Request { Identification = "CAN_FILL_TIMESHEETS", Description = "CAN_FILL_TIMESHEETS" },
            new CreateClaim.Request { Identification = "CAN_REQUEST_MATERIALS", Description = "CAN_REQUEST_MATERIALS" },
            new CreateClaim.Request { Identification = "CAN_SEND_EMAILS", Description = "CAN_SEND_EMAILS" },
        };

        foreach (var command in createClaimCommands)
        {
            var response = await TestingServer.PostAsync<ClaimDetails>("api/authorization/claims", command, "superuser");
            response.ShouldBeSuccess();
        }

        var response1 = await TestingServer.PostAsync<RoleDetails>("api/authorization/roles", new CreateRole.Request { Name = "General User" }, "superuser");
        var response2 = await TestingServer.PostAsync<RoleDetails>("api/authorization/roles", new CreateRole.Request { Name = "General User" }, "admin1");
        var response3 = await TestingServer.PostAsync<RoleDetails>("api/authorization/roles", new CreateRole.Request { Name = "Local Tenant Role" }, "admin1");

        response1.ShouldBeSuccess();
        response2.ShouldBeSuccess();
        response3.ShouldBeSuccess();
    }

    private Claim GetClaim(string identification)
    {
        return TestingServer.CreateContext().Set<Claim>().Single(x => x.Identification.ToLower() == identification.ToLower());
    }

    private Role GetRole(string name, string? tenant = null)
    {
        return TestingServer.CreateContext().Set<Role>().Single(x => x.Name.ToLower() == name.ToLower() && x.TenantId == tenant);
    }

    /// <summary>
    /// A local admin should not be able to change the claims of an application wide role
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 2)]
    public async Task Local_Admin_Cannot_Change_Claims_Of_An_Application_Wide_Role()
    {
        // Prepare 
        var role = GetRole("General User");

        var request = new UpdateRoleClaims.Request
        {
            Id = role.Id,
            ClaimsIds = new[]
            {
                GetClaim("CAN_SEND_EMAILS").Id,
                GetClaim("CAN_FILL_TIMESHEETS").Id,
            }
        };

        // Act
        var response = await TestingServer.PostAsync<RoleDetails>("api/authorization/roles/update-claims", request, "admin1");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Role not found");

        // Assert the database
        role = TestingServer.CreateContext().Set<Role>()
            .Include(x => x.Claims)
            .FirstOrDefault(x => x.Id == role.Id);

        role.ShouldNotBeNull();
        role.TenantId.ShouldBeNull();
        role.Claims.ShouldNotBeNull();
        role.Claims.Count().ShouldBe(0);
    }

    /// <summary>
    /// The global admin should not be able to change the claims of a role that does not exist as an application wide role
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 3)]
    public async Task Global_Admin_Cannot_Change_Claims_Of_a_Tenant_Wide_Role()
    {
        // Prepare 
        var role = GetRole("Local Tenant Role", "BUZIOS");

        var request = new UpdateRoleClaims.Request
        {
            Id = role.Id,
            ClaimsIds = new[]
            {
                GetClaim("CAN_SEND_EMAILS").Id,
                GetClaim("CAN_FILL_TIMESHEETS").Id,
            }
        };

        // Act
        var response = await TestingServer.PostAsync<RoleDetails>("api/authorization/roles/update-claims", request, "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Role not found");

        // Assert the database
        role = TestingServer.CreateContext().Set<Role>()
            .Include(x => x.Claims)
            .FirstOrDefault(x => x.Id == role.Id);

        role.ShouldNotBeNull();
        role.TenantId.ShouldBe("BUZIOS");
        role.Claims.ShouldNotBeNull();
        role.Claims.Count().ShouldBe(0);
    }

    /// <summary>
    /// The global admin should not be able to change the claims of a role that does not exist
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 4)]
    public async Task Global_Admin_Cannot_Change_Claims_Of_Role_That_Does_Not_Exist()
    {
        // Prepare 
        var request = new UpdateRoleClaims.Request
        {
            Id = Guid.NewGuid(),
            ClaimsIds = new[]
            {
                GetClaim("CAN_SEND_EMAILS").Id,
                GetClaim("CAN_FILL_TIMESHEETS").Id,
            }
        };

        // Act
        var response = await TestingServer.PostAsync<RoleDetails>("api/authorization/roles/update-claims", request, "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Role not found");
    }

    /// <summary>
    /// The global admin should not be able to change the claims of an application role using a null list of claims
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 5)]
    public async Task Global_Admin_Cannot_Change_Claims_Of_An_Application_Wide_Role_Using_Null_List()
    {
        // Prepare 
        var role = GetRole("General User");
        role.TenantId.ShouldBeNull();

        var request = new UpdateRoleClaims.Request
        {
            Id = role.Id,
            ClaimsIds = null!
        };

        // Act
        var response = await TestingServer.PostAsync<RoleDetails>("api/authorization/roles/update-claims", request, "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The list of claims must have at least one item");
    }

    #region tables

    /// -------------------------------------------------------------      -------------------------------------------------------------
    /// |                    ROLES TO CLAIMS                        |      |                    ROLES TO CLAIMS                        |
    /// -------------------------------------------------------------      -------------------------------------------------------------
    /// | ROLE                      |  CLAIM                        |      | ROLE                      |  CLAIM                        |
    /// -------------------------------------------------------------  >>  -------------------------------------------------------------
    /// |                           |                               |      | General User (No Tenant)  |  CAN_SEND_EMAILS              |
    /// |                           |                               |      | General User (No Tenant)  |  CAN_FILL_TIMESHEETS          |
    /// -------------------------------------------------------------      -------------------------------------------------------------

    #endregion

    /// <summary>
    /// The global admin should be able to change the claims of a role that exists both as tenant and application wide
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 6)]
    public async Task Global_Admin_Can_Set_Claims_Of_An_Application_Wide_Role()
    {
        // Prepare 
        var role = GetRole("General User");
        role.TenantId.ShouldBeNull();

        var request = new UpdateRoleClaims.Request
        {
            Id = role.Id,
            ClaimsIds = new[]
            {
                GetClaim("CAN_SEND_EMAILS").Id,
                GetClaim("CAN_FILL_TIMESHEETS").Id,
            }
        };

        // Act
        var response = await TestingServer.PostAsync<RoleDetails>("api/authorization/roles/update-claims", request, "superuser");

        // Assert the response
        response.ShouldBeSuccess(out var result);

        result.Id.ShouldBe(role.Id);
        result.Name.ShouldBe(role.Name);
        result.Claims.ShouldNotBeNull();
        result.Claims.Length.ShouldBe(2);
        result.Claims.FirstOrDefault(x => x.Name == "CAN_SEND_EMAILS").ShouldNotBeNull();
        result.Claims.FirstOrDefault(x => x.Name == "CAN_FILL_TIMESHEETS").ShouldNotBeNull();


        // Assert the database
        role = TestingServer.CreateContext().Set<Role>()
            .Include(x => x.Claims)
                .ThenInclude(x => x.Claim)
            .SingleOrDefault(x => x.Id == role.Id);

        role.ShouldNotBeNull();
        role.Name.ShouldBe(role.Name);
        role.TenantId.ShouldBeNull();
        role.Claims.ShouldNotBeNull();
        role.Claims.Count().ShouldBe(2);
        role.Claims.FirstOrDefault(x => x.Claim.Identification == "CAN_SEND_EMAILS").ShouldNotBeNull();
        role.Claims.FirstOrDefault(x => x.Claim.Identification == "CAN_FILL_TIMESHEETS").ShouldNotBeNull();

        // Assert the list endpoint
        var check2 = await TestingServer.GetAsync<RoleDetails[]>("api/authorization/roles", "superuser");
        check2.ShouldBeSuccess();
        check2.Data.ShouldNotBeNull();
        var roleToCheck = check2.Data.SingleOrDefault(x => x.Id == role.Id);
        roleToCheck.ShouldNotBeNull();
        roleToCheck.Name.ShouldBe(role.Name);
        roleToCheck.Claims.ShouldNotBeNull();
        roleToCheck.Claims.Length.ShouldBe(2);
        roleToCheck.Claims.FirstOrDefault(x => x.Name == "CAN_SEND_EMAILS").ShouldNotBeNull();
        roleToCheck.Claims.FirstOrDefault(x => x.Name == "CAN_FILL_TIMESHEETS").ShouldNotBeNull();
    }

    #region tables

    ///  -------------------------------------------------------------      -------------------------------------------------------------
    ///  |                    ROLES TO CLAIMS                        |      |                    ROLES TO CLAIMS                        |
    ///  -------------------------------------------------------------      -------------------------------------------------------------
    ///  | ROLE                      |  CLAIM                        |      | ROLE                      |  CLAIM                        |
    ///  -------------------------------------------------------------  >>  -------------------------------------------------------------
    ///  | General User (No Tenant)  |  CAN_SEND_EMAILS              |      | General User (No Tenant)  |  CAN_REQUEST_MATERIALS        |
    ///  | General User (No Tenant)  |  CAN_FILL_TIMESHEETS          |      | General User (No Tenant)  |  CAN_FILL_TIMESHEETS          |
    ///  -------------------------------------------------------------      -------------------------------------------------------------

    #endregion

    /// <summary>
    /// The global admin should be able to update the claims of a role that exists both as tenant and 
    /// application wide passing a list of new claims
    /// DEPENDENCIES: IT-057/IT-059
    /// </summary>
    [Test, NotInParallel(Order = 7)]
    public async Task Global_Admin_Can_Change_Claims_Of_An_Application_Wide_Role()
    {
        // Ensure the state of the entity is really correct
        var role = GetRole("General User");

        role = TestingServer.CreateContext().Set<Role>()
            .Include(x => x.Claims)
                .ThenInclude(x => x.Claim)
            .SingleOrDefault(x => x.Id == role.Id);

        role.ShouldNotBeNull();
        role.TenantId.ShouldBeNull();
        role.Name.ShouldBe(role.Name);
        role.Claims.ShouldNotBeNull();
        role.Claims.Count().ShouldBe(2);
        role.Claims.FirstOrDefault(x => x.Claim.Identification == "CAN_SEND_EMAILS").ShouldNotBeNull();
        role.Claims.FirstOrDefault(x => x.Claim.Identification == "CAN_FILL_TIMESHEETS").ShouldNotBeNull();

        // Prepare 
        var request = new UpdateRoleClaims.Request
        {
            Id = role.Id,
            ClaimsIds = new[]
            {
                GetClaim("CAN_REQUEST_MATERIALS").Id,
                GetClaim("CAN_FILL_TIMESHEETS").Id,
            }
        };

        // Act
        var response = await TestingServer.PostAsync<RoleDetails>("api/authorization/roles/update-claims", request, "superuser");

        // Assert the response
        response.ShouldBeSuccess(out var result);

        result.Id.ShouldBe(role.Id);
        result.Name.ShouldBe(role.Name);
        result.Claims.ShouldNotBeNull();
        result.Claims.Length.ShouldBe(2);
        result.Claims.FirstOrDefault(x => x.Name == "CAN_REQUEST_MATERIALS").ShouldNotBeNull();
        result.Claims.FirstOrDefault(x => x.Name == "CAN_FILL_TIMESHEETS").ShouldNotBeNull();


        // Assert the database
        role = TestingServer.CreateContext().Set<Role>()
            .Include(x => x.Claims)
                .ThenInclude(x => x.Claim)
            .SingleOrDefault(x => x.Id == role.Id);

        role.ShouldNotBeNull();
        role.TenantId.ShouldBeNull();
        role.Name.ShouldBe(role.Name);
        role.Claims.ShouldNotBeNull();
        role.Claims.Count().ShouldBe(2);
        role.Claims.FirstOrDefault(x => x.Claim.Identification == "CAN_REQUEST_MATERIALS").ShouldNotBeNull();
        role.Claims.FirstOrDefault(x => x.Claim.Identification == "CAN_FILL_TIMESHEETS").ShouldNotBeNull();

        // Assert the list endpoint
        var check2 = await TestingServer.GetAsync<RoleDetails[]>("api/authorization/roles", "superuser");
        check2.ShouldBeSuccess();
        check2.Data.ShouldNotBeNull();
        var roleToCheck = check2.Data.SingleOrDefault(x => x.Id == role.Id);
        roleToCheck.ShouldNotBeNull();
        roleToCheck.Name.ShouldBe(role.Name);
        roleToCheck.Claims.ShouldNotBeNull();
        roleToCheck.Claims.Length.ShouldBe(2);
        roleToCheck.Claims.FirstOrDefault(x => x.Name == "CAN_REQUEST_MATERIALS").ShouldNotBeNull();
        roleToCheck.Claims.FirstOrDefault(x => x.Name == "CAN_FILL_TIMESHEETS").ShouldNotBeNull();
    }

    #region tables

    ///  -------------------------------------------------------------
    ///  |                    ROLES TO CLAIMS                        |
    ///  -------------------------------------------------------------
    ///  | ROLE                      |  CLAIM                        |
    ///  -------------------------------------------------------------
    ///  | General User (No Tenant)  |  CAN_REQUEST_MATERIALS        |
    ///  | General User (No Tenant)  |  CAN_FILL_TIMESHEETS          |
    ///  -------------------------------------------------------------

    #endregion

    /// <summary>
    /// The global admin should be able to update the claims of a role that exists both as tenant and 
    /// application wide passing a list of new claims
    /// DEPENDENCIES: IT-062
    /// </summary>
    [Test, NotInParallel(Order = 8)]
    public async Task Global_Admin_Cannot_Change_Claims_Of_An_Application_Wide_Role_Using_a_Partially_Valid_List()
    {
        // Prepare 
        var role = GetRole("General User");
        role.TenantId.ShouldBeNull();

        var request = new UpdateRoleClaims.Request
        {
            Id = role.Id,
            ClaimsIds = new[]
            {
                GetClaim("CAN_REQUEST_MATERIALS").Id,
                Guid.NewGuid(),
                GetClaim("CAN_FILL_TIMESHEETS").Id,
                Guid.NewGuid(),
            }
        };

        // Act
        var response = await TestingServer.PostAsync<RoleDetails>("api/authorization/roles/update-claims", request, "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Unknown claim in the list");

        // Assert the database
        role = TestingServer.CreateContext().Set<Role>()
            .Include(x => x.Claims)
                .ThenInclude(x => x.Claim)
            .SingleOrDefault(x => x.Id == role.Id);

        role.ShouldNotBeNull();
        role.TenantId.ShouldBeNull();
        role.Name.ShouldBe(role.Name);
        role.Claims.ShouldNotBeNull();
        role.Claims.Count().ShouldBe(2);
        role.Claims.FirstOrDefault(x => x.Claim.Identification == "CAN_REQUEST_MATERIALS").ShouldNotBeNull();
        role.Claims.FirstOrDefault(x => x.Claim.Identification == "CAN_FILL_TIMESHEETS").ShouldNotBeNull();
    }

    #region tables

    ///  -------------------------------------------------------------      -------------------------------------------------------------
    ///  |                    ROLES TO CLAIMS                        |      |                    ROLES TO CLAIMS                        |
    ///  -------------------------------------------------------------      -------------------------------------------------------------
    ///  | ROLE                      |  CLAIM                        |      | ROLE                      |  CLAIM                        |
    ///  -------------------------------------------------------------  >>  -------------------------------------------------------------
    ///  | General User (No Tenant)  |  CAN_SEND_EMAILS              |      |                           |                               |
    ///  | General User (No Tenant)  |  CAN_FILL_TIMESHEETS          |      |                           |                               |
    ///  -------------------------------------------------------------      -------------------------------------------------------------

    #endregion

    /// <summary>
    /// The global admin should be able to remove all the claims of a role that exists both as tenant and application wide
    /// by passing an empty list of claims
    /// DEPENDENCIES: IT-062
    /// </summary>
    [Test, NotInParallel(Order = 9)]
    public async Task Global_Admin_Can_Clear_Claims_Of_An_Application_Wide_Role()
    {
        // Prepare 
        var role = GetRole("General User");
        role = TestingServer.CreateContext().Set<Role>()
            .Include(x => x.Claims)
                .ThenInclude(x => x.Claim)
            .SingleOrDefault(x => x.Id == role.Id);

        role.ShouldNotBeNull();
        role.TenantId.ShouldBeNull();
        role.Claims.Count().ShouldBe(2);

        var request = new UpdateRoleClaims.Request
        {
            Id = role.Id,
            ClaimsIds = new Guid[0]
        };

        // Act
        var response = await TestingServer.PostAsync<RoleDetails>("api/authorization/roles/update-claims", request, "superuser");

        // Assert the response
        response.ShouldBeSuccess(out var result);

        result.Id.ShouldBe(role.Id);
        result.Claims.ShouldNotBeNull();
        result.Claims.Length.ShouldBe(0);

        // Assert the database
        role = TestingServer.CreateContext().Set<Role>()
            .Include(x => x.Claims)
                .ThenInclude(x => x.Claim)
            .SingleOrDefault(x => x.Id == role.Id);

        role.ShouldNotBeNull();
        role.TenantId.ShouldBeNull();
        role.Claims.ShouldNotBeNull();
        role.Claims.Count().ShouldBe(0);

        // Assert the list endpoint
        var check2 = await TestingServer.GetAsync<RoleDetails[]>("api/authorization/roles", "superuser");
        check2.ShouldBeSuccess();
        check2.Data.ShouldNotBeNull();
        var roleToCheck = check2.Data.SingleOrDefault(x => x.Id == role.Id);
        roleToCheck.ShouldNotBeNull();
        roleToCheck.Claims.ShouldNotBeNull();
        roleToCheck.Claims.Length.ShouldBe(0);
    }

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }
}