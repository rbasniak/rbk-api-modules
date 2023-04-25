using rbkApiModules.Commons.Core;
using rbkApiModules.Identity.Core.DataTransfer.Users;

namespace rbkApiModules.Demo1.Tests.Integration.Identity;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class UserRegistrationTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public UserRegistrationTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    [FriendlyNamedFact("IT-000"), Priority(-1)]
    public void Seed()
    {
        var context = _serverFixture.Context;

        context.Set<Tenant>().Add(new Tenant("WAYNE INC", "Wayne Industries"));

        context.Set<User>().Add(new User("WAYNE INC", "user", "user@wayne-inc.com", "admin123", String.Empty, "Admin", AuthenticationMode.Credentials));

        context.SaveChanges();
    }

    /// <summary>
    /// User cannot register if username is not provided
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedTheory("IT-450"), Priority(10)]
    [InlineData(null)]
    [InlineData("")]
    public async Task User_cannot_register_when_username_is_null_or_empty(string username)
    {
        // Prepare
        var request = new Register.Request
        {
            Username = username,
            DisplayName = "John Doe",
            Email = "john.doe@fake-company.com",
            Password = "pw123",
            PasswordConfirmation = "pw123",
            Tenant = "wayne inc",
            Metadata = new Dictionary<string, string>
            {
                { "age", "18" },
                { "sector", "Research" }
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/user/register", request, false);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The field 'User' cannot be empty");
    }

    /// <summary>
    /// User cannot register if username is already taken
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-451"), Priority(20)]
    public async Task User_cannot_register_when_username_is_already_taken()
    {
        // Prepare
        var request = new Register.Request
        {
            Username = "user",
            DisplayName = "John Doe",
            Email = "john.doe@fake-company.com",
            Password = "pw123",
            PasswordConfirmation = "pw123",
            Tenant = "wayne inc",
            Metadata = new Dictionary<string, string>
            {
                { "age", "18" },
                { "sector", "Research" }
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/user/register", request, false);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "User already exists");
    }

    /// <summary>
    /// User cannot register if tenant does not exist
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-452"), Priority(30)]
    public async Task User_cannot_register_when_tenant_does_not_exist()
    {
        // Prepare
        var request = new Register.Request
        {
            Username = "new user",
            DisplayName = "John Doe",
            Email = "john.doe@fake-company.com",
            Password = "pw123",
            PasswordConfirmation = "pw123",
            Tenant = "waine ync",
            Metadata = new Dictionary<string, string>
            {
                { "age", "18" },
                { "sector", "Research" }
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/user/register", request, false);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Tenant not found");
    }

    /// <summary>
    /// User cannot register if tenant is not provided
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedTheory("IT-453"), Priority(40)]
    [InlineData(null)]
    [InlineData("")]
    public async Task User_cannot_register_when_tenant_is_null_or_empty(string tenant)
    {
        // Prepare
        var request = new Register.Request
        {
            Username = "new user",
            DisplayName = "John Doe",
            Email = "john.doe@fake-company.com",
            Password = "pw123",
            PasswordConfirmation = "pw123",
            Tenant = tenant,
            Metadata = new Dictionary<string, string>
            {
                { "age", "18" },
                { "sector", "Research" }
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/user/register", request, false);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The field 'Tenant' cannot be empty");
    }

    /// <summary>
    /// User cannot register if email is not provided
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedTheory("IT-454"), Priority(50)]
    [InlineData(null)]
    [InlineData("")]
    public async Task User_cannot_register_when_email_is_null_or_empty(string email)
    {
        // Prepare
        var request = new Register.Request
        {
            Username = "new user",
            DisplayName = "John Doe",
            Email = email,
            Password = "pw123",
            PasswordConfirmation = "pw123",
            Tenant = "wayne inc",
            Metadata = new Dictionary<string, string>
            {
                { "age", "18" },
                { "sector", "Research" }
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/user/register", request, false);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The field 'E-mail' cannot be empty");
    }

    /// <summary>
    /// User cannot register if email is not valid
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedTheory("IT-455"), Priority(60)]
    [InlineData("aaaaaa")]
    [InlineData("aaaaa@bbbbbb")]
    [InlineData("aaaaaaa@bbbbbbb.")]
    [InlineData("@bbbbbbb.com")]
    public async Task User_cannot_register_when_email_is_invalid(string email)
    {
        // Prepare
        var request = new Register.Request
        {
            Username = "new user",
            DisplayName = "John Doe",
            Email = email,
            Password = "pw123",
            PasswordConfirmation = "pw123",
            Tenant = "wayne inc",
            Metadata = new Dictionary<string, string>
            {
                { "age", "18" },
                { "sector", "Research" }
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/user/register", request, false);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "E-mail is invalid");
    }

    /// <summary>
    /// User cannot register if email is already taken
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-462"), Priority(65)]
    public async Task User_cannot_register_when_email_is_already_taken()
    {
        // Prepare
        var request = new Register.Request
        {
            Username = "new user",
            DisplayName = "John Doe",
            Email = "user@wayne-inc.com",
            Password = "pw123",
            PasswordConfirmation = "pw123",
            Tenant = "wayne inc",
            Metadata = new Dictionary<string, string>
            {
                { "age", "18" },
                { "sector", "Research" }
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/user/register", request, false);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "E-mail already used");
    }

    /// <summary>
    /// User cannot register if display name is not provided
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedTheory("IT-456"), Priority(70)]
    [InlineData(null)]
    [InlineData("")]
    public async Task User_cannot_register_when_display_name_is_null_or_empty(string displayName)
    {
        // Prepare
        var request = new Register.Request
        {
            Username = "new user",
            DisplayName = displayName,
            Email = "new-user@wayne-inc.com",
            Password = "pw123",
            PasswordConfirmation = "pw123",
            Tenant = "wayne inc",
            Metadata = new Dictionary<string, string>
            {
                { "age", "18" },
                { "sector", "Research" }
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/user/register", request, false);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The field 'Display name' cannot be empty");
    }

    /// <summary>
    /// User cannot register if metadata custom validators are not met
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-457"), Priority(80)]
    public async Task User_cannot_register_when_metadata_custom_validators_do_not_pass()
    {
        // Prepare
        var request = new Register.Request
        {
            Username = "new user",
            DisplayName = "John Doe",
            Email = "new-user@wayne-inc.com",
            Password = "pw123",
            PasswordConfirmation = "pw123",
            Tenant = "wayne inc",
            Metadata = new Dictionary<string, string>
            {
                { "age", "XX" },
                { "sector", "" }
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/user/register", request, false);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Age is not valid", "Sector is required");
    }

    /// <summary>
    /// User cannot register if password is not provided
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedTheory("IT-458"), Priority(90)]
    [InlineData(null)]
    [InlineData("")]
    public async Task User_cannot_register_when_password_is_null_or_empty(string password)
    {
        // Prepare
        var request = new Register.Request
        {
            Username = "new user",
            DisplayName = "John Doe",
            Email = "new-user@wayne-inc.com",
            Password = password,
            PasswordConfirmation = "pw123",
            Tenant = "wayne inc",
            Metadata = new Dictionary<string, string>
            {
                { "age", "18" },
                { "sector", "Research" }
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/user/register", request, false);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The field 'Password' cannot be empty");
    }

    /// <summary>
    /// User cannot register if passwords don't match
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-459"), Priority(100)]
    public async Task User_cannot_register_when_passwords_do_not_match()
    {
        // Prepare
        var request = new Register.Request
        {
            Username = "new user",
            DisplayName = "John Doe",
            Email = "new-user@wayne-inc.com",
            Password = "pw456",
            PasswordConfirmation = "pw123",
            Tenant = "wayne inc",
            Metadata = new Dictionary<string, string>
            {
                { "age", "18" },
                { "sector", "Research" }
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/user/register", request, false);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Password and confirmation must match");
    }

    /// <summary>
    /// User cannot register if passwords custom policies are not met
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-460"), Priority(110)]
    public async Task User_cannot_register_when_password_policies_are_not_met()
    {
        // Prepare
        var request = new Register.Request
        {
            Username = "new user",
            DisplayName = "John Doe",
            Email = "new-user@wayne-inc.com",
            Password = "pw",
            PasswordConfirmation = "pw",
            Tenant = "wayne inc",
            Metadata = new Dictionary<string, string>
            {
                { "age", "18" },
                { "sector", "Research" }
            }
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/user/register", request, false);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The password must have at least 3 characteres");
    }

    /// <summary>
    /// User can register
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-461"), Priority(120)]
    public async Task User_can_register()
    {
        // Prepare
        var request = new Register.Request
        {
            Username = "new-user",
            DisplayName = "John Doe",
            Email = "new-user@wayne-inc.com",
            Password = "pw123",
            PasswordConfirmation = "pw123",
            Tenant = "wayne inc",
            Metadata = new Dictionary<string, string>
            {
                { "age", "18" },
                { "sector", "Research" }
            }
        };

        // Act
        var response = await _serverFixture.PostAsync("api/authentication/user/register", request, null);

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        var user = _serverFixture.Context.Set<User>()
            .Include(x => x.Claims)
            .Include(x => x.Roles)
            .First(x => x.Username == "new-user");

        PasswordHasher.VerifyPassword("pw123", user.Password).ShouldBeTrue();
        user.ActivationCode.ShouldNotBeNull();
        user.Avatar.ShouldNotBeNull();
        user.Claims.Count().ShouldBe(0);
        user.DisplayName.ShouldBe("John Doe");
        user.Email.ShouldBe("new-user@wayne-inc.com");
        user.HasTenant.ShouldBeTrue();
        user.IsActive.ShouldBeTrue();
        user.IsConfirmed.ShouldBeFalse();
        user.Metadata.ShouldNotBeNull();
        user.Metadata["sector"].ShouldBe("Research");
        user.Metadata["age"].ShouldBe("18");
        user.PasswordRedefineCode.ShouldBeNull();
        user.TenantId.ShouldBe("WAYNE INC");
        user.Username.ShouldBe("new-user");
        user.AuthenticationMode.ShouldBe(AuthenticationMode.Credentials);
        (DateTime.UtcNow - user.CreationDate).TotalSeconds.ShouldBeLessThan(5);

        _serverFixture.ShouldHaveSentEmail(options => options
            .ToAddress(user.Email)
            .WithTileContaining("Registration confirmation")
        );
    }
}
