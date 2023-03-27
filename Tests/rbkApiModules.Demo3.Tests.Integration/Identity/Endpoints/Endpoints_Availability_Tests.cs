using System.Security.Cryptography;

namespace rbkApiModules.Demo3.Tests.Integration.Identity;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class EndpointsAvailabilityTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public EndpointsAvailabilityTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    /// <summary>
    /// With Windows Authentication  redefine password endpoint should not be available
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-270"), Priority(10)]
    public async Task Redefine_password_endpoint_should_not_be_available()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// With Windows Authentication  reset password  endpoint should not be available
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-271"), Priority(20)]
    public async Task Reset_password_endpoint_should_not_be_available()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// With Windows Authentication resend confirmation endpoint should not be available
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-272"), Priority(30)]
    public async Task Resend_confirmation_endpoint_should_not_be_available()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// With Windows Authentication confirm e-mail  endpoint should not be available
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-273"), Priority(40)]
    public async Task Confirm_email_endpoint_should_not_be_available()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// With Windows Authentication change password endpoint should not be available
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-274"), Priority(50)]
    public async Task Change_password_endpoint_should_not_be_available()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// With Windows Authentication register endpoint should not be available
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-275"), Priority(60)]
    public async Task Register_endpoint_should_not_be_available()
    {
        throw new NotImplementedException();
    }
}