namespace rbkApiModules.Demo3.Tests.Integration.Identity;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class LocalizationTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public LocalizationTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    /// <summary>
    /// Localized resources in the project should return the default description value
    /// </summary>
    [FriendlyNamedFact("IT-406"), Priority(10)]
    public async Task Localized_resources_from_project_should_return_value_of_description()
    {
        using (var httpClient = _serverFixture.Server.CreateClient())
        {
            // Act
            var response = await httpClient.GetAsync("api/demo/localization/resource-from-application/with-description");

            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            response.IsSuccessStatusCode.ShouldBeTrue();
            responseBody.ShouldBe("Message from description attribute");
        }
    }

    /// <summary>
    /// Localized resources in the project should return the enum value when there is no description attribute
    /// </summary>
    [FriendlyNamedFact("IT-407"), Priority(20)]
    public async Task Localized_resources_from_project_should_return_enum_value_when_there_is_no_description()
    {
        using (var httpClient = _serverFixture.Server.CreateClient())
        {
            // Act
            var response = await httpClient.GetAsync("api/demo/localization/resource-from-application/without-description");

            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            response.IsSuccessStatusCode.ShouldBeTrue();
            responseBody.ShouldBe("ValueWithoutDescription");
        }
    }

    /// <summary>
    /// Localization should follow application setup when there is no localization header in the request 
    /// </summary>
    [FriendlyNamedFact("IT-408"), Priority(30)]
    public async Task Localized_resources_should_return_correct_localized_string_from_application_setup()
    {
        using (var httpClient = _serverFixture.Server.CreateClient())
        {
            // Act
            var response = await httpClient.GetAsync("api/demo/localization/localized-string");

            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            response.IsSuccessStatusCode.ShouldBeTrue();
            responseBody.ShouldBe("Credenciais inválidas");
        }
    }

    /// <summary>
    /// Localization should priorize the localization header in the request
    /// </summary>
    [FriendlyNamedFact("IT-409"), Priority(40)]
    public async Task Localized_resources_should_priorize_the_localization_header_in_the_request()
    {
        using (var httpClient = _serverFixture.Server.CreateClient())
        {
            // Prepare
            httpClient.DefaultRequestHeaders.Add("localization", "en-us");

            // Act
            var response = await httpClient.GetAsync("api/demo/localization/localized-string");

            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            response.IsSuccessStatusCode.ShouldBeTrue();
            responseBody.ShouldBe("Invalid credentials");
        }
    }
}

