using rbkApiModules.Identity.Core.DataTransfer.Claims;

namespace rbkApiModules.Tests.Integration.Identity;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class ClaimManagementDependentTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public ClaimManagementDependentTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    /// <summary>
    /// Login call, just to store the access tokens for future tests to use
    /// </summary>
    [FriendlyNamedFact("IT-000"), Priority(-1)]
    public async Task Login()
    {
        await _serverFixture.GetAccessTokenAsync("superuser", "admin", null);
    }

    /// <summary>
    /// The global admin should be able to create a new application claim
    /// </summary>
    [FriendlyNamedFact("IT-001"), Priority(10)]
    public async Task Global_Admin_Can_Create_Claim()
    {
        // Prepare
        var request = new CreateClaim.Request
        {
            Identification = "CAN_MAnaGE_APPrOVALS",
            Description = "Can approval documents",
        };

        // Act
        var response = await _serverFixture.PostAsync<ClaimDetails>("api/authorization/claims", request, authenticated: true);

        // Assert the response
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.Id.ShouldNotBeNull();
        response.Data.Id.ShouldNotBe(Guid.Empty.ToString());
        response.Data.Identification.ShouldBe("CAN_MANAGE_APPROVALS");
        response.Data.Description.ShouldBe("Can approval documents");
        response.Data.IsProtected.ShouldBe(false);

        // Assert the database
        var claim = _serverFixture.Context.Set<Claim>().FirstOrDefault(x => x.Id == new Guid(response.Data.Id));

        claim.Identification.ShouldBe("CAN_MANAGE_APPROVALS");
        claim.Description.ShouldBe("Can approval documents");
        claim.IsProtected.ShouldBe(false);
    }

    /// <summary>
    /// The global admin should be able to update an application claim 
    /// </summary>
    [FriendlyNamedFact("IT-002"), Priority(20)]
    public async Task Global_Admin_Can_Update_Claim()
    {
        // Prepare
        var existingClaim = _serverFixture.Context.Set<Claim>().FirstOrDefault(x => x.Identification == "CAN_MANAGE_APPROVALS");
        var request = new UpdateClaim.Request
        {
            Id = existingClaim.Id,
            Description = "Workflow approval",
        };

        // Act
        var response = await _serverFixture.PutAsync<ClaimDetails>("api/authorization/claims", request, authenticated: true);

        // Assert the response
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.Id.ShouldNotBeNull();
        response.Data.Id.ShouldBe(existingClaim.Id.ToString());
        response.Data.Identification.ShouldBe("CAN_MANAGE_APPROVALS");
        response.Data.Description.ShouldBe("Workflow approval");
        response.Data.IsProtected.ShouldBe(false);

        // Assert the database
        var claim = _serverFixture.Context.Set<Claim>().FirstOrDefault(x => x.Id == existingClaim.Id);

        claim.Identification.ShouldBe("CAN_MANAGE_APPROVALS");
        claim.Description.ShouldBe("Workflow approval");
        claim.IsProtected.ShouldBe(false);
    }

    /// <summary>
    /// The global admin should be able to update an application claim 
    /// </summary>
    [FriendlyNamedFact("IT-003"), Priority(30)]
    public async Task Global_Admin_Cannot_Create_Claim_With_Same_Identification()
    {
        // Prepare
        var request = new CreateClaim.Request
        {
            Identification = "CaN_MANAGE_aPPROVAlS",
            Description = "Workflow approval 2",
        };

        // Act
        var response = await _serverFixture.PostAsync<ClaimDetails>("api/authorization/claims", request, authenticated: true);
        
        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "There is already a claim with this identification");
    }

    /// <summary>
    /// The global admin should be able to update an application claim 
    /// </summary>
    [FriendlyNamedFact("IT-004"), Priority(35)]
    public async Task Global_Admin_Cannot_Create_Claim_With_Same_Description()
    {
        // Prepare
        var request = new CreateClaim.Request
        {
            Identification = "CaN_MaNaGe_aPPRoOVaLS_2",
            Description = "WORKFLOW APPROVAL",
        };

        // Act
        var response = await _serverFixture.PostAsync<ClaimDetails>("api/authorization/claims", request, authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "There is already a claim with this description");
    }

    /// <summary>
    /// The global admin should be able to update an application claim with a description already being used
    /// </summary>
    [FriendlyNamedFact("IT-102"), Priority(37)]
    public async Task Global_Admin_Cannot_Update_Claim_With_Same_Description()
    {
        // Prepare
        var existingClaim = _serverFixture.Context.Set<Claim>().SingleOrDefault(x => x.Identification == "CAN_MANAGE_APPROVALS");
        existingClaim.ShouldNotBeNull();

        var request = new UpdateClaim.Request
        {
            Id = existingClaim.Id,
            Description = "WORKFLOW APPROVAL",
        };

        // Act
        var response = await _serverFixture.PutAsync<ClaimDetails>("api/authorization/claims", request, authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "There is already a claim with this description");
    }

    /// <summary>
    /// The global admin should be able to protect an application claim 
    /// </summary>
    [FriendlyNamedFact("IT-005"), Priority(40)]
    public async Task Global_Admin_Can_Protect_Claim()
    {
        // Prepare
        var existingClaim = _serverFixture.Context.Set<Claim>().FirstOrDefault(x => x.Identification == "CAN_MANAGE_APPROVALS");
        existingClaim.IsProtected.ShouldBe(false);

        var request = new ProtectClaim.Request
        {
            Id = existingClaim.Id
        };

        // Act
        var response = await _serverFixture.PostAsync<ClaimDetails>("api/authorization/claims/protect", request, authenticated: true);

        // Assert the response
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.Id.ShouldBe(existingClaim.Id.ToString());
        response.Data.Identification.ShouldBe(existingClaim.Identification);
        response.Data.Description.ShouldBe(existingClaim.Description);
        response.Data.IsProtected.ShouldBe(true);

        // Assert the database
        var claim = _serverFixture.Context.Set<Claim>().FirstOrDefault(x => x.Id == existingClaim.Id);

        claim.Identification.ShouldBe(existingClaim.Identification);
        claim.Description.ShouldBe(existingClaim.Description);
        claim.IsProtected.ShouldBe(true);
    }

    /// <summary>
    /// Global admin cannot delete a protected claim
    /// </summary>
    [FriendlyNamedFact("IT-094"), Priority(45)]
    public async Task Global_Admin_Cannot_Delete_a_Protected_Claims()
    {
        // Prepare
        var existingClaim = _serverFixture.Context.Set<Claim>().Single(x => x.Identification == "CAN_MANAGE_APPROVALS");
        existingClaim.IsProtected.ShouldBe(true);

        // Act
        var response = await _serverFixture.DeleteAsync($"api/authorization/claims/{existingClaim.Id}", authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Cannot remove a system protected claim");
    }

    /// <summary>
    /// The global admin should be able to unprotect an application claim 
    /// </summary>
    [FriendlyNamedFact("IT-006"), Priority(50)]
    public async Task Global_Admin_Can_Unprotect_Claim()
    {
        // Prepare
        var existingClaim = _serverFixture.Context.Set<Claim>().FirstOrDefault(x => x.Identification == "CAN_MANAGE_APPROVALS");
        existingClaim.IsProtected.ShouldBe(true);

        var request = new UnprotectClaim.Request
        {
            Id = existingClaim.Id
        };

        // Act
        var response = await _serverFixture.PostAsync<ClaimDetails>("api/authorization/claims/unprotect", request, authenticated: true);

        // Assert the response
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.Id.ShouldBe(existingClaim.Id.ToString());
        response.Data.Identification.ShouldBe(existingClaim.Identification);
        response.Data.Description.ShouldBe(existingClaim.Description);
        response.Data.IsProtected.ShouldBe(false);

        // Assert the database
        var claim = _serverFixture.Context.Set<Claim>().FirstOrDefault(x => x.Id == existingClaim.Id);

        claim.Identification.ShouldBe(existingClaim.Identification);
        claim.Description.ShouldBe(existingClaim.Description);
        claim.IsProtected.ShouldBe(false);
    }

    /// <summary>
    /// Local admin cannot delete claims 
    /// </summary>
    [FriendlyNamedFact("IT-093"), Priority(70)]
    public async Task Local_Admin_Cannot_Delete_Claims()
    {
        // Prepare
        var existingClaim = _serverFixture.Context.Set<Claim>().Single(x => x.Identification == "CAN_MANAGE_APPROVALS");

        // Act
        var response = await _serverFixture.DeleteAsync($"api/authorization/claims/{existingClaim.Id}", await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// Global admin can delete claims when they are not used yet
    /// </summary>
    [FriendlyNamedFact("IT-010"), Priority(90)]
    public async Task Global_Admin_Can_Delete_Claims()
    {
        // Prepare
        var existingClaim = _serverFixture.Context.Set<Claim>().Single(x => x.Identification == "CAN_MANAGE_APPROVALS");

        // Act
        var response = await _serverFixture.DeleteAsync($"api/authorization/claims/{existingClaim.Id}", authenticated: true);

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        var tenant = _serverFixture.Context.Set<Claim>().FirstOrDefault(x => x.Identification == existingClaim.Identification);

        tenant.ShouldBeNull();
    }
}

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class ClaimManagementIndependentTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public ClaimManagementIndependentTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    /// <summary>
    /// Login call, just to store the access tokens for future tests to use
    /// </summary>
    [FriendlyNamedFact("IT-000"), Priority(-1)]
    public async Task Login()
    {
        await _serverFixture.GetAccessTokenAsync("superuser", "admin", null);

        var context = _serverFixture.Context;
        context.Add(new Claim("CAN_MANAGE_APPROVALS", "can manage approvals"));
        context.SaveChanges();
    }

    /// <summary>
    /// The global admin should not be able to protect an application claim that doesn't exist
    /// </summary>
    [FriendlyNamedFact("IT-007"), Priority(10)]
    public async Task Global_Admin_Cannot_Protect_Claim_That_Doesnt_Exist()
    {
        // Prepare
        var request = new ProtectClaim.Request
        {
            Id = Guid.NewGuid()
        };

        // Act
        var response = await _serverFixture.PostAsync<ClaimDetails>("api/authorization/claims/protect", request, authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Claim not found");
    }

    /// <summary>
    /// The global admin should not be able to protect an application claim that doesn't exist
    /// </summary>
    [FriendlyNamedFact("IT-008"), Priority(20)]
    public async Task Global_Admin_Cannot_Unprotect_Claim_That_Doesnt_Exist()
    {
        // Prepare
        var request = new UnprotectClaim.Request
        {
            Id = Guid.NewGuid()
        };

        // Act
        var response = await _serverFixture.PostAsync<ClaimDetails>("api/authorization/claims/unprotect", request, authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Claim not found");
    }

    /// <summary>
    /// The global admin should not be able to delete an application claim that doesn't exist
    /// </summary>
    [FriendlyNamedFact("IT-009"), Priority(30)]
    public async Task Global_Admin_Cannot_Delete_Claim_That_Doesnt_Exist()
    {
        // Act
        var response = await _serverFixture.DeleteAsync($"api/authorization/claims/{Guid.NewGuid()}", authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Claim not found");
    }

    /// <summary>
    /// Local admin should not be able to create an application claim 
    /// </summary>
    [FriendlyNamedFact("IT-090"), Priority(40)]
    public async Task Local_Admin_Cannot_Create_Claim()
    {
        // Prepare
        var request = new CreateClaim.Request
        {
            Identification = "CaN_MANAGE_aPPROVAlS",
            Description = "Workflow approval",
        };

        // Act
        var response = await _serverFixture.PostAsync<ClaimDetails>("api/authorization/claims", request, await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// Local admin should not be able to update an application claim 
    /// </summary>
    [FriendlyNamedFact("IT-097"), Priority(50)]
    public async Task Local_Admin_Cannot_Update_Claim()
    {
        // Prepare
        var existingClaim = _serverFixture.Context.Set<Claim>().FirstOrDefault(x => x.Identification == "CAN_MANAGE_APPROVALS");
        existingClaim.ShouldNotBeNull();

        var request = new CreateClaim.Request
        {
            Identification = existingClaim.Identification,
            Description = "Workflow approval 2",
        };

        // Act
        var response = await _serverFixture.PutAsync<ClaimDetails>("api/authorization/claims", request, await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// Local admin cannot protect an appplication claim
    /// </summary>
    [FriendlyNamedFact("IT-095"), Priority(60)]
    public async Task Local_Admin_Cannot_Protect_Claim()
    {
        // Prepare
        var existingClaim = _serverFixture.Context.Set<Claim>().FirstOrDefault(x => x.Identification == "CAN_MANAGE_APPROVALS");
        existingClaim.ShouldNotBeNull();

        var request = new ProtectClaim.Request
        {
            Id = existingClaim.Id
        };

        // Act
        var response = await _serverFixture.PostAsync<ClaimDetails>("api/authorization/claims/protect", request, await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// Local admin cannot unprotect an appplication claim
    /// </summary>
    [FriendlyNamedFact("IT-096"), Priority(70)]
    public async Task Local_Admin_Cannot_Unprotect_Claim()
    {
        // Prepare
        var existingClaim = _serverFixture.Context.Set<Claim>().FirstOrDefault(x => x.Identification == "CAN_MANAGE_APPROVALS");
        existingClaim.ShouldNotBeNull();

        var request = new UnprotectClaim.Request
        {
            Id = existingClaim.Id
        };

        // Act
        var response = await _serverFixture.PostAsync<ClaimDetails>("api/authorization/claims/unprotect", request, await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// Global admin should not be able to create a claim without identification
    /// </summary>
    [InlineData(null)]
    [InlineData("")]
    [FriendlyNamedTheory("IT-091"), Priority(80)]
    public async Task Global_Admin_Cannot_Create_Claim_Without_Identification(string identification)
    {
        // Prepare
        var request = new CreateClaim.Request
        {
            Identification = identification,
            Description = "Workflow approval",
        };

        // Act
        var response = await _serverFixture.PostAsync<ClaimDetails>("api/authorization/claims", request, authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "O campo 'Identification' não pode ser vazio");
    }

    /// <summary>
    /// Global admin should not be able to create a claim without identification
    /// </summary>
    [InlineData(null)]
    [InlineData("")]
    [FriendlyNamedTheory("IT-092"), Priority(90)]
    public async Task Global_Admin_Cannot_Create_Claim_Without_Description(string description)
    {
        // Prepare
        var request = new CreateClaim.Request
        {
            Identification = "VALID_IDENTIFICATION",
            Description = description,
        };

        // Act
        var response = await _serverFixture.PostAsync<ClaimDetails>("api/authorization/claims", request, authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "O campo 'Description' não pode ser vazio");
    }

    /// <summary>
    /// Global admin should not be able to update a claim that doesn't exist
    /// </summary>
    [FriendlyNamedFact("IT-098"), Priority(100)]
    public async Task Global_Admin_Cannot_Update_Claim_That_Does_Not_Exist()
    {
        // Prepare
        var request = new UpdateClaim.Request
        {
            Id = Guid.NewGuid(),
            Description = "Workflow approval",
        };

        // Act
        var response = await _serverFixture.PutAsync<ClaimDetails>("api/authorization/claims", request, authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Claim not found");
    }

    /// <summary>
    /// Global admin should not be able to create a claim without identification
    /// </summary>
    [InlineData(null)]
    [InlineData("")]
    [FriendlyNamedTheory("IT-099"), Priority(110)]
    public async Task Global_Admin_Cannot_Update_Claim_Without_Description(string description)
    {
        // Prepare
        var existingClaim = _serverFixture.Context.Set<Claim>().FirstOrDefault(x => x.Identification == "CAN_MANAGE_APPROVALS");
        existingClaim.ShouldNotBeNull();

        var request = new UpdateClaim.Request
        {
            Id = existingClaim.Id,
            Description = description,
        };

        // Act
        var response = await _serverFixture.PutAsync<ClaimDetails>("api/authorization/claims", request, authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "O campo 'Description' não pode ser vazio");
    }

    /// <summary>
    /// Global admin should be able to get the list of claims
    /// </summary>
    [FriendlyNamedFact("IT-100"), Priority(120)]
    public async Task Global_Admin_Should_Be_Able_To_Query_The_Claims()
    {
        // Act
        var response = await _serverFixture.GetAsync<ClaimDetails[]>("api/authorization/claims", authenticated: true);

        // Assert the response
        response.ShouldBeSuccess();
        response.Data.ShouldNotBeNull();
        response.Data.Length.ShouldBeGreaterThan(0);
        response.Data[0].ShouldNotBeNull();

        foreach (var claim in response.Data)
        {
            String.IsNullOrEmpty(claim.Description).ShouldBeFalse();
            String.IsNullOrEmpty(claim.Identification).ShouldBeFalse();
        }

        // Ensure hidden roles are not showing
        response.Data.FirstOrDefault(x => x.Identification == AuthenticationClaims.CHANGE_CLAIM_PROTECTION).ShouldBeNull();
        response.Data.FirstOrDefault(x => x.Identification == AuthenticationClaims.MANAGE_CLAIMS).ShouldBeNull();
        response.Data.FirstOrDefault(x => x.Identification == AuthenticationClaims.MANAGE_TENANTS).ShouldBeNull();
        response.Data.FirstOrDefault(x => x.Identification == AuthenticationClaims.MANAGE_APPLICATION_WIDE_ROLES).ShouldBeNull();
    }

    /// <summary>
    /// Local admin should not be able to get the list of claims
    /// </summary>
    [FriendlyNamedFact("IT-101"), Priority(130)]
    public async Task Local_Admin_Should_Not_Be_Able_To_Query_The_Claims()
    {
        // Act
        var response = await _serverFixture.GetAsync<ClaimDetails[]>("api/authorization/claims", await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// The global admin should no be able to delete a claim used by a role
    /// </summary>
    [FriendlyNamedFact("IT-220"), Priority(140)]
    public async Task Global_Admin_Cannot_Delete_a_Claim_Used_By_a_Role()
    {
        // Prepare
        var context = _serverFixture.Context;
        var claim = context.Add(new Claim("MY_NEW_CLAIM_1", "My new claim")).Entity;
        context.SaveChanges();

        var role = context.Set<Role>().Add(new Role("Test Role")).Entity;
        role.AddClaim(claim);

        context.SaveChanges();

        role = _serverFixture.Context.Set<Role>().Include(x => x.Claims).First(x => x.Name == role.Name);
        role.Claims.Count().ShouldBe(1);

        claim = _serverFixture.Context.Set<Claim>()
            .Include(x => x.Users)
            .Include(x => x.Roles)
            .First(x => x.Identification == "MY_NEW_CLAIM_1");
        claim.Users.Count().ShouldBe(0);
        claim.Roles.Count().ShouldBe(1);

        // Act
        var response = await _serverFixture.DeleteAsync($"api/authorization/claims/{claim.Id}", authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Cannot remove a claim that is being used by any roles");
    }

    /// <summary>
    /// The global admin should be able to create a new application claim
    /// </summary>
    [FriendlyNamedFact("IT-221"), Priority(140)]
    public async Task Global_Admin_Cannot_Delete_a_Claim_Used_By_An_User()
    {
        // Prepare
        var context = _serverFixture.Context;
        var claim = context.Add(new Claim("MY_NEW_CLAIM_2", "My new claim")).Entity;
        context.SaveChanges();

        var user = context.Set<User>().Include(x => x.Claims).First(x => x.TenantId == "BUZIOS");
        user.AddClaim(claim, ClaimAccessType.Allow);

        context.SaveChanges();

        user = _serverFixture.Context.Set<User>().Include(x => x.Claims).First(x => x.Username == user.Username && x.TenantId == user.TenantId);
        user.Claims.Count().ShouldBeGreaterThan(0);

        claim = _serverFixture.Context.Set<Claim>()
            .Include(x => x.Users)
            .Include(x => x.Roles)
            .First(x => x.Identification == "MY_NEW_CLAIM_2");
        claim.Users.Count().ShouldBe(1); 
        claim.Roles.Count().ShouldBe(0); 

        // Act
        var response = await _serverFixture.DeleteAsync($"api/authorization/claims/{claim.Id}", authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Cannot remove a claim that is being used in any users");
    }
}


