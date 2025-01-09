using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace rbkApiModules.MultiApiIntegration.Tests.Integration;

public class MultiApiIntegrationTests
{
    private static WebApplicationFactory<Demo6.Proxy.Program> _captureApiFactory;
    private static WebApplicationFactory<Demo6.Processing.Program> _processingApiFactory;
    private static HttpClient _capturerApiClient;
    private static HttpClient _processingApiClient;

    [Before(Class)]
    public static void SetUp()
    {
        _processingApiFactory = new WebApplicationFactory<Demo6.Processing.Program>();
        _processingApiClient = _processingApiFactory.CreateClient();

        _captureApiFactory = new WebApplicationFactory<Demo6.Proxy.Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddHttpClient("ProcessingApi")
                        .ConfigurePrimaryHttpMessageHandler(() => _processingApiFactory.Server.CreateHandler());
            });
        });

        _capturerApiClient = _captureApiFactory.CreateClient();
    }

    [After(Class)]
    public static void TearDown()
    {
        _capturerApiClient.Dispose();
        _processingApiClient.Dispose();
        _captureApiFactory.Dispose();
        _processingApiFactory.Dispose();
    }

    [Test]
    public async Task Test_Proxy_TestEndpoint_ShouldReturnSuccess()
    {
        // Arrange
        var processorResponse = await _processingApiClient.GetAsync("/process");
        processorResponse.EnsureSuccessStatusCode();

        // Act
        var response = await _capturerApiClient.GetAsync("/api/proxy/test");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        await Assert.That(content).IsEqualTo("Processing completed successfully");
    }
}