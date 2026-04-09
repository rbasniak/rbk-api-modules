using Demo1.Tests;
using rbkApiModules.Identity.Core;

namespace rbkApiModules.Identity.Tests.Claims;

[NotInParallel(nameof(Global_Claim_Management_Tests))]
public class Global_Claim_Management_Tests
{
    [ClassDataSource<Demo1TestingServer>(Shared = SharedType.PerClass)]
    public required Demo1TestingServer TestingServer { get; set; } = default!;

    [Test, NotInParallel(Order = 1)]
    public async Task Seed()
    {
        await TestingServer.CacheCredentialsAsync("superuser", "admin", null);
        await TestingServer.CacheCredentialsAsync("admin1", "123", "buzios");
    }

    /// <summary>
    /// The global admin should be able to create a new application claim
    /// </summary>
    [Test, NotInParallel(Order = 2)]
    public async Task Global_Admin_Can_Create_Claim()
    {
        // Prepare
        var request = new CreateClaim.Request
        {
            Identification = "CAN_MAnaGE_APPrOVALS",
            Description = "Can approval documents",
            AllowApiKeyUsage = false,
        };

        // Act
        var response = await TestingServer.PostAsync<ClaimDetails>("api/authorization/claims", request, "superuser");

        // Assert the response
        response.ShouldBeSuccess(out var result);

        result.Id.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty.ToString());
        result.Identification.ShouldBe("CAN_MANAGE_APPROVALS");
        result.Description.ShouldBe("Can approval documents");
        result.IsProtected.ShouldBe(false);
        result.AllowApiKeyUsage.ShouldBe(false);

        // Assert the database
        var claim = TestingServer.CreateContext().Set<Claim>().FirstOrDefault(x => x.Id == new Guid(response.Data.Id));

