using rbkApiModules.Identity.Core.DataTransfer.Roles;
using rbkApiModules.Identity.Core.DataTransfer.Tenants;
using System.Text.Json;

namespace rbkApiModules.Tests.Integration.Identity;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class TenantManagementTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public TenantManagementTests(ServerFixture serverFixture)
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
    /// The global admin should be able to create a new tenant called ACME
    /// </summary>
    [FriendlyNamedFact("IT-075"), Priority(5)]
    public async Task Global_Admin_Cannot_Create_Tenant_If_Password_Does_Not_Fit_Policies()
    {
        // Prepare 
        var request = new CreateTenant.Request
        {
            Alias = "Acme",
            Name = "Acme Inc.",
            Metadata = "{ \"city\": \"Aalborg\" }",
            AdminInfo = new CreateTenant.AdminUser
            {
                DisplayName = "Acme Administrator",
                Email = "acme@acme.com",
                Password = "1",
                Username = "acme.admin"
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<TenantDetails>("api/authorization/tenants", request, authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The password must have at least 3 characteres");
    }

    /// <summary>
    /// The global admin should be able to create a new tenant called ACME
    /// </summary>
    [FriendlyNamedFact("IT-021"), Priority(10)]
    public async Task Global_Admin_Can_Create_Tenant()
    {
        // Prepare 
        var request = new CreateTenant.Request
        {
            Alias = "Acme",
            Name = "Acme Inc.",
            Metadata = "{ \"city\": \"Aalborg\" }",
            AdminInfo = new CreateTenant.AdminUser
            {
                DisplayName = "Acme Administrator",
                Email = "acme@acme.com",
                Password = "12345",
                Username = "acme.admin"
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<TenantDetails>("api/authorization/tenants", request, authenticated: true);

        // Assert the response
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.Alias.ShouldBe("ACME");
        response.Data.Name.ShouldBe("Acme Inc.");

        // Assert the database
        var tenant = _serverFixture.Context.Set<Tenant>().FirstOrDefault(x => x.Alias == response.Data.Alias);

        tenant.ShouldNotBeNull();
        tenant.Alias.ShouldBe("ACME");
        tenant.Name.ShouldBe("Acme Inc.");

        var metadata = JsonSerializer.Deserialize<JsonElement>(tenant.Metadata.ToString());
        metadata.GetProperty("city").GetString().ShouldBe("Aalborg");

        var user = await _serverFixture.Context.Set<User>().Include(x => x.Claims).ThenInclude(x => x.Claim).FirstOrDefaultAsync(x => x.Username == "acme.admin" && x.TenantId == "ACME");
        user.ShouldNotBeNull();
        user.Claims.Count().ShouldBe(4);
        user.Claims.SingleOrDefault(x => x.Claim.Identification == AuthenticationClaims.MANAGE_USERS && x.Access == ClaimAccessType.Allow).ShouldNotBeNull();
        user.Claims.SingleOrDefault(x => x.Claim.Identification == AuthenticationClaims.MANAGE_TENANT_SPECIFIC_ROLES && x.Access == ClaimAccessType.Allow).ShouldNotBeNull();
        user.Claims.SingleOrDefault(x => x.Claim.Identification == AuthenticationClaims.MANAGE_USER_ROLES && x.Access == ClaimAccessType.Allow).ShouldNotBeNull();
        user.Claims.SingleOrDefault(x => x.Claim.Identification == AuthenticationClaims.OVERRIDE_USER_CLAIMS && x.Access == ClaimAccessType.Allow).ShouldNotBeNull();
    }

    /// <summary>
    /// The global admin should not be able to create a new tenant without an alias
    /// </summary>
    [InlineData("")]
    [InlineData(null)]
    [FriendlyNamedTheory("IT-081"), Priority(20)]
    public async Task Global_Admin_Cannot_Create_Tenant_Without_Alias(string alias)
    {
        // Prepare 
        var request = new CreateTenant.Request
        {
            Alias = alias,
            Name = "Acme Inc.",
            Metadata = "{ \"city\": \"Aalborg\" }",
            AdminInfo = new CreateTenant.AdminUser
            {
                DisplayName = "Acme Administrator",
                Email = "acme@acme.com",
                Password = "12345",
                Username = "acme.admin"
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<TenantDetails>("api/authorization/tenants", request, authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "O campo 'Alias' não pode ser vazio");
    }

    /// <summary>
    /// The global admin should not be able to create a new tenant without a name
    /// </summary>
    [InlineData("")]
    [InlineData(null)]
    [FriendlyNamedTheory("IT-082"), Priority(30)]
    public async Task Global_Admin_Cannot_Create_Tenant_Without_Name(string name)
    {
        // Prepare 
        var request = new CreateTenant.Request
        {
            Alias = "ALIAS",
            Name = name,
            AdminInfo = new CreateTenant.AdminUser
            {
                DisplayName = "Acme Administrator",
                Email = "acme@acme.com",
                Password = "12345",
                Username = "acme.admin"
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<TenantDetails>("api/authorization/tenants", request, authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "O campo 'Name' não pode ser vazio");
    }

    /// <summary>
    /// A local admin should not be able to create a new tenant 
    /// </summary>
    [FriendlyNamedFact("IT-080"), Priority(40)]
    public async Task Local_Admin_Can_Create_Tenant()
    {
        // Prepare 
        var request = new CreateTenant.Request
        {
            Alias = "NOT ALLOWED",
            Name = "This wil not be created",
        };

        // Act
        var response = await _serverFixture.PostAsync<TenantDetails>("api/authorization/tenants", request, await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// A local admin should not be able to update a tenant
    /// </summary>
    [FriendlyNamedFact("IT-088"), Priority(50)]
    public async Task Local_Admin_Cannot_Update_Tenant()
    {
        // Prepare
        var request = new UpdateTenant.Request
        {
            Alias = "AcmE",
            Name = "Acme Industries",
            Metadata = "{ \"city\": \"Auhrus\" }"
        };

        // Act
        var response = await _serverFixture.PutAsync<TenantDetails>("api/authorization/tenants", request, await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// The global admin should be able to update the existing ACME tenant
    /// </summary>
    [FriendlyNamedFact("IT-022"), Priority(60)]
    public async Task Global_Admin_Can_Update_Tenant()
    {
        // Prepare
        var request = new UpdateTenant.Request
        {
            Alias = "AcmE",
            Name = "Acme Industries",
            Metadata = "{ \"city\": \"Auhrus\" }"
        };

        // Act
        var response = await _serverFixture.PutAsync<TenantDetails>("api/authorization/tenants", request, authenticated: true);

        // Assert the response
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.Alias.ShouldBe("ACME");
        response.Data.Name.ShouldBe("Acme Industries");

        // Assert the database
        var tenant = _serverFixture.Context.Set<Tenant>().FirstOrDefault(x => x.Alias == response.Data.Alias);

        tenant.ShouldNotBeNull();
        tenant.Alias.ShouldBe("ACME");
        tenant.Name.ShouldBe("Acme Industries");

        var metadata = JsonSerializer.Deserialize<JsonElement>(tenant.Metadata.ToString());
        metadata.GetProperty("city").GetString().ShouldBe("Auhrus");
    }

    /// <summary>
    /// The global admin should not be able to update an existing tenant with the name used by another
    /// </summary>
    [FriendlyNamedFact("IT-084"), Priority(70)]
    public async Task Global_Admin_Cannot_Update_Tenant_With_Name_Used_By_Another()
    {
        // Prepare
        var preRequest = new CreateTenant.Request
        {
            Alias = "WAYNE INC",
            Name = "Temporary Name",
            AdminInfo = new CreateTenant.AdminUser
            {
                DisplayName = "Wayne Administrator",
                Email = "admin@wayne.com",
                Password = "12345",
                Username = "wayne.admin"
            }
        };
        var preResponse = await _serverFixture.PostAsync<TenantDetails>("api/authorization/tenants", preRequest, authenticated: true);
        preResponse.ShouldBeSuccess();

        var existingTenant = _serverFixture.Context.Set<Tenant>().SingleOrDefault(x => x.Alias == "WAYNE INC");
        existingTenant.ShouldNotBeNull();

        var request = new UpdateTenant.Request
        {
            Alias = "wayne inc",
            Name = "ACME INDUSTRIES",
        };

        // Act
        var response = await _serverFixture.PutAsync<TenantDetails>("api/authorization/tenants", request, authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Name already being used");
    }

    /// <summary>
    /// The global admin should not be able to be able to create a tenant with a duplicated name
    /// </summary>
    [FriendlyNamedFact("IT-083"), Priority(80)]
    public async Task Global_Admin_Cannot_Create_Tenant_With_Duplicated_Name()
    {
        // Prepare
        var request = new CreateTenant.Request
        {
            Alias = "AcmE2",
            Name = "ACME INDUSTRIES",
            AdminInfo = new CreateTenant.AdminUser
            {
                DisplayName = "Acme Administrator",
                Email = "acme@acme.com",
                Password = "12345",
                Username = "acme.admin"
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<TenantDetails>("api/authorization/tenants", request, authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Name already being used");
    }

    /// <summary>
    /// The global admin should be able to update the existing ACME tenant
    /// </summary>
    [FriendlyNamedFact("IT-023"), Priority(90)]
    public async Task Global_Admin_Cannot_Update_Tenant_That_Does_Not_Exist()
    {
        // Prepare
        var request = new UpdateTenant.Request
        {
            Alias = "STARK",
            Name = "Stark Industries",
            Metadata = "{ \"city\": \"Odese\" }"
        };

        // Act
        var response = await _serverFixture.PutAsync<TenantDetails>("api/authorization/tenants", request, authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Tenant not found");
    }

    /// <summary>
    /// The global admin should have validation error if name is empty or null
    /// </summary>
    [FriendlyNamedTheory("IT-024"), Priority(100)]
    [InlineData(null)]
    [InlineData("")]
    public async Task Global_Admin_Cannot_Update_Tenant_With_Empty_Or_Null_Name(string name)
    {
        // Prepare
        var request = new UpdateTenant.Request
        {
            Alias = "acme",
            Name = name,
            Metadata = "{ \"city\": \"Odese\" }"
        };

        // Act
        var response = await _serverFixture.PutAsync<TenantDetails>("api/authorization/tenants", request, authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "O campo 'Name' não pode ser vazio");
    }

    /// <summary>
    /// The global admin should have validation error if alias is empty or null
    /// </summary>
    [FriendlyNamedTheory("IT-025"), Priority(110)]
    [InlineData(null)]
    [InlineData("")]
    public async Task Global_Admin_Cannot_Update_Tenant_With_Empty_Or_Null_Alias(string alias)
    {
        // Prepare
        var request = new UpdateTenant.Request
        {
            Alias = alias,
            Name = "Acme Inc.",
            Metadata = "{ \"city\": \"Odese\" }"
        };

        // Act
        var response = await _serverFixture.PutAsync<TenantDetails>("api/authorization/tenants", request, authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "O campo 'Alias' não pode ser vazio");
    }

    /// <summary>
    /// Tenants with the same alias are not allowed, so we should not be able to create them
    /// </summary>
    [FriendlyNamedFact("IT-026"), Priority(120)]
    public async Task Global_Admin_Cannot_Create_Tenant_With_Duplicated_Id()
    {
        // Prepare
        var request = new CreateTenant.Request
        {
            Alias = "acme",
            Name = "Acme Inc",
            Metadata = "{ \"city\": \"Auhrus\" }",
            AdminInfo = new CreateTenant.AdminUser
            {
                DisplayName = "Acme Administrator",
                Email = "acme@acme.com",
                Password = "12345",
                Username = "acme.admin"
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<TenantDetails>("api/authorization/tenants", request, authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Alias already used");
    }

    /// <summary>
    /// Global admin can see a list of all tenants, so in this list the ACME tenant must be present
    /// </summary>
    [FriendlyNamedFact("IT-027"), Priority(130)]
    public async Task Global_Admin_Can_Query_Tenants()
    {
        // Act
        var response = await _serverFixture.GetAsync<TenantDetails[]>("api/authorization/tenants", authenticated: true);

        // Assert the response
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.Length.ShouldBeGreaterThan(0);
        
        // Find the entity created within these tests and asset it
        var tenant = response.Data.SingleOrDefault(x => x.Alias == "ACME");

        tenant.Name.ShouldBe("Acme Industries");

        var metadata = JsonSerializer.Deserialize<JsonElement>(tenant.Metadata.ToString());
        metadata.GetProperty("city").GetString().ShouldBe("Auhrus");
    }

    /// <summary>
    /// Global admin can delete tenants when they do not exist
    /// </summary>
    [FriendlyNamedFact("IT-085"), Priority(160)]
    public async Task Global_Admin_Cannot_Delete_Tenant_That_Do_Not_Exist()
    {
        // Act
        var response = await _serverFixture.DeleteAsync("api/authorization/tenants/xxxx", authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Tenant not found");
    }

    /// <summary>
    /// Global admin can delete tenants without the alias
    /// </summary>
    [FriendlyNamedFact("IT-086"), Priority(170)]
    public async Task Global_Admin_Cannot_Delete_Tenant_Wihtout_The_Alias()
    {
        // Act
        var response = await _serverFixture.DeleteAsync($"api/authorization/tenants", authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.MethodNotAllowed);
    }

    /// <summary>
    /// A local admin cannot delete tenants 
    /// </summary>
    [FriendlyNamedFact("IT-087"), Priority(180)]
    public async Task Local_Admin_Cannot_Delete_Tenants()
    {
        // Prepare
        var tenant = _serverFixture.Context.Set<Tenant>().Single(x => x.Alias == "WAYNE INC");

        // Act
        var response = await _serverFixture.DeleteAsync($"api/authorization/tenants/WAYNE%20INC", await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// A local admin cannot see a list of all tenants
    /// </summary>
    [FriendlyNamedFact("IT-088"), Priority(190)]
    public async Task Local_Admin_Cannot_Query_Tenants()
    {
        // Act
        var response = await _serverFixture.GetAsync<TenantDetails[]>("api/authorization/tenants", await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// Global admin can delete tenants and all related entities
    /// </summary>
    [FriendlyNamedFact("IT-028/IT-072/IT-073"), Priority(200)]
    public async Task Global_Admin_Can_Delete_Tenants_And_Related_Entities()
    {
        // Prepare
        var createTenantRequest = new CreateTenant.Request
        {
            Alias = "Stark",
            Name = "Stark Industries",
            Metadata = "{ \"city\": \"Aalborg\" }",
            AdminInfo = new CreateTenant.AdminUser
            {
                DisplayName = "Tony Stark",
                Email = "tony.stark@stark-industries.com",
                Password = "12345",
                Username = "tony.admin"
            }
        };
        var createTenantResponse = await _serverFixture.PostAsync<TenantDetails>("api/authorization/tenants", createTenantRequest, await _serverFixture.GetAccessTokenAsync("superuser", "admin", null));
        createTenantResponse.ShouldBeSuccess();

        var roleNames = new[] { "Role1", "Role2" };
        foreach (var roleName in roleNames)
        {
            var roleCreationResponse = await _serverFixture.PostAsync<RoleDetails>("api/authorization/roles", new CreateRole.Request { Name = roleName }, await _serverFixture.GetAccessTokenAsync("tony.admin", "12345", "Stark"));
            roleCreationResponse.ShouldBeSuccess();
        }

        var context = _serverFixture.Context;
        context.Add(new User("STARK", "user1", "user1@stark-industries.com", "123", "", "username1"));
        context.Add(new User("STARK", "user2", "user2@stark-industries.com", "123", "", "username2"));
        context.Add(new User("STARK", "user3", "user3@stark-industries.com", "123", "", "username3"));
        context.SaveChanges();

        var relationResponse1 = await _serverFixture.PostAsync<RoleDetails>("api/authorization/users/set-roles", new ReplaceUserRoles.Request 
        { 
            RoleIds = new[] 
            { 
                _serverFixture.Context.Set<Role>().First(x => x.Name == "Role1" && x.TenantId == "STARK").Id,
                _serverFixture.Context.Set<Role>().First(x => x.Name == "Role2" && x.TenantId == "STARK").Id
            },
            Username = "User1"
        }, await _serverFixture.GetAccessTokenAsync("tony.admin", "12345", "Stark"));
        relationResponse1.ShouldBeSuccess();

        var relationResponse2 = await _serverFixture.PostAsync<RoleDetails>("api/authorization/users/set-roles", new ReplaceUserRoles.Request
        {
            RoleIds = new[]
            {
                _serverFixture.Context.Set<Role>().First(x => x.Name == "Role1" && x.TenantId == "STARK").Id
            },
            Username = "User2"
        }, await _serverFixture.GetAccessTokenAsync("tony.admin", "12345", "Stark"));
        relationResponse2.ShouldBeSuccess();

        var claimRequest = new AddClaimOverride.Request { AccessType = ClaimAccessType.Allow, Username = "user1", ClaimIds = new[] { _serverFixture.Context.Set<Claim>().First().Id } };
        var claimResponse = await _serverFixture.PostAsync<Claim[]>("api/authorization/users/add-claim", claimRequest, await _serverFixture.GetAccessTokenAsync("tony.admin", "12345", "Stark"));
        claimResponse.ShouldBeSuccess();

        var user1 = _serverFixture.Context.Set<User>().Include(x => x.Roles).Include(x => x.Claims).First(x => x.Username == "user1" && x.TenantId == "STARK");
        var user2 = _serverFixture.Context.Set<User>().Include(x => x.Roles).Include(x => x.Claims).First(x => x.Username == "user2" && x.TenantId == "STARK");
        var user3 = _serverFixture.Context.Set<User>().Include(x => x.Roles).Include(x => x.Claims).First(x => x.Username == "user3" && x.TenantId == "STARK");

        user1.Roles.Count().ShouldBe(2);
        user2.Roles.Count().ShouldBe(1);
        user3.Roles.Count().ShouldBe(0);

        user1.Claims.Count().ShouldBe(1);
        user2.Claims.Count().ShouldBe(0);
        user3.Claims.Count().ShouldBe(0);

        // Act
        var response = await _serverFixture.DeleteAsync("api/authorization/tenants/stark", await _serverFixture.GetAccessTokenAsync("superuser", "admin", null));

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        var tenant = _serverFixture.Context.Set<Tenant>().FirstOrDefault(x => x.Alias == "STARK");
        tenant.ShouldBeNull();

        var roles = _serverFixture.Context.Set<Role>().Where(x => x.TenantId == "STARK").ToList();
        roles.Count.ShouldBe(0);
        var users = _serverFixture.Context.Set<User>().Where(x => x.TenantId == "STARK").ToList();
        users.Count.ShouldBe(0);

    }
}

