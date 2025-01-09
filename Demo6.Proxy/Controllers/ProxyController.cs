using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Core;
namespace Demo6.Proxy.Controllers;

[ApiController]
[Route("api/proxy")]
public class ProxyController: BaseController
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ProxyController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("test")]
    public async Task<ActionResult> Test()
    {
        var client = _httpClientFactory.CreateClient("ProcessingApi");

        var response = await client.GetAsync("/process");
        var result = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            return Ok(result);
        }

        return StatusCode((int)response.StatusCode);
    }
}
