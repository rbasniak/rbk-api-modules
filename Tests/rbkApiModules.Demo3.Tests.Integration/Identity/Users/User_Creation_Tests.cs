using rbkApiModules.Commons.Core;
using rbkApiModules.Identity.Core;
using rbkApiModules.Identity.Core.DataTransfer.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static rbkApiModules.Commons.Core.Utilities.Localization.AuthenticationMessages;

namespace rbkApiModules.Demo3.Tests.Integration.Identity;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class UserCreationTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public UserCreationTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    [FriendlyNamedFact("IT-000"), Priority(-1)]
    public async Task Seed()
    {
        var context = _serverFixture.Context;

        context.Set<Tenant>().Add(new Tenant("WAYNE INC", "Wayne Industries"));

        var admin = context.Set<User>().Add(new User("WAYNE INC", "admin", "admin@wayne-inc.com", null, String.Empty, "Admin")).Entity;
        var user = context.Set<User>().Add(new User("WAYNE INC", "user", "user@wayne-inc.com", null, String.Empty, "User")).Entity;

        admin.Confirm();
        user.Confirm();

        admin.AddClaim(context.Set<Claim>().First(x => x.Identification == AuthenticationClaims.MANAGE_USERS), ClaimAccessType.Allow);

        var claim1 = context.Set<Claim>().Add(new Claim("CLAIM1", "Claim 1")).Entity;
        var claim2 = context.Set<Claim>().Add(new Claim("CLAIM2", "Claim 2")).Entity;
        var claim3 = context.Set<Claim>().Add(new Claim("CLAIM3", "Claim 3")).Entity;

        var role1 = context.Set<Role>().Add(new Role("WAYNE INC", "Role1")).Entity;
        var role2 = context.Set<Role>().Add(new Role("WAYNE INC", "Role2")).Entity;

        role1.AddClaim(claim1);
        role1.AddClaim(claim2);
        
        role2.AddClaim(claim2);
        role2.AddClaim(claim3);

        context.SaveChanges();

        // Default user for all tests
        await _serverFixture.GetAccessTokenAsync("admin", "wayne inc");
    }

    /// <summary>
    /// User cannot be created if username is not supplied
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedTheory("IT-427"), Priority(10)]
    [InlineData(null)]
    [InlineData("")]
    public async Task User_cannot_be_created_when_username_is_null_or_empty(string username)
    {
        var role1 = _serverFixture.Context.Set<Role>().First(x => x.Name == "Role1");

        // Prepare
        var request = new CreateUser.Request
        {
            Username = username,
            DisplayName = "John Doe",
            Email = "john.doe@fake-company.com",
            RoleIds = new[] { role1.Id },
            Metadata = new Dictionary<string, string>
            {
                { "age", "18" },
                { "sector", "Research" }
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/user/create", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "O campo 'Usuário' não pode ser vazio(a)");
    }

    /// <summary>
    /// User cannot be created if username is already taken
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-428"), Priority(20)]
    public async Task User_cannot_be_created_when_username_is_already_taken()
    {
        var role1 = _serverFixture.Context.Set<Role>().First(x => x.Name == "Role1");

        // Prepare
        var request = new CreateUser.Request
        {
            Username = "user",
            DisplayName = "John Doe",
            Email = "john.doe@fake-company.com",
            RoleIds = new[] { role1.Id },
            Metadata = new Dictionary<string, string>
            {
                { "age", "18" },
                { "sector", "Research" }
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/user/create", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Usuário já existe"); 
    }

    /// <summary>
    /// User cannot be created if email is not supplied
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedTheory("IT-429"), Priority(30)]
    [InlineData(null)]
    [InlineData("")]
    public async Task User_cannot_be_created_when_email_is_null_or_empty(string email)
    {
        var role1 = _serverFixture.Context.Set<Role>().First(x => x.Name == "Role1");

        // Prepare
        var request = new CreateUser.Request
        {
            Username = "new-user",
            DisplayName = "John Doe",
            Email = email,
            RoleIds = new[] { role1.Id },
            Metadata = new Dictionary<string, string>
            {
                { "age", "18" },
                { "sector", "Research" }
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/user/create", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "O campo 'E-mail' não pode ser vazio(a)");
    }

    /// <summary>
    /// User cannot be created if email is invalid
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedTheory("IT-430"), Priority(40)]
    [InlineData("aaaaaa")]
    [InlineData("aaaaa@bbbbbb")]
    [InlineData("aaaaaaa@bbbbbbb.")]
    [InlineData("@bbbbbbb.com")]
    public async Task User_cannot_be_created_when_email_is_invalid(string email)
    {
        Debug.WriteLine(email);

        var role1 = _serverFixture.Context.Set<Role>().First(x => x.Name == "Role1");

        // Prepare
        var request = new CreateUser.Request
        {
            Username = "new-user",
            DisplayName = "John Doe",
            Email = email,
            RoleIds = new[] { role1.Id },
            Metadata = new Dictionary<string, string>
            {
                { "age", "18" },
                { "sector", "Research" }
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/user/create", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "E-mail inválido");
    }

    /// <summary>
    /// User cannot be created if email is already taken
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-401"), Priority(45)]
    public async Task User_cannot_be_created_when_email_is_already_taken()
    {
        var role1 = _serverFixture.Context.Set<Role>().First(x => x.Name == "Role1");

        // Prepare
        var request = new CreateUser.Request
        {
            Username = "new-user",
            DisplayName = "John Doe",
            Email = "user@wayne-inc.com",
            RoleIds = new[] { role1.Id },
            Metadata = new Dictionary<string, string>
            {
                { "age", "18" },
                { "sector", "Research" }
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/user/create", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "E-mail já utilizado");
    }

    /// <summary>
    /// User cannot be created if display name is not supplied
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedTheory("IT-431"), Priority(50)]
    [InlineData(null)]
    [InlineData("")]
    public async Task User_cannot_be_created_when_display_name_is_null_or_empty(string displayName)
    {
        var role1 = _serverFixture.Context.Set<Role>().First(x => x.Name == "Role1");

        // Prepare
        var request = new CreateUser.Request
        {
            Username = "new-user",
            DisplayName = displayName,
            Email = "new-user@wayne-inc.com",
            RoleIds = new[] { role1.Id },
            Metadata = new Dictionary<string, string>
            {
                { "age", "18" },
                { "sector", "Research" }
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/user/create", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "O campo 'Nome' não pode ser vazio(a)");
    }

    /// <summary>
    /// User cannot be created if custom metadata validations are not met
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-432"), Priority(60)]
    public async Task User_cannot_be_created_when_metadata_custom_validators_do_not_pass()
    {
        var role1 = _serverFixture.Context.Set<Role>().First(x => x.Name == "Role1");

        // Prepare
        var request = new CreateUser.Request
        {
            Username = "new-user",
            DisplayName = "New User",
            Email = "new-user@wayne-inc.com",
            RoleIds = new[] { role1.Id },
            Metadata = new Dictionary<string, string>
            {
                { "sector", "" },
                { "age", "XIV" }
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/user/create", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Age is not valid", "Sector is required");
    }

    /// <summary>
    /// User cannot be created if no role is supplied (null list)
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-436"), Priority(90)]
    public async Task User_cannot_be_created_when_role_list_is_null()
    {
        // Prepare
        var request = new CreateUser.Request
        {
            Username = "new-user",
            DisplayName = "John Doe",
            Email = "new-user@wayne-inc.com",
            RoleIds = null,
            Metadata = new Dictionary<string, string>
            {
                { "age", "18" },
                { "sector", "Research" }
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/user/create", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "A lista de perfis precisa ter ao menos um item");
    }

    /// <summary>
    /// User cannot be created if no role is supplied (empty list)
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-439"), Priority(100)]
    public async Task User_cannot_be_created_when_role_list_is_empty()
    {
        // Prepare
        var request = new CreateUser.Request
        {
            Username = "new-user",
            DisplayName = "John Doe",
            Email = "new-user@wayne-inc.com",
            RoleIds = new Guid[0],
            Metadata = new Dictionary<string, string>
            {
                { "age", "18" },
                { "sector", "Research" }
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/user/create", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "A lista de perfis precisa ter ao menos um item");
    }

    /// <summary>
    /// User cannot be created if role list has an invalid role
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-440"), Priority(110)]
    public async Task User_cannot_be_created_when_role_list_has_an_invlid_role()
    {
        var role1 = _serverFixture.Context.Set<Role>().First(x => x.Name == "Role1");

        // Prepare
        var request = new CreateUser.Request
        {
            Username = "new-user",
            DisplayName = "John Doe",
            Email = "new-user@wayne-inc.com",
            RoleIds = new[] { role1.Id, Guid.Empty },
            Metadata = new Dictionary<string, string>
            {
                { "age", "18" },
                { "sector", "Research" }
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/user/create", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Perfil não encontrado");
    }

    /// <summary>
    /// User cannot be created if requesting user doesn't have the proper access rights
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-436"), Priority(120)]
    public async Task User_cannot_be_created_when_requester_does_not_have_proper_access_rights()
    {
        var role1 = _serverFixture.Context.Set<Role>().First(x => x.Name == "Role1");

        // Prepare
        var request = new CreateUser.Request
        {
            Username = "new-user",
            DisplayName = "John Doe",
            Email = "new-user@wayne-inc.com",
            RoleIds = new[] { role1.Id, Guid.Empty },
            Metadata = new Dictionary<string, string>
            {
                { "age", "18" },
                { "sector", "Research" }
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/user/create", request, await _serverFixture.GetAccessTokenAsync("user", "wayne inc"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.Forbidden);
    } 

    /// <summary>
    /// User can be created
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-489"), Priority(140)]
    public async Task User_can_be_created()
    {
        var role1 = _serverFixture.Context.Set<Role>().First(x => x.Name == "Role1");
        var role2 = _serverFixture.Context.Set<Role>().First(x => x.Name == "Role2");

        // Prepare
        var request = new CreateUser.Request
        {
            Username = "new-user",
            DisplayName = "John Doe",
            Email = "new-user@wayne-inc.com",
            RoleIds = new[] { role1.Id, role2.Id },
            Metadata = new Dictionary<string, string>
            {
                { "age", "25" },
                { "sector", "Research" }
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/user/create", request, true);

        // Assert the response
        response.ShouldBeSuccess();
        response.Data.ShouldNotBeNull();
        response.Data.ShouldBeOfType<UserDetails>();
        response.Data.DisplayName.ShouldBe("John Doe");
        response.Data.Username.ShouldBe("new-user");
        response.Data.Email.ShouldBe("new-user@wayne-inc.com");
        response.Data.Avatar.ShouldNotBeNull("");
        response.Data.IsConfirmed.ShouldBeTrue();
        response.Data.Roles.Length.ShouldBe(2);
        response.Data.Claims.Length.ShouldBe(3);
        response.Data.OverridedClaims.Length.ShouldBe(0);

        // Assert the database
        var user = _serverFixture.Context.Set<User>()
            .Include(x => x.Claims)
            .Include(x => x.Roles)
            .First(x => x.Username == "new-user");

        user.ActivationCode.ShouldBeNull();
        user.Avatar.ShouldNotBeNull();
        user.Claims.Count().ShouldBe(0);
        user.DisplayName.ShouldBe("John Doe");
        user.Email.ShouldBe("new-user@wayne-inc.com");
        user.HasTenant.ShouldBeTrue();
        user.IsActive.ShouldBeTrue();
        user.IsConfirmed.ShouldBeTrue();
        user.Metadata.ShouldNotBeNull();
        user.Metadata.Count.ShouldBe(5);
        user.Metadata["sector"].ShouldBe("Research");
        user.Metadata["age"].ShouldBe("25");
        user.Metadata["building"].ShouldBe("B11");
        user.Metadata["office"].ShouldBe("1219");
        user.Metadata["desk"].ShouldBe("3");
        user.PasswordRedefineCode.ShouldBeNull();
        user.Password.ShouldBeNull();
        user.TenantId.ShouldBe("WAYNE INC");
        user.Username.ShouldBe("new-user");
        user.Roles.Count().ShouldBe(2);
        (DateTime.UtcNow - user.CreationDate).TotalSeconds.ShouldBeLessThan(60);
    }
}
