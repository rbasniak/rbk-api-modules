using System.Net.Http.Json;

namespace Demo1.Clients;

public class Demo2ApiClient(HttpClient httpClient) : IDemo2ApiClient
{
    public async Task<string> GetAnonymousMessageAsync(CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync("/demo/anonymous", cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<DemoMessageResponse>(cancellationToken)
            ?? throw new InvalidOperationException("Demo2 returned an empty response.");

        return payload.message;
    }

    private sealed record DemoMessageResponse(string message);
}
