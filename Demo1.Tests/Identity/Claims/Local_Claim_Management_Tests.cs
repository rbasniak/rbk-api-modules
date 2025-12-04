using Demo1.Tests;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Identity.Core;

namespace rbkApiModules.Identity.Tests.Claims;

[NotInParallel(nameof(Local_Claim_Management_Tests))]
public class Local_Claim_Management_Tests
{
    [ClassDataSource<Demo1TestingServer>(Shared = SharedType.PerClass)]
    public required Demo1TestingServer TestingServer { get; set; } = default!;

    [Test, NotInParallel(Order = 1)]
    public async Task Seed()
    {
        await TestingServer.CacheCredentialsAsync("superuser", "admin", null);
        await TestingServer.CacheCredentialsAsync("admin1", "123", "buzios");

        var context = TestingServer.CreateContext();
        context.Add(new Claim("CAN_MANAGE_APPROVALS", "can manage approvals"));
        context.SaveChanges();
    }

    /// <summary>
    /// The global admin should not be able to protect an application claim that doesn't exist
    /// </summary>
    [Test, NotInParallel(Order = 2)]
    public async Task Global_Admin_Cannot_Protect_Claim_That_Doesnt_Exist()
    {
        // Prepare
        var request = new ProtectClaim.Request
        {
            Id = Guid.NewGuid()
        };

        // Act
        var response = await TestingServer.PostAsync<ClaimDetails>("api/authorization/claims/protect", request, "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Claim not found");
    }

    /// <summary>
    /// The global admin should not be able to protect an application claim that doesn't exist
    /// </summary>
    [Test, NotInParallel(Order = 3)]
    public async Task Global_Admin_Cannot_Unprotect_Claim_That_Doesnt_Exist()
    {
        // Prepare
        var request = new UnprotectClaim.Request
        {
            Id = Guid.NewGuid()
        };

        // Act
        var response = await TestingServer.PostAsync<ClaimDetails>("api/authorization/claims/unprotect", request, "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Claim not found");
    }

    /// <summary>
    /// The global admin should not be able to delete an application claim that doesn't exist
    /// </summary>
    [Test, NotInParallel(Order = 4)]
    public async Task Global_Admin_Cannot_Delete_Claim_That_Doesnt_Exist()
    {
        // Act
        var response = await TestingServer.DeleteAsync($"api/authorization/claims/{Guid.NewGuid()}", "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Claim not found");
    }

