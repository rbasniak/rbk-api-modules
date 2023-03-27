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

        var role1 = context.Set<Role>().Add(new Role("WAYNE INC", "Role1")).Entity;
        var role2 = context.Set<Role>().Add(new Role("WAYNE INC", "Role2")).Entity;

        context.SaveChanges();

        // Default user for all tests
        await _serverFixture.GetAccessTokenAsync("admin", "wayne inc");
    }

    /// <summary>
    /// User cannot be created if username is not supplied
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedTheory("IT-227"), Priority(10)]
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
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/create-user", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The field 'User' cannot be empty");
    }

    /// <summary>
    /// User cannot be created if username is already taken
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-228"), Priority(20)]
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
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/create-user", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "User already exists"); 
    }

    /// <summary>
    /// User cannot be created if email is not supplied
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedTheory("IT-229"), Priority(30)]
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
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/create-user", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The field 'E-mail' cannot be empty");
    }

    /// <summary>
    /// User cannot be created if email is invalid
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedTheory("IT-230"), Priority(40)]
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
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/create-user", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "E-mail is invalid");
    }

    /// <summary>
    /// User cannot be created if email is already taken
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-263"), Priority(45)]
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
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/create-user", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "E-mail already used");
    }

    /// <summary>
    /// User cannot be created if display name is not supplied
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedTheory("IT-231"), Priority(50)]
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
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/create-user", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The field 'Display name' cannot be empty");
    }

    /// <summary>
    /// User cannot be created if custom metadata validations are not met
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-232"), Priority(60)]
    public async Task User_cannot_be_created_when_metadata_custom_validators_do_not_pass()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User cannot be created if password is not supplied
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedTheory("IT-233"), Priority(70)]
    [InlineData(null)]
    [InlineData("")]
    public async Task User_cannot_be_created_when_password_is_null_or_empty(string password)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User cannot be created if passwords do not metch
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-234"), Priority(80)]
    public async Task User_cannot_be_created_when_passwords_do_not_match()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User cannot be created if no role is supplied (null list)
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-236"), Priority(90)]
    public async Task User_cannot_be_created_when_role_list_is_null()
    {
        // Prepare
        var request = new CreateUser.Request
        {
            Username = "new-user",
            DisplayName = "John Doe",
            Email = "new-user@wayne-inc.com",
            RoleIds = null,
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/create-user", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The list of roles must have at least one item");
    }

    /// <summary>
    /// User cannot be created if no role is supplied (empty list)
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-239"), Priority(100)]
    public async Task User_cannot_be_created_when_role_list_is_empty()
    {
        // Prepare
        var request = new CreateUser.Request
        {
            Username = "new-user",
            DisplayName = "John Doe",
            Email = "new-user@wayne-inc.com",
            RoleIds = new Guid[0],
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/create-user", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The list of roles must have at least one item");
    }

    /// <summary>
    /// User cannot be created if role list has an invalid role
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-240"), Priority(110)]
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
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authentication/create-user", request, true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Role not found");
    }

    /// <summary>
    /// User cannot be created if requesting user doesn't have the proper access rights
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-236"), Priority(120)]
    public async Task User_cannot_be_created_when_requester_does_not_have_proper_access_rights()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User cannot be created if custom password policies are not met
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-238"), Priority(130)]
    public async Task User_cannot_be_created_when_password_policies_are_not_met()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User can be created
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-237"), Priority(140)]
    public async Task User_can_be_created()
    {
        throw new NotImplementedException();
    }


}
