using System.Security.Cryptography;

namespace rbkApiModules.Tests.Integration.Identity;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class UserChangePasswordTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public UserChangePasswordTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture; 
    }

    /// <summary>
    /// User cannot change password if old password doesn't match with the new one
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-220"), Priority(10)]
    public async Task User_Cannot_Change_Password_If_Old_Password_Doesnt_Match()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User cannot change password if new and confirmation passwords don't match
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-221"), Priority(20)]
    public async Task User_Cannot_Change_Password_If_New_And_COnfirmation_Passwords_Dont_Match()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User cannot change password if custom policies aren't met
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-222"), Priority(30)]
    public async Task User_Cannot_Change_Password_If_Custom_Policies_Arent_Met()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User can change his password
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-226"), Priority(40)]
    public async Task User_Can_Change_His_Password()
    {
        throw new NotImplementedException();
    }
}
