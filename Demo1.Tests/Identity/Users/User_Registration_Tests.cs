using Demo1.Tests;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Core;
using rbkApiModules.Identity.Core;
using rbkApiModules.Identity.Core.DataTransfer.Users;

namespace rbkApiModules.Identity.Tests.Users;

[NotInParallel(nameof(User_Registration_Tests))]
public class User_Registration_Tests
{
    [ClassDataSource<Demo1TestingServer>(Shared = SharedType.PerClass)]
    public required Demo1TestingServer TestingServer { get; set; } = default!;

    [Test, NotInParallel(Order = 1)]
    public void Seed()
    {
        TestingServer.ClearMailsFolder();

        var context = TestingServer.CreateContext();

        context.Set<Tenant>().Add(new Tenant("WAYNE INC", "Wayne Industries"));

        context.Set<User>().Add(new User("WAYNE INC", "user", "user@wayne-inc.com", "admin123", String.Empty, "Admin", AuthenticationMode.Credentials));

        context.SaveChanges();
    }

    /// <summary>
    /// User cannot register if username is not provided
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 2)]
    [Arguments(null)]
    [Arguments("")]
    public async Task User_cannot_register_when_username_is_null_or_empty(string? username)
    {
        // Prepare
        var request = new Register.Request
        {
            Username = username!,
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
        var response = await TestingServer.PostAsync<UserDetails>("api/authentication/register", request);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "'Username' must not be empty.");
    }

    /// <summary>
    /// User cannot register if username is already taken
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 3)]
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
        var response = await TestingServer.PostAsync<UserDetails>("api/authentication/register", request);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "User already exists");
    }

    /// <summary>
    /// User cannot register if tenant does not exist
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 4)]
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
        var response = await TestingServer.PostAsync<UserDetails>("api/authentication/register", request);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Tenant not found");
    }

    /// <summary>
    /// User cannot register if tenant is not provided
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 5)]
    [Arguments(null)]
    [Arguments("")]
    public async Task User_cannot_register_when_tenant_is_null_or_empty(string? tenant)
    {
        // Prepare
        var request = new Register.Request
        {
            Username = "new user",
            DisplayName = "John Doe",
            Email = "john.doe@fake-company.com",
            Password = "pw123",
            PasswordConfirmation = "pw123",
            Tenant = tenant!,
            Metadata = new Dictionary<string, string>
            {
                { "age", "18" },
                { "sector", "Research" }
            }
        };

        // Act
        var response = await TestingServer.PostAsync<UserDetails>("api/authentication/register", request);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "'Tenant' must not be empty.");
    }

    /// <summary>
    /// User cannot register if email is not provided
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 6)]
    [Arguments(null)]
    [Arguments("")]
    public async Task User_cannot_register_when_email_is_null_or_empty(string? email)
    {
        // Prepare
        var request = new Register.Request
        {
            Username = "new user",
            DisplayName = "John Doe",
            Email = email!,
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
        var response = await TestingServer.PostAsync<UserDetails>("api/authentication/register", request);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "'Email' must not be empty.");
    }

    /// <summary>
    /// User cannot register if email is not valid
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 7)]
    [Arguments("aaaaaa")]
    //[Arguments("aaaaa@bbbbbb")] // Fluentvalidation is accepting this as a valid e-mail
    //[Arguments("aaaaaaa@bbbbbbb.")] // Fluentvalidation is accepting this as a valid e-mail
    [Arguments("@bbbbbbb.com")]
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
        var response = await TestingServer.PostAsync("api/authentication/register", request);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "'Email' is not a valid email address.");
    }

    /// <summary>
    /// User cannot register if email is already taken
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 8)]
    public async Task User_cannot_register_when_email_is_already_taken()
    {
        // Prepare
        var request = new Register.Request
        {
            Username = "new user 2",
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
        var response = await TestingServer.PostAsync<UserDetails>("api/authentication/register", request);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "E-mail already used");
    }

    /// <summary>
    /// User cannot register if display name is not provided
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 9)]
    [Arguments(null)]
    [Arguments("")]
    public async Task User_cannot_register_when_display_name_is_null_or_empty(string? displayName)
    {
        // Prepare
        var request = new Register.Request
        {
            Username = "new user",
            DisplayName = displayName!,
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
        var response = await TestingServer.PostAsync("api/authentication/register", request);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "'Display Name' must not be empty.");
    }

    /// <summary>
    /// User cannot register if metadata custom validators are not met
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 10)]
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
        var response = await TestingServer.PostAsync("api/authentication/register", request);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Age is not valid", "Sector is required");
    }

    /// <summary>
    /// User cannot register if password is not provided
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 11)]
    [Arguments(null)]
    [Arguments("")]
    public async Task User_cannot_register_when_password_is_null_or_empty(string? password)
    {
        // Prepare
        var request = new Register.Request
        {
            Username = "new user",
            DisplayName = "John Doe",
            Email = "new-user@wayne-inc.com",
            Password = password!,
            PasswordConfirmation = "pw123",
            Tenant = "wayne inc",
            Metadata = new Dictionary<string, string>
            {
                { "age", "18" },
                { "sector", "Research" }
            }
        };

        // Act
        var response = await TestingServer.PostAsync("api/authentication/register", request);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "'Password' must not be empty.");
    }

    /// <summary>
    /// User cannot register if passwords don't match
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 12)]
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
        var response = await TestingServer.PostAsync<UserDetails>("api/authentication/register", request);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Password and confirmation must match");
    }

    /// <summary>
    /// User cannot register if passwords custom policies are not met
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 13)]
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
        var response = await TestingServer.PostAsync<UserDetails>("api/authentication/register", request);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The password must have at least 3 characteres");
    }

    /// <summary>
    /// User can register
    /// DEPENDENCIES: none
    /// </summary>
    [Test, NotInParallel(Order = 14)]
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
        var response = await TestingServer.PostAsync("api/authentication/register", request);

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        var user = TestingServer.CreateContext().Set<User>()
            .Include(x => x.Claims)
            .Include(x => x.Roles)
            .First(x => x.Username == "new-user");

        user.ShouldNotBeNull();
        user.Password.ShouldNotBeNull();

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

        user.Email.ShouldNotBeNull();

        TestingServer.ShouldHaveSentEmail(options => options
            .ToAddress(user.Email)
            .WithTileContaining("Registration confirmation")
        );
    }

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }
}
