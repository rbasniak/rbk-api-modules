using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Tests.Integration.Identity;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class UserRegistrationTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public UserRegistrationTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    /// <summary>
    /// User cannot register if username is not provided
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedTheory("IT-250"), Priority(10)]
    [InlineData(null)]
    [InlineData("")]
    public async Task User_cannot_register_when_username_is_null_or_empty(string username)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User cannot register if username is already taken
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-251"), Priority(20)]
    public async Task User_cannot_register_when_username_is_already_taken()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User cannot register if tenant does not exist
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-252"), Priority(30)]
    public async Task User_cannot_register_when_tenant_does_not_exist()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User cannot register if tenant is not provided
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedTheory("IT-253"), Priority(40)]
    [InlineData(null)]
    [InlineData("")]
    public async Task User_cannot_register_when_tenant_is_null_or_empty(string tenant)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User cannot register if email is not provided
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedTheory("IT-254"), Priority(50)]
    [InlineData(null)]
    [InlineData("")]
    public async Task User_cannot_register_when_email_is_null_or_empty(string email)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User cannot register if email is not valid
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedTheory("IT-255"), Priority(60)]
    [InlineData("aaaaaa")]
    [InlineData("aaaaa@bbbbbb")]
    [InlineData("aaaaaaa@bbbbbbb.")]
    [InlineData("-aaaaaaa@bbbbbbb.com")]
    [InlineData("123@bbbbbbb.com")]
    [InlineData("_aaaaa@bbbbbbb.com")]
    [InlineData("@bbbbbbb.com")]
    public async Task User_cannot_register_when_email_is_invalid(string email)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User cannot register if email is already taken
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-262"), Priority(65)]
    public async Task User_cannot_register_when_email_is_already_taken()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User cannot register if display name is not provided
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedTheory("IT-256"), Priority(70)]
    [InlineData(null)]
    [InlineData("")]
    public async Task User_cannot_register_when_display_name_is_null_or_empty(string displayName)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User cannot register if metadata custom validators are not met
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-257"), Priority(80)]
    public async Task User_cannot_register_when_metadata_custom_validators_do_not_pass()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User cannot register if password is not provided
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedTheory("IT-258"), Priority(90)]
    [InlineData(null)]
    [InlineData("")]
    public async Task User_cannot_register_when_password_is_null_or_empty(string password)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User cannot register if passwords don't match
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-259"), Priority(100)]
    public async Task User_cannot_register_when_passwords_do_not_match()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User cannot register if passwords custom policies are not met
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-260"), Priority(110)]
    public async Task User_cannot_register_when_password_policies_are_not_met()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User can register
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-261"), Priority(120)]
    public async Task User_can_register()
    {
        throw new NotImplementedException();
    }
}
