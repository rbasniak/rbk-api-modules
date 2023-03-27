using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Tests.Integration.Identity;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class UserCreationTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public UserCreationTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
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
        
        throw new NotImplementedException();
    }

    /// <summary>
    /// User cannot be created if username is already taken
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-228"), Priority(20)]
    public async Task User_cannot_be_created_when_username_is_already_taken()
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// User cannot be created if email is invalid
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedTheory("IT-230"), Priority(40)]
    [InlineData("aaaaaa")]
    [InlineData("aaaaa@bbbbbb")]
    [InlineData("aaaaaaa@bbbbbbb.")]
    [InlineData("-aaaaaaa@bbbbbbb.com")]
    [InlineData("123@bbbbbbb.com")]
    [InlineData("_aaaaa@bbbbbbb.com")]
    [InlineData("@bbbbbbb.com")]
    public async Task User_cannot_be_created_when_email_is_invalid(string email)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User cannot be created if email is already taken
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-263"), Priority(45)]
    public async Task User_cannot_be_created_when_email_is_already_taken()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User cannot be created if display name is not supplied
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-231"), Priority(50)]
    [InlineData(null)]
    [InlineData("")]
    public async Task User_cannot_be_created_when_display_name_is_null_or_empty(string displayName)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// User cannot be created if no role is supplied (empty list)
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-239"), Priority(100)]
    public async Task User_cannot_be_created_when_role_list_is_empty()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User cannot be created if role list has an invalid role
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-240"), Priority(110)]
    public async Task User_cannot_be_created_when_role_list_has_an_invlid_role()
    {
        throw new NotImplementedException();
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