        claim.Identification.ShouldBe("CAN_MANAGE_APPROVALS");
        claim.Description.ShouldBe("Can approval documents");
        claim.IsProtected.ShouldBe(false);
        claim.AllowApiKeyUsage.ShouldBe(false);
    }    

    /// <summary>
    /// The global admin should be able to update an application claim 
    /// </summary>
    [Test, NotInParallel(Order = 3)]
    public async Task Global_Admin_Can_Update_Claim()
    {
        // Prepare
        var existingClaim = TestingServer.CreateContext().Set<Claim>().FirstOrDefault(x => x.Identification == "CAN_MANAGE_APPROVALS");
        var request = new UpdateClaim.Request
        {
            Id = existingClaim.Id,
            Description = "Workflow approval",
            AllowApiKeyUsage = true,
        };

        // Act
        var response = await TestingServer.PutAsync<ClaimDetails>("api/authorization/claims", request, "superuser");

        // Assert the response
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.Id.ShouldNotBeNull();
        response.Data.Id.ShouldBe(existingClaim.Id.ToString());
        response.Data.Identification.ShouldBe("CAN_MANAGE_APPROVALS");
        response.Data.Description.ShouldBe("Workflow approval");
        response.Data.IsProtected.ShouldBe(false);
        response.Data.AllowApiKeyUsage.ShouldBe(true);

        // Assert the database
        var claim = TestingServer.CreateContext().Set<Claim>().FirstOrDefault(x => x.Id == existingClaim.Id);

        claim.Identification.ShouldBe("CAN_MANAGE_APPROVALS");
        claim.Description.ShouldBe("Workflow approval");
        claim.IsProtected.ShouldBe(false);
        claim.AllowApiKeyUsage.ShouldBe(true);
    }

    /// <summary>
    /// The global admin should be able to update an application claim 
    /// </summary>
    [Test, NotInParallel(Order = 4)]
    public async Task Global_Admin_Cannot_Create_Claim_With_Same_Identification()
    {
        // Prepare
        var request = new CreateClaim.Request
        {
            Identification = "CaN_MANAGE_aPPROVAlS",
            Description = "Workflow approval 2",
            AllowApiKeyUsage = false,
        };

        // Act
        var response = await TestingServer.PostAsync<ClaimDetails>("api/authorization/claims", request, "superuser");
        
        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "There is already a claim with this identification");
    }

    /// <summary>
    /// The global admin should be able to update an application claim 
    /// </summary>
    [Test, NotInParallel(Order = 5)]
    public async Task Global_Admin_Cannot_Create_Claim_With_Same_Description()
    {
        // Prepare
        var request = new CreateClaim.Request
        {
            Identification = "CaN_MaNaGe_aPPRoOVaLS_2",
            Description = "WORKFLOW APPROVAL",
            AllowApiKeyUsage = false,
        };

        // Act
        var response = await TestingServer.PostAsync<ClaimDetails>("api/authorization/claims", request, "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "There is already a claim with this description");
    }

    /// <summary>
    /// The global admin should be able to update an application claim with a description already being used
    /// </summary>
    [Test, NotInParallel(Order = 6)]
    public async Task Global_Admin_Cannot_Update_Claim_With_Same_Description()
    {
        // Prepare
        var existingClaim = TestingServer.CreateContext().Set<Claim>().SingleOrDefault(x => x.Identification == "CAN_MANAGE_APPROVALS");
        existingClaim.ShouldNotBeNull();

        var request = new UpdateClaim.Request
        {
            Id = existingClaim.Id,
            Description = "WORKFLOW APPROVAL",
            AllowApiKeyUsage = false,
        };

        // Act
        var response = await TestingServer.PutAsync<ClaimDetails>("api/authorization/claims", request, "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "There is already a claim with this description");
    }

    /// <summary>
    /// The global admin should be able to protect an application claim 
    /// </summary>
    [Test, NotInParallel(Order = 7)]
    public async Task Global_Admin_Can_Protect_Claim()
    {
        // Prepare
        var existingClaim = TestingServer.CreateContext().Set<Claim>().FirstOrDefault(x => x.Identification == "CAN_MANAGE_APPROVALS");
        existingClaim.IsProtected.ShouldBe(false);

        var request = new ProtectClaim.Request
        {
            Id = existingClaim.Id
        };

        // Act
        var response = await TestingServer.PostAsync<ClaimDetails>("api/authorization/claims/protect", request, "superuser");

        // Assert the response
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.Id.ShouldBe(existingClaim.Id.ToString());
        response.Data.Identification.ShouldBe(existingClaim.Identification);
        response.Data.Description.ShouldBe(existingClaim.Description);
        response.Data.IsProtected.ShouldBe(true);

        // Assert the database
        var claim = TestingServer.CreateContext().Set<Claim>().FirstOrDefault(x => x.Id == existingClaim.Id);

        claim.Identification.ShouldBe(existingClaim.Identification);
        claim.Description.ShouldBe(existingClaim.Description);
        claim.IsProtected.ShouldBe(true);
    }

    /// <summary>
    /// Global admin cannot delete a protected claim
    /// </summary>
    [Test, NotInParallel(Order = 8)]
    public async Task Global_Admin_Cannot_Delete_a_Protected_Claims()
    {
        // Prepare
        var existingClaim = TestingServer.CreateContext().Set<Claim>().Single(x => x.Identification == "CAN_MANAGE_APPROVALS");
        existingClaim.IsProtected.ShouldBe(true);

        // Act
        var response = await TestingServer.DeleteAsync($"api/authorization/claims/{existingClaim.Id}", "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Cannot remove a system protected claim");
    }

    /// <summary>
    /// The global admin should be able to unprotect an application claim 
    /// </summary>
    [Test, NotInParallel(Order = 9)]
    public async Task Global_Admin_Can_Unprotect_Claim()
    {
        // Prepare
        var existingClaim = TestingServer.CreateContext().Set<Claim>().FirstOrDefault(x => x.Identification == "CAN_MANAGE_APPROVALS");
        existingClaim.IsProtected.ShouldBe(true);

        var request = new UnprotectClaim.Request
        {
            Id = existingClaim.Id
        };

        // Act
        var response = await TestingServer.PostAsync<ClaimDetails>("api/authorization/claims/unprotect", request, "superuser");

        // Assert the response
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.Id.ShouldBe(existingClaim.Id.ToString());
        response.Data.Identification.ShouldBe(existingClaim.Identification);
        response.Data.Description.ShouldBe(existingClaim.Description);
        response.Data.IsProtected.ShouldBe(false);

        // Assert the database
        var claim = TestingServer.CreateContext().Set<Claim>().FirstOrDefault(x => x.Id == existingClaim.Id);

        claim.Identification.ShouldBe(existingClaim.Identification);
        claim.Description.ShouldBe(existingClaim.Description);
        claim.IsProtected.ShouldBe(false);
    }

    /// <summary>
    /// Local admin cannot delete claims 
    /// </summary>
    [Test, NotInParallel(Order = 10)]
    public async Task Local_Admin_Cannot_Delete_Claims()
    {
        // Prepare
        var existingClaim = TestingServer.CreateContext().Set<Claim>().Single(x => x.Identification == "CAN_MANAGE_APPROVALS");

        // Act
        var response = await TestingServer.DeleteAsync($"api/authorization/claims/{existingClaim.Id}", "admin1");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// Global admin can delete claims when they are not used yet
    /// </summary>
    [Test, NotInParallel(Order = 11)]
    public async Task Global_Admin_Can_Delete_Claims()
    {
        // Prepare
        var existingClaim = TestingServer.CreateContext().Set<Claim>().Single(x => x.Identification == "CAN_MANAGE_APPROVALS");

        // Act
        var response = await TestingServer.DeleteAsync($"api/authorization/claims/{existingClaim.Id}", "superuser");

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        var tenant = TestingServer.CreateContext().Set<Claim>().FirstOrDefault(x => x.Identification == existingClaim.Identification);

        tenant.ShouldBeNull();
    }

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    } 
}