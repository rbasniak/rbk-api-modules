﻿using rbkApiModules.Identity.Core.DataTransfer.Claims;
using rbkApiModules.Identity.Core.DataTransfer.Roles;

namespace rbkApiModules.Demo1.Tests.Integration.Identity;

/// <summary>
/// In general, the global admin can only manage roles that are application wide,
/// for tenant wide roles, only the local admins are able to manage
/// These tests deal with the update of list of claims for the roles
/// </summary>
[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class TenantRolesClaimAssociationTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public TenantRolesClaimAssociationTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    #region tables

    /// -------------------------------------   -------------------------------------   
    /// |              ROLES                |   |              CLAIMS               |   
    /// -------------------------------------   -------------------------------------   
    /// | ID | TENANT | NAME                |   | ID |  NAME                        |   
    /// -------------------------------------   -------------------------------------   
    /// |    | NULL   | General User        |   |    |  CAN_FILL_TIMESHEETS      |   
    /// |    | BUZIOS | General User        |   |    |  CAN_REQUEST_MATERIALS       |   
    /// |    | NULL   | Administrator       |   |    |  CAN_SEND_EMAILS             |   
    /// |    | UN-ES  | General User        |   -------------------------------------   
    /// -------------------------------------                                        
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
    [FriendlyNamedFact("IT-000"), Priority(-1)]
    public async Task Setup()
    {
        // Prepare the default access token
        await _serverFixture.GetAccessTokenAsync("admin1", "123", "BUzios");

        var createClaimCommands = new[]
        {
            new CreateClaim.Request { Identification = "CAN_FILL_TIMESHEETS", Description = "CAN_FILL_TIMESHEETS" },
            new CreateClaim.Request { Identification = "CAN_REQUEST_MATERIALS", Description = "CAN_REQUEST_MATERIALS" },
            new CreateClaim.Request { Identification = "CAN_SEND_EMAILS", Description = "CAN_SEND_EMAILS" },
        };

        foreach (var command in createClaimCommands) 
        {
            var response = await _serverFixture.PostAsync<ClaimDetails>("api/authorization/claims", command, await _serverFixture.GetAccessTokenAsync("superuser", "admin", null));
            response.ShouldBeSuccess();
        } 

        var response1 = await _serverFixture.PostAsync<RoleDetails>("api/authorization/roles", new CreateRole.Request { Name = "Administrator" }, await _serverFixture.GetAccessTokenAsync("superuser", "admin", null));
        var response2 = await _serverFixture.PostAsync<RoleDetails>("api/authorization/roles", new CreateRole.Request { Name = "General User" }, await _serverFixture.GetAccessTokenAsync("superuser", "admin", null));
        var response3 = await _serverFixture.PostAsync<RoleDetails>("api/authorization/roles", new CreateRole.Request { Name = "General User" }, await _serverFixture.GetAccessTokenAsync("admin1", "123", "Buzios"));
        var response4 = await _serverFixture.PostAsync<RoleDetails>("api/authorization/roles", new CreateRole.Request { Name = "General User" }, await _serverFixture.GetAccessTokenAsync("admin1", "123", "un-BS"));

        response1.ShouldBeSuccess();
        response2.ShouldBeSuccess();
        response3.ShouldBeSuccess();
        response4.ShouldBeSuccess();
    }

    private Claim GetClaim(string identification) => _serverFixture.Context.Set<Claim>().Single(x => x.Identification.ToLower() == identification.ToLower());

    private Role GetRole(string name, string tenant = null) => _serverFixture.Context.Set<Role>().Single(x => x.Name.ToLower() == name.ToLower() && x.TenantId == tenant);

    /// <summary>
    /// A local admin should not be able to change the claims of a role that does not exist as a tenant wide role
    /// but exist as an application role
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-064"), Priority(0)]
    public async Task Local_Admin_Cannot_Change_Claims_of_a_Tenant_Wide_Role_That_Does_Not_Exist_as_Tenant_Role()
    {
        // Prepare 
        var role = GetRole("Administrator");
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
        var response = await _serverFixture.PostAsync<RoleDetails>("api/authorization/roles/update-claims", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Role not found");

        // Assert the database
        role = _serverFixture.Context.Set<Role>()
            .Include(x => x.Claims)
            .FirstOrDefault(x => x.Id == role.Id);

        role.ShouldNotBeNull();
        role.Claims.ShouldNotBeNull();
        role.Claims.Count().ShouldBe(0);
    }

    /// <summary>
    /// A local admin should not be able to change the claims of a role that does not exist at all
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-065"), Priority(0)]
    public async Task Local_Admin_Cannot_Change_Claims_Of_Role_That_Does_Not_Exist_At_All()
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
        var response = await _serverFixture.PostAsync<RoleDetails>("api/authorization/roles/update-claims", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Role not found");
    }

    /// <summary>
    /// A local admin should not be able to change the claims of a tenant role using a null list of claims
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-066"), Priority(0)]
    public async Task Local_Admin_Cannot_Change_Claims_Of_An_Application_Wide_Role_Using_Null_List()
    {
        // Prepare 
        var role = GetRole("General User", "BUZIOS");
        role.TenantId.ShouldBe("BUZIOS");

        var request = new UpdateRoleClaims.Request
        {
            Id = role.Id,
            ClaimsIds = null
        };

        // Act
        var response = await _serverFixture.PostAsync<RoleDetails>("api/authorization/roles/update-claims", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The list of claims must have at least one item");
    }

    #region tables

    /// -------------------------------------------------------------      -------------------------------------------------------------
    /// |                    ROLES TO CLAIMS                        |      |                    ROLES TO CLAIMS                        |
    /// -------------------------------------------------------------      -------------------------------------------------------------
    /// | ROLE                      |  CLAIM                        |      | ROLE                      |  CLAIM                        |
    /// -------------------------------------------------------------  >>  -------------------------------------------------------------
    /// |                           |                               |      | General User (BUZIOS)     |  CAN_SEND_EMAILS              |
    /// |                           |                               |      | General User (BUZIOS)     |  CAN_FILL_TIMESHEETS          |
    /// -------------------------------------------------------------      -------------------------------------------------------------

    #endregion

    /// <summary>
    /// A local admin should be able to change the claims of a role that exists both as tenant and application wide
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-067/IT-068"), Priority(10)]
    public async Task Local_Admin_Can_Set_Claims_Of_a_Tenant_Wide_Role()
    {
        // Prepare 
        var role = GetRole("General User", "BUZIOS");
        role.TenantId.ShouldBe("BUZIOS");

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
        var response = await _serverFixture.PostAsync<RoleDetails>("api/authorization/roles/update-claims", request, true);

        // Assert the response
        response.ShouldBeSuccess();

        response.Data.Id.ShouldBe(role.Id.ToString());
        response.Data.Name.ShouldBe(role.Name);
        response.Data.Claims.ShouldNotBeNull();
        response.Data.Claims.Length.ShouldBe(2);
        response.Data.Claims.FirstOrDefault(x => x.Name == "CAN_SEND_EMAILS").ShouldNotBeNull();
        response.Data.Claims.FirstOrDefault(x => x.Name == "CAN_FILL_TIMESHEETS").ShouldNotBeNull();


        // Assert the database
        role = _serverFixture.Context.Set<Role>()
            .Include(x => x.Claims)
                .ThenInclude(x => x.Claim)
            .SingleOrDefault(x => x.Id == role.Id);

        role.ShouldNotBeNull();
        role.TenantId.ShouldBe("BUZIOS");
        role.Name.ShouldBe(role.Name);
        role.Claims.ShouldNotBeNull();
        role.Claims.Count().ShouldBe(2);
        role.Claims.FirstOrDefault(x => x.Claim.Identification == "CAN_SEND_EMAILS").ShouldNotBeNull();
        role.Claims.FirstOrDefault(x => x.Claim.Identification == "CAN_FILL_TIMESHEETS").ShouldNotBeNull();

        // Assert the list endpoint
        var check2 = await _serverFixture.GetAsync<RoleDetails[]>("api/authorization/roles", true);
        check2.ShouldBeSuccess();
        check2.Data.ShouldNotBeNull();
        var roleToCheck = check2.Data.SingleOrDefault(x => x.Id == role.Id.ToString());
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
    ///  | General User (BUZIOS)     |  CAN_SEND_EMAILS              |      | General User (BUZIOS)     |  CAN_REQUEST_MATERIALS        |
    ///  | General User (BUZIOS)     |  CAN_FILL_TIMESHEETS          |      | General User (BUZIOS)     |  CAN_FILL_TIMESHEETS          |
    ///  -------------------------------------------------------------      -------------------------------------------------------------

    #endregion

    /// <summary>
    /// A local admin should be able to update the claims of a role that exists both as tenant and 
    /// application wide passing a list of new claims
    /// DEPENDENCIES: IT-067/IT-068
    /// </summary>
    [FriendlyNamedFact("IT-069"), Priority(20)]
    public async Task Local_Admin_Can_Change_Claims_Of_An_Application_Wide_Role()
    {
        // Ensure the state of the entity is really correct
        var role = GetRole("General User", "BUZIOS");

        role = _serverFixture.Context.Set<Role>()
            .Include(x => x.Claims)
                .ThenInclude(x => x.Claim)
            .SingleOrDefault(x => x.Id == role.Id);

        role.ShouldNotBeNull();
        role.TenantId.ShouldBe("BUZIOS");
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
        var response = await _serverFixture.PostAsync<RoleDetails>("api/authorization/roles/update-claims", request, true);

        // Assert the response
        response.ShouldBeSuccess();

        response.Data.Id.ShouldBe(role.Id.ToString());
        response.Data.Name.ShouldBe(role.Name);
        response.Data.Claims.ShouldNotBeNull();
        response.Data.Claims.Length.ShouldBe(2);
        response.Data.Claims.FirstOrDefault(x => x.Name == "CAN_REQUEST_MATERIALS").ShouldNotBeNull();
        response.Data.Claims.FirstOrDefault(x => x.Name == "CAN_FILL_TIMESHEETS").ShouldNotBeNull();


        // Assert the database
        role = _serverFixture.Context.Set<Role>()
            .Include(x => x.Claims)
                .ThenInclude(x => x.Claim)
            .SingleOrDefault(x => x.Id == role.Id);

        role.ShouldNotBeNull();
        role.Name.ShouldBe(role.Name);
        role.TenantId.ShouldBe("BUZIOS");
        role.Claims.ShouldNotBeNull();
        role.Claims.Count().ShouldBe(2);
        role.Claims.FirstOrDefault(x => x.Claim.Identification == "CAN_REQUEST_MATERIALS").ShouldNotBeNull();
        role.Claims.FirstOrDefault(x => x.Claim.Identification == "CAN_FILL_TIMESHEETS").ShouldNotBeNull();

        // Assert the list endpoint
        var check2 = await _serverFixture.GetAsync<RoleDetails[]>("api/authorization/roles", true);
        check2.ShouldBeSuccess();
        check2.Data.ShouldNotBeNull();
        var roleToCheck = check2.Data.SingleOrDefault(x => x.Id == role.Id.ToString());
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
    ///  | General User (BUZIOS)     |  CAN_REQUEST_MATERIALS        |
    ///  | General User (BUZIOS)     |  CAN_FILL_TIMESHEETS          |
    ///  -------------------------------------------------------------

    #endregion

    /// <summary>
    /// A local admin should be able to update the claims of a role that exists both as tenant and 
    /// application wide passing a list of new claims
    /// DEPENDENCIES: IT-069
    /// </summary>
    [FriendlyNamedFact("IT-070"), Priority(30)]
    public async Task Local_Admin_Cannot_Change_Claims_Of_An_Application_Wide_Role_Using_a_Partially_Valid_List()
    {
        // Prepare 
        var role = GetRole("General User", "BUZIOS");
        role.TenantId.ShouldBe("BUZIOS");

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
        var response = await _serverFixture.PostAsync<RoleDetails>("api/authorization/roles/update-claims", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Unknown claim in the list");

        // Assert the database
        role = _serverFixture.Context.Set<Role>()
            .Include(x => x.Claims)
                .ThenInclude(x => x.Claim)
            .SingleOrDefault(x => x.Id == role.Id);

        role.ShouldNotBeNull();
        role.Name.ShouldBe(role.Name);
        role.TenantId.ShouldBe("BUZIOS");
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
    ///  | General User (BUZIOS)     |  CAN_SEND_EMAILS              |      |                           |                               |
    ///  | General User (BUZIOS)     |  CAN_FILL_TIMESHEETS          |      |                           |                               |
    ///  -------------------------------------------------------------      -------------------------------------------------------------

    #endregion

    /// <summary>
    /// A local admin should be able to remove all the claims of a role that exists both as tenant and application wide
    /// by passing an empty list of claims
    /// DEPENDENCIES: IT-070
    /// </summary>
    [FriendlyNamedFact("IT-071"), Priority(40)]
    public async Task Local_Admin_Can_Clear_Claims_Of_An_Application_Wide_Role()
    {
        // Prepare 
        var role = GetRole("General User", "BUZIOS");
        role = _serverFixture.Context.Set<Role>()
            .Include(x => x.Claims)
                .ThenInclude(x => x.Claim)
            .SingleOrDefault(x => x.Id == role.Id);
        role.TenantId.ShouldBe("BUZIOS");
        role.Claims.Count().ShouldBe(2);

        var request = new UpdateRoleClaims.Request
        {
            Id = role.Id,
            ClaimsIds = new Guid[0]
        };

        // Act
        var response = await _serverFixture.PostAsync<RoleDetails>("api/authorization/roles/update-claims", request, true);

        // Assert the response
        response.ShouldBeSuccess();

        response.Data.Id.ShouldBe(role.Id.ToString());
        response.Data.Claims.ShouldNotBeNull();
        response.Data.Claims.Length.ShouldBe(0);

        // Assert the database
        role = _serverFixture.Context.Set<Role>()
            .Include(x => x.Claims)
                .ThenInclude(x => x.Claim)
            .SingleOrDefault(x => x.Id == role.Id);

        role.ShouldNotBeNull();
        role.TenantId.ShouldBe("BUZIOS");
        role.Claims.ShouldNotBeNull();
        role.Claims.Count().ShouldBe(0);

        // Assert the list endpoint
        var check2 = await _serverFixture.GetAsync<RoleDetails[]>("api/authorization/roles", true);
        check2.ShouldBeSuccess();
        check2.Data.ShouldNotBeNull();
        var roleToCheck = check2.Data.SingleOrDefault(x => x.Id == role.Id.ToString());
        roleToCheck.ShouldNotBeNull();
        roleToCheck.Claims.ShouldNotBeNull();
        roleToCheck.Claims.Length.ShouldBe(0);
    }
}