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
    [FriendlyNamedFact("IT-470"), Priority(10)]
    public async Task Redefine_password_endpoint_should_not_be_available()
    {
        // Prepare
        var request = new RedefinePassword.Request();

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/redefine-password", request, credentials: null);

        // Assert
        response.ShouldHaveErrors(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// With Windows Authentication  reset password  endpoint should not be available
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-471"), Priority(20)]
    public async Task Reset_password_endpoint_should_not_be_available()
    {
        // Prepare
        var request = new RequestPasswordReset.Request();

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/reset-password", request, credentials: null);

        // Assert
        response.ShouldHaveErrors(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// With Windows Authentication resend confirmation endpoint should not be available
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-472"), Priority(30)]
    public async Task Resend_confirmation_endpoint_should_not_be_available()
    {
        // Prepare
        var request = new ResendEmailConfirmation.Request();

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/resend-confirmation", request, credentials: null);

        // Assert
        response.ShouldHaveErrors(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// With Windows Authentication confirm e-mail  endpoint should not be available
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-473"), Priority(40)]
    public async Task Confirm_email_endpoint_should_not_be_available()
    {
        // Act
        var response = await _serverFixture.GetAsync<JwtResponse>("api/authentication/confirm-email?email=aaaa@aaa.com&code=aaaaaaaaaa&tenant=aaaaaa", credentials: null);

        // Assert
        response.ShouldHaveErrors(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// With Windows Authentication change password endpoint should not be available
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-474"), Priority(50)]
    public async Task Change_password_endpoint_should_not_be_available()
    {
        // Prepare
        var request = new ChangePassword.Request();

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/change-password", request, credentials: null);

        // Assert
        response.ShouldHaveErrors(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// With Windows Authentication register endpoint should not be available
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("IT-475"), Priority(60)]
    public async Task Register_endpoint_should_not_be_available()
    {
        // Prepare
        var request = new Register.Request();

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/register", request, credentials: null);

        // Assert
        response.ShouldHaveErrors(HttpStatusCode.NotFound);
    }
}