using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities
{
    public abstract class BaseHttpService
    {
        protected readonly IHttpClientFactory _clientFactory;

        public BaseHttpService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        protected async Task<HttpResponse<T>> SendAsync<T>(HttpMethod method, string url, object data = null)
        {
            var request = new HttpRequestMessage(method, url);
            // request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            if ((method == HttpMethod.Post || method == HttpMethod.Put) && data != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            }

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<T>(jsonContent);
                return new HttpResponse<T>(response.StatusCode, result);
            }
            else
            {
                // TODO: Ler a msg de erro do back e diferenciar 400, 401, 403 e 500
                return new HttpResponse<T>(response.StatusCode, new[] { "Erro na requisição" });
            } 
        }
    }

}
