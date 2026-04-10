using Demo2.Tests;
using rbkApiModules.Identity.Core;

namespace rbkApiModules.Identity.Tests.Claims;

[HumanFriendlyDisplayName]
public class Claim_Update_ApiKeyUsage_Tests
{
    [ClassDataSource<Demo2TestingServer>(Shared = SharedType.PerClass)]
    public required Demo2TestingServer TestingServer { get; set; } = default!;

    [Test, NotInParallel(Order = 1)]
    public async Task Seed()
    {
        await TestingServer.CacheCredentialsAsync("superuser", "admin", null);

        var request = new CreateClaim.Request
        {
            Identification = "CLAIM_APIKEY_USAGE_FLAG_TEST",
            Description = "Claim for API key usage flag tests",
            AllowApiKeyUsage = true,
        };

        var response = await TestingServer.PostAsync<ClaimDetails>("api/authorization/claims", request, "superuser");
        response.ShouldBeSuccess();
    }

    [Test, NotInParallel(Order = 2)]
    public async Task Can_Disable_AllowApiKeyUsage_When_No_ApiKey_Uses_The_Claim()
    {
        // Prepare
        var claim = TestingServer.CreateContext().Set<Claim>().First(x => x.Identification == "CLAIM_APIKEY_USAGE_FLAG_TEST");

        var request = new UpdateClaim.Request
        {
            Id = claim.Id,
            Description = claim.Description,
            AllowApiKeyUsage = false,
        };

        // Act
        var response = await TestingServer.PutAsync<ClaimDetails>("api/authorization/claims", request, "superuser");

        // Assert
        response.ShouldBeSuccess();
        response.Data.AllowApiKeyUsage.ShouldBe(false);

        var updated = TestingServer.CreateContext().Set<Claim>().First(x => x.Identification == "CLAIM_APIKEY_USAGE_FLAG_TEST");
        updated.AllowApiKeyUsage.ShouldBe(false);
    }

    [Test, NotInParallel(Order = 3)]
    public async Task Re_Enable_AllowApiKeyUsage_And_Assign_Claim_To_An_ApiKey()
    {
        // Re-enable so the claim can be assigned to an API key
        var claim = TestingServer.CreateContext().Set<Claim>().First(x => x.Identification == "CLAIM_APIKEY_USAGE_FLAG_TEST");

        var updateRequest = new UpdateClaim.Request
        {
            Id = claim.Id,
            Description = claim.Description,
            AllowApiKeyUsage = true,
        };

        var updateResponse = await TestingServer.PutAsync<ClaimDetails>("api/authorization/claims", updateRequest, "superuser");
        updateResponse.ShouldBeSuccess();

        // Create an API key that carries this claim
        var createKeyRequest = new CreateApiKey.Request
        {
            Name = "Key for AllowApiKeyUsage flag test",
            TenantId = null,
            ClaimIds = new List<Guid> { claim.Id },
        };

        var createKeyResponse = await TestingServer.PostAsync<CreateApiKey.Result>("api/authorization/api-keys", createKeyRequest, "superuser");
        createKeyResponse.ShouldBeSuccess();
    }

    [Test, NotInParallel(Order = 4)]
    public async Task Cannot_Disable_AllowApiKeyUsage_When_Claim_Is_Used_By_An_ApiKey()
    {
        // Prepare
        var claim = TestingServer.CreateContext().Set<Claim>().First(x => x.Identification == "CLAIM_APIKEY_USAGE_FLAG_TEST");

        var request = new UpdateClaim.Request
        {
            Id = claim.Id,
            Description = claim.Description,
            AllowApiKeyUsage = false,
        };

        // Act
        var response = await TestingServer.PutAsync<ClaimDetails>("api/authorization/claims", request, "superuser");

        // Assert - request rejected
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "There is already one or more api keys using this claim");

        // Assert - database unchanged
        var unchanged = TestingServer.CreateContext().Set<Claim>().First(x => x.Identification == "CLAIM_APIKEY_USAGE_FLAG_TEST");
        unchanged.AllowApiKeyUsage.ShouldBe(true);
    }
}
