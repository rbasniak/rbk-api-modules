using System.Security.Cryptography;

namespace rbkApiModules.Demo1.Tests.Integration.Identity;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class EndpointsAvailabilityTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public EndpointsAvailabilityTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    /// <summary>
    /// Without Windows Authentication switch domain endpoint should not be available
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-290"), Priority(10)]
    public async Task Switch_domain_endpoint_should_not_be_available()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Without Windows Authentication create user endpoint should not be available
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-291"), Priority(20)]
    public async Task Create_user_endpoint_should_not_be_available()
    {
        throw new NotImplementedException();
    }
}