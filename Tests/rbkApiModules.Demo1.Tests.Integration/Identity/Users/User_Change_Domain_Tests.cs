using System.Security.Cryptography;

namespace rbkApiModules.Demo1.Tests.Integration.Identity;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class UserChangeDomainTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public UserChangeDomainTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture; 
    }

    /// <summary>
    /// User cannot change domain if the domain doesn't exist in database
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-223"), Priority(10)]
    public async Task User_Cannot_Change_Domain_If_Destination_Domain_Doesnt_Exist()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User cannot change domain if its user doesn't exist in the new domain
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-224"), Priority(20)]
    public async Task User_Cannot_Change_Domain_If_He_Doesnt_Exist_In_Destination_Domain()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User can change domain 
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-226"), Priority(30)]
    public async Task User_Can_Change_Domain()
    {
        throw new NotImplementedException();
    }
}
