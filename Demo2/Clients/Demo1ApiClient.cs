using System.Net.Http.Json;

namespace Demo2.Clients;

public class Demo1ApiClient(HttpClient httpClient) : IDemo1ApiClient
{
    public async Task<string> GetAnonymousMessageAsync(CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync("/demo/anonymous", cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<DemoMessageResponse>(cancellationToken)
            ?? throw new InvalidOperationException("Demo1 returned an empty response.");

        return payload.message;
    }

    private sealed record DemoMessageResponse(string message);
}
