using rbkApiModules.Identity.Core.DataTransfer.Claims;
using rbkApiModules.Identity.Core.DataTransfer.Users;

namespace rbkApiModules.Tests.Integration.Identity;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class ClaimOverridingTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public ClaimOverridingTests(ServerFixture serverFixture)
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
    /// Admin cannot override a non-existant claim for a given user
    /// </summary>
    [FriendlyNamedFact("IT-200"), Priority(10)]
    public async Task Admin_Cannot_Override_Claim_That_Does_Not_Exist()
    {
        // Prepare
        var user = _serverFixture.Context.Set<User>().Include(x => x.Claims).First(x => x.Username == "john.doe" && x.TenantId == "BUZIOS");

        var request = new AddClaimOverride.Request
        {
            Username = user.Username,
            AccessType = ClaimAccessType.Block,
            ClaimIds = new[] { Guid.NewGuid() }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authorization/users/add-claims", request, await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Claim not found");
    }

    /// <summary>
    /// Admin cannot override a claim for user that does not exist
    /// </summary>
    [FriendlyNamedFact("IT-205/IT-206"), Priority(20)]
    public async Task Admin_Cannot_Override_Claim_For_a_User_That_Does_Not_Exist()
    {
        // Prepare
        var user = _serverFixture.Context.Set<User>().Include(x => x.Claims).First(x => x.Username == "tony.stark" && x.TenantId == "UN-BS"); // User exists, but in another tenant
        var claim = _serverFixture.Context.Set<Claim>().First(x => x.Identification == AuthenticationClaims.MANAGE_USERS);

        var request = new AddClaimOverride.Request
        {
            Username = user.Username,
            AccessType = ClaimAccessType.Block,
            ClaimIds = new[] { claim.Id }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authorization/users/add-claims", request, await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "User not found.");
    }

    /// <summary>
    /// Admin can override a claim for a given user
    /// </summary>
    [FriendlyNamedFact("IT-202/IT-203"), Priority(30)]
    public async Task Admin_Can_Override_Claim_For_a_Given_User()
    {
        // Prepare
        var user = _serverFixture.Context.Set<User>().Include(x => x.Claims).First(x => x.Username == "john.doe" && x.TenantId == "BUZIOS");
        var claim = _serverFixture.Context.Set<Claim>().First(x => x.Identification == AuthenticationClaims.MANAGE_USERS);

        var request = new AddClaimOverride.Request
        {
            Username = user.Username,
            AccessType = ClaimAccessType.Allow,
            ClaimIds = new[] { claim.Id }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authorization/users/add-claims", request, await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios"));

        // Assert the response
        response.ShouldBeSuccess();
        response.Data.ShouldNotBeNull();
        var overrideClaim = response.Data.OverridedClaims.FirstOrDefault(x => x.Claim.Id == claim.Id.ToString());
        overrideClaim.ShouldNotBeNull();
        overrideClaim.Access.Id.ShouldBe((int)ClaimAccessType.Allow);
        overrideClaim.Claim.Id.ShouldBe(claim.Id.ToString());

        // Assert the database
        user = _serverFixture.Context.Set<User>()
            .Include(x => x.Claims)
            .ThenInclude(x => x.Claim)
            .First(x => x.Username == "john.doe" && x.TenantId == "BUZIOS");

        var overrideEntity = user.Claims.SingleOrDefault(x => x.ClaimId == claim.Id && x.UserId == user.Id);
        overrideEntity.ShouldNotBeNull();
        overrideEntity.Access.ShouldBe(ClaimAccessType.Allow);

        // Confirm that the user with same name in another tenant hasn't been changed
        user = _serverFixture.Context.Set<User>()
            .Include(x => x.Claims)
            .ThenInclude(x => x.Claim)
            .First(x => x.Username == "john.doe" && x.TenantId == "UN-BS");

        overrideEntity = user.Claims.SingleOrDefault(x => x.ClaimId == claim.Id && x.UserId == user.Id);
        overrideEntity.ShouldBeNull();
    }

    /// <summary>
    /// Admin can override a claim for a given user that already has that override but in another access type
    /// then the override will just be updated
    /// </summary>
    [FriendlyNamedFact("IT-204"), Priority(40)]
    public async Task Admin_Can_Override_Claim_For_a_Given_User_That_Already_Has_That_Override()
    {
        // Prepare
        var user = _serverFixture.Context.Set<User>()
            .Include(x => x.Claims)
            .ThenInclude(x => x.Claim)
            .First(x => x.Username == "john.doe" && x.TenantId == "BUZIOS");

        var claim = _serverFixture.Context.Set<Claim>().First(x => x.Identification == AuthenticationClaims.MANAGE_USERS);

        user.Claims.Single(x => x.ClaimId == claim.Id && x.UserId == user.Id).Access.ShouldBe(ClaimAccessType.Allow);

        var request = new AddClaimOverride.Request
        {
            Username = user.Username,
            AccessType = ClaimAccessType.Block,
            ClaimIds = new[] { claim.Id }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authorization/users/add-claims", request, await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios"));

        // Assert the response
        response.ShouldBeSuccess();
        response.Data.ShouldNotBeNull();
        var overrideClaim = response.Data.OverridedClaims.FirstOrDefault(x => x.Claim.Id == claim.Id.ToString());
        overrideClaim.ShouldNotBeNull();
        overrideClaim.Access.Id.ShouldBe((int)ClaimAccessType.Block);
        overrideClaim.Claim.Id.ShouldBe(claim.Id.ToString());

        // Assert the database
        user = _serverFixture.Context.Set<User>()
            .Include(x => x.Claims)
            .ThenInclude(x => x.Claim)
            .First(x => x.Username == "john.doe" && x.TenantId == "BUZIOS");

        var overrideEntity = user.Claims.SingleOrDefault(x => x.ClaimId == claim.Id && x.UserId == user.Id);
        overrideEntity.ShouldNotBeNull();
        overrideEntity.Access.ShouldBe(ClaimAccessType.Block);

        // Confirm that the user with same name in another tenant hasn't been changed
        user = _serverFixture.Context.Set<User>()
            .Include(x => x.Claims)
            .ThenInclude(x => x.Claim)
            .First(x => x.Username == "john.doe" && x.TenantId == "UN-BS");

        overrideEntity = user.Claims.SingleOrDefault(x => x.ClaimId == claim.Id && x.UserId == user.Id);
        overrideEntity.ShouldBeNull();
    }

    /// <summary>
    /// Global admin cannot override a claims for any user
    /// </summary>
    [FriendlyNamedFact("IT-201"), Priority(50)]
    public async Task Global_Admin_Cannot_Override_Claim_For_Any_User()
    {
        // Prepare
        var user = _serverFixture.Context.Set<User>().Include(x => x.Claims).First(x => x.Username == "john.doe" && x.TenantId == "BUZIOS");
        var claim = _serverFixture.Context.Set<Claim>().First(x => x.Identification == AuthenticationClaims.MANAGE_USERS);

        var request = new AddClaimOverride.Request
        {
            Username = user.Username,
            AccessType = ClaimAccessType.Allow,
            ClaimIds = new[] { claim.Id }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authorization/users/add-claims", request, await _serverFixture.GetAccessTokenAsync("superuser", "admin", null));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// Admin cannot remove an override of a claim that does not exist
    /// DEPENDENCIES: IT-201
    /// </summary>
    [FriendlyNamedFact("IT-207"), Priority(60)]
    public async Task Admin_Cannot_Remove_Overrided_Claim_That_Does_Not_Exist()
    {
        // Prepare
        var request = new RemoveClaimOverride.Request
        {
            Username = "john.doe",
            ClaimIds = new[] { Guid.NewGuid() }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authorization/users/remove-claims", request, await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Claim not found"); 
    }

    /// <summary>
    /// Global admin cannot remove an override of a claim for any users
    /// DEPENDENCIES: IT-201
    /// </summary>
    [FriendlyNamedFact("IT-208"), Priority(70)]
    public async Task Global_Admin_Cannot_Remove_Overrided_For_Any_Users()
    {
        // Prepare
        var user = _serverFixture.Context.Set<User>()
            .Include(x => x.Claims)
            .ThenInclude(x => x.Claim)
            .First(x => x.Username == "john.doe" && x.TenantId == "BUZIOS");

        var claim = _serverFixture.Context.Set<Claim>().First(x => x.Identification == AuthenticationClaims.MANAGE_USERS);

        user.Claims.SingleOrDefault(x => x.ClaimId == claim.Id && x.UserId == user.Id).ShouldNotBeNull();

        var request = new RemoveClaimOverride.Request
        {
            Username = user.Username,
            ClaimIds = new[] { claim.Id }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authorization/users/remove-claims", request, await _serverFixture.GetAccessTokenAsync("superuser", "admin", null));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// Admin can remove an overrided claim given the right conditions
    /// DEPENDENCIES: IT-201
    /// </summary>
    [FriendlyNamedFact("IT-209"), Priority(80)]
    public async Task Admin_Can_Remove_Overrided_Claim()
    {
        // Prepare
        var user = _serverFixture.Context.Set<User>()
            .Include(x => x.Claims)
            .ThenInclude(x => x.Claim)
            .First(x => x.Username == "john.doe" && x.TenantId == "BUZIOS");

        var claim = _serverFixture.Context.Set<Claim>().First(x => x.Identification == AuthenticationClaims.MANAGE_USERS);

        user.Claims.SingleOrDefault(x => x.ClaimId == claim.Id && x.UserId == user.Id).ShouldNotBeNull();

        var request = new RemoveClaimOverride.Request
        {
            Username = user.Username,
            ClaimIds = new[] { claim.Id }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authorization/users/remove-claims", request, await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios"));

        // Assert the response
        response.ShouldBeSuccess();
        response.Data.ShouldNotBeNull();
        var overrideClaim = response.Data.OverridedClaims.FirstOrDefault(x => x.Claim.Id == claim.Id.ToString());
        overrideClaim.ShouldBeNull();

        // Assert the database
        user = _serverFixture.Context.Set<User>()
            .Include(x => x.Claims)
            .ThenInclude(x => x.Claim)
            .First(x => x.Username == "john.doe" && x.TenantId == "BUZIOS");

        var overrideEntity = user.Claims.SingleOrDefault(x => x.ClaimId == claim.Id && x.UserId == user.Id);
        overrideEntity.ShouldBeNull();

        // Confirm that the user with same name in another tenant hasn't been changed
        user = _serverFixture.Context.Set<User>()
            .Include(x => x.Claims)
            .ThenInclude(x => x.Claim)
            .First(x => x.Username == "john.doe" && x.TenantId == "UN-BS");

        overrideEntity = user.Claims.SingleOrDefault(x => x.ClaimId == claim.Id && x.UserId == user.Id);
        overrideEntity.ShouldBeNull();
    }

    /// <summary>
    /// Admin cannot remove an overrided claim from a user from another tenant
    /// DEPENDENCIES: IT-201
    /// </summary>
    [FriendlyNamedFact("IT-210/IT-211"), Priority(90)]
    public async Task Admin_Cannot_Remove_Overrided_Claim_From_Another_Tenant()
    {
        // Prepare
        var claim = _serverFixture.Context.Set<Claim>().First(x => x.Identification == AuthenticationClaims.MANAGE_USERS);

        var request = new RemoveClaimOverride.Request
        {
            Username = "tony.stark",
            ClaimIds = new[] { claim.Id }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authorization/users/remove-claims", request, await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "User not found.");
    }

    /// <summary>
    /// Admin cannot remove an override that does not exist in the user
    /// DEPENDENCIES: IT-209
    /// </summary>
    [FriendlyNamedFact("IT-212"), Priority(100)]
    public async Task Admin_Cannot_Remove_Overrided_Claim_That_Was_Not_Overrided_In_The_User()
    {
        // Prepare
        var user = _serverFixture.Context.Set<User>()
            .Include(x => x.Claims)
            .ThenInclude(x => x.Claim)
            .First(x => x.Username == "john.doe" && x.TenantId == "BUZIOS");

        var claim = _serverFixture.Context.Set<Claim>().First(x => x.Identification == AuthenticationClaims.MANAGE_USERS);

        user.Claims.SingleOrDefault(x => x.ClaimId == claim.Id && x.UserId == user.Id).ShouldBeNull();

        var request = new RemoveClaimOverride.Request
        {
            Username = user.Username,
            ClaimIds = new[] { claim.Id }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authorization/users/remove-claims", request, await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Claim is not overrided in the user");

        // Assert the database
        user = _serverFixture.Context.Set<User>()
            .Include(x => x.Claims)
            .ThenInclude(x => x.Claim)
            .First(x => x.Username == "john.doe" && x.TenantId == "BUZIOS");

        var overrideEntity = user.Claims.SingleOrDefault(x => x.ClaimId == claim.Id && x.UserId == user.Id);
        overrideEntity.ShouldBeNull();

        // Confirm that the user with same name in another tenant hasn't been changed
        user = _serverFixture.Context.Set<User>()
            .Include(x => x.Claims)
            .ThenInclude(x => x.Claim)
            .First(x => x.Username == "john.doe" && x.TenantId == "UN-BS");

        overrideEntity = user.Claims.SingleOrDefault(x => x.ClaimId == claim.Id && x.UserId == user.Id);
        overrideEntity.ShouldBeNull();
    }
}
 