    /// <summary>
    /// Local admin should not be able to create an application claim 
    /// </summary>
    [Test, NotInParallel(Order = 5)]
    public async Task Local_Admin_Cannot_Create_Claim()
    {
        // Prepare
        var request = new CreateClaim.Request
        {
            Identification = "CaN_MANAGE_aPPROVAlS",
            Description = "Workflow approval",
        };

        // Act
        var response = await TestingServer.PostAsync<ClaimDetails>("api/authorization/claims", request, "admin1");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// Local admin should not be able to update an application claim 
    /// </summary>
    [Test, NotInParallel(Order = 6)]
    public async Task Local_Admin_Cannot_Update_Claim()
    {
        // Prepare
        var existingClaim = TestingServer.CreateContext().Set<Claim>().FirstOrDefault(x => x.Identification == "CAN_MANAGE_APPROVALS");
        existingClaim.ShouldNotBeNull();

        var request = new CreateClaim.Request
        {
            Identification = existingClaim.Identification,
            Description = "Workflow approval 2",
        };

        // Act
        var response = await TestingServer.PutAsync<ClaimDetails>("api/authorization/claims", request, "admin1");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// Local admin cannot protect an appplication claim
    /// </summary>
    [Test, NotInParallel(Order = 7)]
    public async Task Local_Admin_Cannot_Protect_Claim()
    {
        // Prepare
        var existingClaim = TestingServer.CreateContext().Set<Claim>().FirstOrDefault(x => x.Identification == "CAN_MANAGE_APPROVALS");
        existingClaim.ShouldNotBeNull();

        var request = new ProtectClaim.Request
        {
            Id = existingClaim.Id
        };

        // Act
        var response = await TestingServer.PostAsync<ClaimDetails>("api/authorization/claims/protect", request, "admin1");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// Local admin cannot unprotect an appplication claim
    /// </summary>
    [Test, NotInParallel(Order = 8)]
    public async Task Local_Admin_Cannot_Unprotect_Claim()
    {
        // Prepare
        var existingClaim = TestingServer.CreateContext().Set<Claim>().FirstOrDefault(x => x.Identification == "CAN_MANAGE_APPROVALS");
        existingClaim.ShouldNotBeNull();

        var request = new UnprotectClaim.Request
        {
            Id = existingClaim.Id
        };

        // Act
        var response = await TestingServer.PostAsync<ClaimDetails>("api/authorization/claims/unprotect", request, "admin1");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// Global admin should not be able to create a claim without identification
    /// </summary>
    [Arguments(null)]
    [Arguments("")]
    [Test, NotInParallel(Order = 9)]
    public async Task Global_Admin_Cannot_Create_Claim_Without_Identification(string identification)
    {
        // Prepare
        var request = new CreateClaim.Request
        {
            Identification = identification,
            Description = "Workflow approval",
        };

        // Act
        var response = await TestingServer.PostAsync<ClaimDetails>("api/authorization/claims", request, "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "'Identification' must not be empty.");
    }

    /// <summary>
    /// Global admin should not be able to create a claim without identification
    /// </summary>
    [Arguments(null)]
    [Arguments("")]
    [Test, NotInParallel(Order = 10)]
    public async Task Global_Admin_Cannot_Create_Claim_Without_Description(string description)
    {
        // Prepare
        var request = new CreateClaim.Request
        {
            Identification = "VALID_IDENTIFICATION",
            Description = description,
        };

        // Act
        var response = await TestingServer.PostAsync<ClaimDetails>("api/authorization/claims", request, "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "'Description' must not be empty.");
    }

    /// <summary>
    /// Global admin should not be able to update a claim that doesn't exist
    /// </summary>
    [Test, NotInParallel(Order = 11)]
    public async Task Global_Admin_Cannot_Update_Claim_That_Does_Not_Exist()
    {
        // Prepare
        var request = new UpdateClaim.Request
        {
            Id = Guid.NewGuid(),
            Description = "Workflow approval",
        };

        // Act
        var response = await TestingServer.PutAsync<ClaimDetails>("api/authorization/claims", request, "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Claim not found");
    }

    /// <summary>
    /// Global admin should not be able to create a claim without identification
    /// </summary>
    [Arguments(null)]
    [Arguments("")]
    [Test, NotInParallel(Order = 12)]
    public async Task Global_Admin_Cannot_Update_Claim_Without_Description(string description)
    {
        // Prepare
        var existingClaim = TestingServer.CreateContext().Set<Claim>().FirstOrDefault(x => x.Identification == "CAN_MANAGE_APPROVALS");
        existingClaim.ShouldNotBeNull();

        var request = new UpdateClaim.Request
        {
            Id = existingClaim.Id,
            Description = description,
        };

        // Act
        var response = await TestingServer.PutAsync<ClaimDetails>("api/authorization/claims", request, "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "'Description' must not be empty.");
    }

    /// <summary>
    /// Global admin should be able to get the list of claims
    /// </summary>
    [Test, NotInParallel(Order = 13)]
    public async Task Global_Admin_Should_Be_Able_To_Query_The_Claims()
    {
        // Act
        var response = await TestingServer.GetAsync<ClaimDetails[]>("api/authorization/claims", "superuser");

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
    /// Local admin should be able to get the list of claims
    /// </summary>
    [Test, NotInParallel(Order = 14)]
    public async Task Local_Admin_Should_Be_Able_To_Query_The_Claims()
    {
        // Act
        var response = await TestingServer.GetAsync<ClaimDetails[]>("api/authorization/claims", "admin1");

        // Assert the response
        response.ShouldBeSuccess();
        response.Data.ShouldNotBeNull();
        response.Data.Length.ShouldBeGreaterThan(0);
        response.Data[0].GetType().ShouldBe(typeof(ClaimDetails));
    }

    /// <summary>
    /// The global admin should no be able to delete a claim used by a role
    /// </summary>
    [Test, NotInParallel(Order = 15)]
    public async Task Global_Admin_Cannot_Delete_a_Claim_Used_By_a_Role()
    {
        // Prepare
        var context = TestingServer.CreateContext();
        var claim = context.Add(new Claim("MY_NEW_CLAIM_1", "My new claim")).Entity;
        context.SaveChanges();

        var role = context.Set<Role>().Add(new Role("Test Role")).Entity;
        role.AddClaim(claim);

        context.SaveChanges();

        role = TestingServer.CreateContext().Set<Role>().Include(x => x.Claims).First(x => x.Name == role.Name);
        role.Claims.Count().ShouldBe(1);

        claim = TestingServer.CreateContext().Set<Claim>()
            .Include(x => x.Users)
            .Include(x => x.Roles)
            .First(x => x.Identification == "MY_NEW_CLAIM_1");
        claim.Users.Count().ShouldBe(0);
        claim.Roles.Count().ShouldBe(1);

        // Act
        var response = await TestingServer.DeleteAsync($"api/authorization/claims/{claim.Id}", "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Cannot remove a claim that is being used by any roles");
    }

    /// <summary>
    /// The global admin should be able to create a new application claim
    /// </summary>
    [Test, NotInParallel(Order = 16)]
    public async Task Global_Admin_Cannot_Delete_a_Claim_Used_By_An_User()
    {
        // Prepare
        var context = TestingServer.CreateContext();
        var claim = context.Add(new Claim("MY_NEW_CLAIM_2", "My new claim")).Entity;
        context.SaveChanges();

        var user = context.Set<User>().Include(x => x.Claims).First(x => x.TenantId == "BUZIOS");
        user.AddClaim(claim, ClaimAccessType.Allow);

        context.SaveChanges();

        user = TestingServer.CreateContext().Set<User>().Include(x => x.Claims).First(x => x.Username == user.Username && x.TenantId == user.TenantId);
        user.Claims.Count().ShouldBeGreaterThan(0);

        claim = TestingServer.CreateContext().Set<Claim>()
            .Include(x => x.Users)
            .Include(x => x.Roles)
            .First(x => x.Identification == "MY_NEW_CLAIM_2");
        claim.Users.Count().ShouldBe(1);
        claim.Roles.Count().ShouldBe(0);

        // Act
        var response = await TestingServer.DeleteAsync($"api/authorization/claims/{claim.Id}", "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Cannot remove a claim that is being used in any users");
    }

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }
}