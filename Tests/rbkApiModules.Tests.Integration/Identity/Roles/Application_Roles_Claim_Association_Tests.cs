namespace rbkApiModules.Tests.Integration.Identity;

/// <summary>
/// In general, the global admin can only manage roles that are application wide,
/// for tenant wide roles, only the local admins are able to manage
/// These tests deal with the update of list of claims for the roles
/// </summary>
[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class ApplicationRolesClaimAssociationTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public ApplicationRolesClaimAssociationTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

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
    [FriendlyNamedFact("IT-000"), Priority(-1)]
    public async Task Setup()
    {
        // Prepare the default access token
        await _serverFixture.GetAccessTokenAsync("superuser", "admin", null);

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

        var response1 = await _serverFixture.PostAsync<Roles.Details>("api/authorization/roles", new CreateRole.Request { Name = "General User" }, await _serverFixture.GetAccessTokenAsync("superuser", "admin", null));
        var response2 = await _serverFixture.PostAsync<Roles.Details>("api/authorization/roles", new CreateRole.Request { Name = "General User" }, await _serverFixture.GetAccessTokenAsync("admin1", "123", "Buzios"));
        var response3 = await _serverFixture.PostAsync<Roles.Details>("api/authorization/roles", new CreateRole.Request { Name = "Local Tenant Role" }, await _serverFixture.GetAccessTokenAsync("admin1", "123", "Buzios"));

        response1.ShouldBeSuccess();
        response2.ShouldBeSuccess();
        response3.ShouldBeSuccess();
    }

    private Claim GetClaim(string identification)
    {
        return _serverFixture.Context.Set<Claim>().Single(x => x.Identification.ToLower() == identification.ToLower());
    }

    private Role GetRole(string name, string tenant = null)
    {
        return _serverFixture.Context.Set<Role>().Single(x => x.Name.ToLower() == name.ToLower() && x.TenantId == tenant);
    }

    /// <summary>
    /// A local admin should not be able to change the claims of an application wide role
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-055"), Priority(0)]
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
        var response = await _serverFixture.PostAsync<Roles.Details>("api/authorization/roles/update-claims", request, await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Role not found");

        // Assert the database
        role = _serverFixture.Context.Set<Role>()
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
    [FriendlyNamedFact("IT-056"), Priority(0)]
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
        var response = await _serverFixture.PostAsync<Roles.Details>("api/authorization/roles/update-claims", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Role not found");

        // Assert the database
        role = _serverFixture.Context.Set<Role>()
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
    [FriendlyNamedFact("IT-058"), Priority(0)]
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
        var response = await _serverFixture.PostAsync<Roles.Details>("api/authorization/roles/update-claims", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Role not found");
    }

    /// <summary>
    /// The global admin should not be able to change the claims of an application role using a null list of claims
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-61"), Priority(0)]
    public async Task Global_Admin_Cannot_Change_Claims_Of_An_Application_Wide_Role_Using_Null_List()
    {
        // Prepare 
        var role = GetRole("General User");
        role.TenantId.ShouldBeNull();

        var request = new UpdateRoleClaims.Request
        {
            Id = role.Id,
            ClaimsIds = null
        };

        // Act
        var response = await _serverFixture.PostAsync<Roles.Details>("api/authorization/roles/update-claims", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The list of claims must be provided");
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
    [FriendlyNamedFact("IT-057/IT-059"), Priority(10)]
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
        var response = await _serverFixture.PostAsync<Roles.Details>("api/authorization/roles/update-claims", request, true);

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
        role.Name.ShouldBe(role.Name);
        role.TenantId.ShouldBeNull();
        role.Claims.ShouldNotBeNull();
        role.Claims.Count().ShouldBe(2);
        role.Claims.FirstOrDefault(x => x.Claim.Identification == "CAN_SEND_EMAILS").ShouldNotBeNull();
        role.Claims.FirstOrDefault(x => x.Claim.Identification == "CAN_FILL_TIMESHEETS").ShouldNotBeNull();

        // Assert the list endpoint
        var check2 = await _serverFixture.GetAsync<Roles.Details[]>("api/authorization/roles", true);
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
    ///  | General User (No Tenant)  |  CAN_SEND_EMAILS              |      | General User (No Tenant)  |  CAN_REQUEST_MATERIALS        |
    ///  | General User (No Tenant)  |  CAN_FILL_TIMESHEETS          |      | General User (No Tenant)  |  CAN_FILL_TIMESHEETS          |
    ///  -------------------------------------------------------------      -------------------------------------------------------------

    #endregion

    /// <summary>
    /// The global admin should be able to update the claims of a role that exists both as tenant and 
    /// application wide passing a list of new claims
    /// DEPENDENCIES: IT-057/IT-059
    /// </summary>
    [FriendlyNamedFact("IT-062"), Priority(20)]
    public async Task Global_Admin_Can_Change_Claims_Of_An_Application_Wide_Role()
    {
        // Ensure the state of the entity is really correct
        var role = GetRole("General User");

        role = _serverFixture.Context.Set<Role>()
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
        var response = await _serverFixture.PostAsync<Roles.Details>("api/authorization/roles/update-claims", request, true);

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
        role.TenantId.ShouldBeNull();
        role.Name.ShouldBe(role.Name);
        role.Claims.ShouldNotBeNull();
        role.Claims.Count().ShouldBe(2);
        role.Claims.FirstOrDefault(x => x.Claim.Identification == "CAN_REQUEST_MATERIALS").ShouldNotBeNull();
        role.Claims.FirstOrDefault(x => x.Claim.Identification == "CAN_FILL_TIMESHEETS").ShouldNotBeNull();

        // Assert the list endpoint
        var check2 = await _serverFixture.GetAsync<Roles.Details[]>("api/authorization/roles", true);
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
    ///  | General User (No Tenant)  |  CAN_REQUEST_MATERIALS        |
    ///  | General User (No Tenant)  |  CAN_FILL_TIMESHEETS          |
    ///  -------------------------------------------------------------

    #endregion

    /// <summary>
    /// The global admin should be able to update the claims of a role that exists both as tenant and 
    /// application wide passing a list of new claims
    /// DEPENDENCIES: IT-062
    /// </summary>
    [FriendlyNamedFact("IT-063"), Priority(30)]
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
        var response = await _serverFixture.PostAsync<Roles.Details>("api/authorization/roles/update-claims", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Unknown claim in the list");

        // Assert the database
        role = _serverFixture.Context.Set<Role>()
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
    [FriendlyNamedFact("IT-060"), Priority(40)]
    public async Task Global_Admin_Can_Clear_Claims_Of_An_Application_Wide_Role()
    {
        // Prepare 
        var role = GetRole("General User");
        role = _serverFixture.Context.Set<Role>()
            .Include(x => x.Claims)
                .ThenInclude(x => x.Claim)
            .SingleOrDefault(x => x.Id == role.Id);
        role.TenantId.ShouldBeNull();
        role.Claims.Count().ShouldBe(2);

        var request = new UpdateRoleClaims.Request
        {
            Id = role.Id,
            ClaimsIds = new Guid[0]
        };

        // Act
        var response = await _serverFixture.PostAsync<Roles.Details>("api/authorization/roles/update-claims", request, true);

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
        role.TenantId.ShouldBeNull();
        role.Claims.ShouldNotBeNull();
        role.Claims.Count().ShouldBe(0);

        // Assert the list endpoint
        var check2 = await _serverFixture.GetAsync<Roles.Details[]>("api/authorization/roles", true);
        check2.ShouldBeSuccess();
        check2.Data.ShouldNotBeNull();
        var roleToCheck = check2.Data.SingleOrDefault(x => x.Id == role.Id.ToString());
        roleToCheck.ShouldNotBeNull();
        roleToCheck.Claims.ShouldNotBeNull();
        roleToCheck.Claims.Length.ShouldBe(0);
    }
}