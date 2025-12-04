using System.Text.Json;
using Demo1.Tests;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Identity.Core;
using rbkApiModules.Identity.Core.DataTransfer;
using rbkApiModules.Identity.Core.DataTransfer.Users;

namespace rbkApiModules.Identity.Tests.Tenants;

[NotInParallel(nameof(Tenant_Management_Tests))]
public class Tenant_Management_Tests
{
    [ClassDataSource<Demo1TestingServer>(Shared = SharedType.PerClass)]
    public required Demo1TestingServer TestingServer { get; set; } = default!;

    /// <summary>
    /// Login call, just to store the access tokens for future tests to use
    /// </summary>
    [Test, NotInParallel(Order = 1)]
    public async Task Login()
    {
        await TestingServer.CacheCredentialsAsync("superuser", "admin", null);
        await TestingServer.CacheCredentialsAsync("admin1", "123", "buzios");
    }

    /// <summary>
    /// The global admin should be able to create a new tenant called ACME
    /// </summary>
    [Test, NotInParallel(Order = 2)]
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
        var response = await TestingServer.PostAsync<TenantDetails>("api/authorization/tenants", request, "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The password must have at least 3 characteres");
    }

    /// <summary>
    /// The global admin should be able to create a new tenant called ACME
    /// </summary>
    [Test, NotInParallel(Order = 3)]
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
        var response = await TestingServer.PostAsync<TenantDetails>("api/authorization/tenants", request, "superuser");

        // Assert the response
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.Alias.ShouldBe("ACME");
        response.Data.Name.ShouldBe("Acme Inc.");

        // Assert the database
        var tenant = TestingServer.CreateContext().Set<Tenant>().FirstOrDefault(x => x.Alias == response.Data.Alias);

        tenant.ShouldNotBeNull();
        tenant.Alias.ShouldBe("ACME");
        tenant.Name.ShouldBe("Acme Inc.");

        tenant.Metadata.ShouldNotBeNull();

        var metadata = JsonSerializer.Deserialize<JsonElement>(tenant.Metadata.ToString());
        metadata.GetProperty("city").GetString().ShouldBe("Aalborg");

        var user = await TestingServer.CreateContext().Set<User>().Include(x => x.Claims).ThenInclude(x => x.Claim).FirstOrDefaultAsync(x => x.Username == "acme.admin" && x.TenantId == "ACME");
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
    [Arguments(null)]
    [Arguments("")]
    [Test, NotInParallel(Order = 4)]
    public async Task Global_Admin_Cannot_Create_Tenant_Without_Alias(string? alias)
    {
        // Prepare 
        var request = new CreateTenant.Request
        {
            Alias = alias!,
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
        var response = await TestingServer.PostAsync<TenantDetails>("api/authorization/tenants", request, "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "'Alias' must not be empty.");
    }

    /// <summary>
    /// The global admin should not be able to create a new tenant without a name
    /// </summary>
    [Arguments(null)]
    [Arguments("")]
    [Test, NotInParallel(Order = 5)]
    public async Task Global_Admin_Cannot_Create_Tenant_Without_Name(string? name)
    {
        // Prepare 
        var request = new CreateTenant.Request
        {
            Alias = "ALIAS",
            Name = name!,
            AdminInfo = new CreateTenant.AdminUser
            {
                DisplayName = "Acme Administrator",
                Email = "acme@acme.com",
                Password = "12345",
                Username = "acme.admin"
            }
        };

        // Act
        var response = await TestingServer.PostAsync<TenantDetails>("api/authorization/tenants", request, "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "'Name' must not be empty.");
    }

    /// <summary>
    /// A local admin should not be able to create a new tenant 
    /// </summary>
    [Test, NotInParallel(Order = 6)]
    public async Task Local_Admin_Can_Create_Tenant()
    {
        // Prepare 
        var request = new CreateTenant.Request
        {
            Alias = "NOT ALLOWED",
            Name = "This wil not be created",
        };

        // Act
        var response = await TestingServer.PostAsync<TenantDetails>("api/authorization/tenants", request, "admin1");    

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// A local admin should not be able to update a tenant
    /// </summary>
    [Test, NotInParallel(Order = 7)]
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
        var response = await TestingServer.PutAsync<TenantDetails>("api/authorization/tenants", request, "admin1");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// The global admin should be able to update the existing ACME tenant
    /// </summary>
    [Test, NotInParallel(Order = 8)]
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
        var response = await TestingServer.PutAsync<TenantDetails>("api/authorization/tenants", request, "superuser");

        // Assert the response
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.Alias.ShouldBe("ACME");
        response.Data.Name.ShouldBe("Acme Industries");

        // Assert the database
        var tenant = TestingServer.CreateContext().Set<Tenant>().FirstOrDefault(x => x.Alias == response.Data.Alias);

        tenant.ShouldNotBeNull();
        tenant.Alias.ShouldBe("ACME");
        tenant.Name.ShouldBe("Acme Industries");

        tenant.Metadata.ShouldNotBeNull();

        var metadata = JsonSerializer.Deserialize<JsonElement>(tenant.Metadata.ToString());
        metadata.GetProperty("city").GetString().ShouldBe("Auhrus");
    }

    /// <summary>
    /// The global admin should not be able to update an existing tenant with the name used by another
    /// </summary>
    [Test, NotInParallel(Order = 9)]
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
        var preResponse = await TestingServer.PostAsync<TenantDetails>("api/authorization/tenants", preRequest, "superuser");
        preResponse.ShouldBeSuccess();

        var existingTenant = TestingServer.CreateContext().Set<Tenant>().SingleOrDefault(x => x.Alias == "WAYNE INC");
        existingTenant.ShouldNotBeNull();

        var request = new UpdateTenant.Request
        {
            Alias = "wayne inc",
            Name = "ACME INDUSTRIES",
        };

        // Act
        var response = await TestingServer.PutAsync<TenantDetails>("api/authorization/tenants", request, "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Name already used");
    }

    /// <summary>
    /// The global admin should not be able to be able to create a tenant with a duplicated name
    /// </summary>
    [Test, NotInParallel(Order = 10)]
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
        var response = await TestingServer.PostAsync<TenantDetails>("api/authorization/tenants", request, "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Name already used");
    }

    /// <summary>
    /// The global admin should be able to update the existing ACME tenant
    /// </summary>
    [Test, NotInParallel(Order = 11)]
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
        var response = await TestingServer.PutAsync<TenantDetails>("api/authorization/tenants", request, "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Tenant not found");
    }

    /// <summary>
    /// The global admin should have validation error if name is empty or null
    /// </summary>
    [Arguments(null)]
    [Arguments("")]
    [Test, NotInParallel(Order = 12)]
    public async Task Global_Admin_Cannot_Update_Tenant_With_Empty_Or_Null_Name(string? name)
    {
        // Prepare
        var request = new UpdateTenant.Request
        {
            Alias = "acme",
            Name = name!,
            Metadata = "{ \"city\": \"Odese\" }"
        };

        // Act
        var response = await TestingServer.PutAsync<TenantDetails>("api/authorization/tenants", request, "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "'Name' must not be empty.");
    }

    /// <summary>
    /// The global admin should have validation error if alias is empty or null
    /// </summary>
    [Arguments(null)]
    [Arguments("")]
    [Test, NotInParallel(Order = 13)]
    public async Task Global_Admin_Cannot_Update_Tenant_With_Empty_Or_Null_Alias(string? alias)
    {
        // Prepare
        var request = new UpdateTenant.Request
        {
            Alias = alias!,
            Name = "Acme Inc.",
            Metadata = "{ \"city\": \"Odese\" }"
        };

        // Act
        var response = await TestingServer.PutAsync<TenantDetails>("api/authorization/tenants", request, "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "'Alias' must not be empty.");
    }

    /// <summary>
    /// Tenants with the same alias are not allowed, so we should not be able to create them
    /// </summary>
    [Test, NotInParallel(Order = 14)]
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
        var response = await TestingServer.PostAsync<TenantDetails>("api/authorization/tenants", request, "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Tenant alias already used");
    }

    /// <summary>
    /// Global admin can see a list of all tenants, so in this list the ACME tenant must be present
    /// </summary>
    [Test, NotInParallel(Order = 15)]
    public async Task Global_Admin_Can_Query_Tenants()
    {
        // Act
        var response = await TestingServer.GetAsync<TenantDetails[]>("api/authorization/tenants", "superuser");

        // Assert the response
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.Length.ShouldBeGreaterThan(0);
        
        // Find the entity created within these tests and asset it
        var tenant = response.Data.SingleOrDefault(x => x.Alias == "ACME");
        
        tenant.ShouldNotBeNull();
        tenant.Name.ShouldBe("Acme Industries");

        tenant.Metadata.ShouldNotBeNull();

        var metadata = JsonSerializer.Deserialize<JsonElement>(tenant.Metadata.ToString());
        metadata.GetProperty("city").GetString().ShouldBe("Auhrus");
    }

    /// <summary>
    /// Global admin can delete tenants when they do not exist
    /// </summary>
    [Test, NotInParallel(Order = 16)]
    public async Task Global_Admin_Cannot_Delete_Tenant_That_Do_Not_Exist()
    {
        // Act
        var response = await TestingServer.DeleteAsync("api/authorization/tenants/xxxx", "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Tenant not found");
    }

    /// <summary>
    /// Global admin can delete tenants without the alias
    /// </summary>
    [Test, NotInParallel(Order = 17)]
    public async Task Global_Admin_Cannot_Delete_Tenant_Wihtout_The_Alias()
    {
        // Act
        var response = await TestingServer.DeleteAsync($"api/authorization/tenants", "superuser");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.MethodNotAllowed);
    }

    /// <summary>
    /// A local admin cannot delete tenants 
    /// </summary>
    [Test, NotInParallel(Order = 18)]
    public async Task Local_Admin_Cannot_Delete_Tenants()
    {
        // Prepare
        var tenant = TestingServer.CreateContext().Set<Tenant>().Single(x => x.Alias == "WAYNE INC");

        // Act
        var response = await TestingServer.DeleteAsync($"api/authorization/tenants/WAYNE%20INC", "admin1");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// A local admin cannot see a list of all tenants
    /// </summary>
    [Test, NotInParallel(Order = 19)]
    public async Task Local_Admin_Cannot_Query_Tenants()
    {
        // Act
        var response = await TestingServer.GetAsync<TenantDetails[]>("api/authorization/tenants", "admin1");

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// Global admin can delete tenants and all related entities
    /// </summary>
    [Test, NotInParallel(Order = 20)]
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

        var createTenantResponse = await TestingServer.PostAsync<TenantDetails>("api/authorization/tenants", createTenantRequest, "superuser");
        createTenantResponse.ShouldBeSuccess();

        await TestingServer.CacheCredentialsAsync("tony.admin", "12345", "Stark");

        var roleNames = new[] { "Role1", "Role2" };
        foreach (var roleName in roleNames)
        {
            var roleCreationResponse = await TestingServer.PostAsync<RoleDetails>("api/authorization/roles", new CreateRole.Request { Name = roleName }, "tony.admin");
            roleCreationResponse.ShouldBeSuccess();
        }

        var context = TestingServer.CreateContext();
        context.Add(new User("STARK", "user1", "user1@stark-industries.com", "123", AvatarGenerator.GenerateBase64Avatar("U 1"), "username1", AuthenticationMode.Credentials));
        context.Add(new User("STARK", "user2", "user2@stark-industries.com", "123", AvatarGenerator.GenerateBase64Avatar("U 2"), "username2", AuthenticationMode.Credentials));
        context.Add(new User("STARK", "user3", "user3@stark-industries.com", "123", AvatarGenerator.GenerateBase64Avatar("U 3"), "username3", AuthenticationMode.Credentials));
        context.SaveChanges();

        var relationResponse1 = await TestingServer.PostAsync<UserDetails>("api/authorization/users/set-roles", new ReplaceUserRoles.Request 
        { 
            RoleIds = new[] 
            { 
                TestingServer.CreateContext().Set<Role>().First(x => x.Name == "Role1" && x.TenantId == "STARK").Id,
                TestingServer.CreateContext().Set<Role>().First(x => x.Name == "Role2" && x.TenantId == "STARK").Id
            },
            Username = "User1"
        }, "tony.admin");
        relationResponse1.ShouldBeSuccess();

        var relationResponse2 = await TestingServer.PostAsync<UserDetails>("api/authorization/users/set-roles", new ReplaceUserRoles.Request
        {
            RoleIds = new[]
            {
                TestingServer.CreateContext().Set<Role>().First(x => x.Name == "Role1" && x.TenantId == "STARK").Id
            },
            Username = "User2"
        }, "tony.admin");
        relationResponse2.ShouldBeSuccess();

        var claimRequest = new AddClaimOverride.Request { AccessType = ClaimAccessType.Allow, Username = "user1", ClaimIds = new[] { TestingServer.CreateContext().Set<Claim>().First().Id } };
        var claimResponse = await TestingServer.PostAsync<UserDetails>("api/authorization/users/add-claims", claimRequest, "tony.admin");
        claimResponse.ShouldBeSuccess();

        var user1 = TestingServer.CreateContext().Set<User>().Include(x => x.Roles).Include(x => x.Claims).First(x => x.Username == "user1" && x.TenantId == "STARK");
        var user2 = TestingServer.CreateContext().Set<User>().Include(x => x.Roles).Include(x => x.Claims).First(x => x.Username == "user2" && x.TenantId == "STARK");
        var user3 = TestingServer.CreateContext().Set<User>().Include(x => x.Roles).Include(x => x.Claims).First(x => x.Username == "user3" && x.TenantId == "STARK");

        user1.Roles.Count().ShouldBe(2);
        user2.Roles.Count().ShouldBe(1);
        user3.Roles.Count().ShouldBe(0);

        user1.Claims.Count().ShouldBe(1);
        user2.Claims.Count().ShouldBe(0);
        user3.Claims.Count().ShouldBe(0);

        // Act
        var response = await TestingServer.DeleteAsync("api/authorization/tenants/stark", "superuser");

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        var tenant = TestingServer.CreateContext().Set<Tenant>().FirstOrDefault(x => x.Alias == "STARK");
        tenant.ShouldBeNull();

        var roles = TestingServer.CreateContext().Set<Role>().Where(x => x.TenantId == "STARK").ToList();
        roles.Count.ShouldBe(0);
        var users = TestingServer.CreateContext().Set<User>().Where(x => x.TenantId == "STARK").ToList();
        users.Count.ShouldBe(0);
    }

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }
}

