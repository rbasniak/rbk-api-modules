using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Paypal.SqlServer
{
    public interface IPaypalService
    {
        Task<string> LoginAsync();
        Task<Subscription> GetSubscriptionAsync(string token, string id);
        Task<Error> CancelSubscriptionAsync(string token, string id, string reason);
        Task<string> ValidateWebhookSignature(string bodyObject, IHeaderDictionary header);
    }

    public class PaypalService : IPaypalService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly PaypalSettings _settings;

        public PaypalService(IHttpClientFactory clientFactory, IOptions<PaypalSettings> settings)
        {
            _clientFactory = clientFactory;
            _settings = settings.Value;
        }

        public async Task<Error> CancelSubscriptionAsync(string token, string id, string reason)
        {
            var request = new HttpRequestMessage(HttpMethod.Post,
                "https://api.paypal.com/v1/billing/subscriptions/" + id + "/cancel");
            request.Headers.Add("authorization", "Bearer " + token);

            var client = _clientFactory.CreateClient();

            var json = JsonConvert.SerializeObject(new CancelRequest() { Reason = reason });
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return null;
            }
            else
            {
                return JsonConvert.DeserializeObject<Error>(
                    await response.Content.ReadAsStringAsync());
            }
        }

        public async Task<Subscription> GetSubscriptionAsync(string token, string id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                "https://api.paypal.com/v1/billing/subscriptions/" + id);
            request.Headers.Add("authorization", "Bearer " + token);

            return await GetPaypalObject<Subscription>(request);
        }

        public async Task<string> LoginAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Post,
                "https://api.paypal.com/v1/oauth2/token");

            var loginPass = string.Format("{0}:{1}", _settings.PaypalClientID, _settings.PaypalSecret);
            var encodedBytes = Encoding.UTF8.GetBytes(loginPass);
            var base64LoginPass = Convert.ToBase64String(encodedBytes);

            request.Headers.Add("authorization", "Basic " + base64LoginPass);
            var body = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            };
            request.Content = new FormUrlEncodedContent(body);

            var token = await GetPaypalObject<Token>(request);

            return token != null ? token.AccessToken : string.Empty;
        }

        public async Task<string> ValidateWebhookSignature(string requestBody, IHeaderDictionary header)
        {
            var token = await LoginAsync();

            var headerNameValueCollection = GetNameValueCollectionHeader(header);

            var request = new HttpRequestMessage(HttpMethod.Post,
                "https://api.paypal.com/v1/notifications/verify-webhook-signature");
            request.Headers.Add("authorization", "Bearer " + token);

            var body = new PaypalWebhookSignatureVerification
            {
                AuthAlgo = headerNameValueCollection["PAYPAL-AUTH-ALGO"],
                CertUrl = headerNameValueCollection["PAYPAL-CERT-URL"],
                TransmissionId = headerNameValueCollection["PAYPAL-TRANSMISSION-ID"],
                TransmissionSig = headerNameValueCollection["PAYPAL-TRANSMISSION-SIG"],
                TransmissionTime = headerNameValueCollection["PAYPAL-TRANSMISSION-TIME"],
                WebhookId = _settings.WebhookId,
                WebhookEvent = "**xx**",
            };
            var json = JsonConvert.SerializeObject(body, Formatting.Indented).Replace("\"**xx**\"", requestBody);

            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await GetPaypalObject<PaypalWebhookEventResponse>(request);

            return response.VerificationStatus;
        }

        private async Task<T> GetPaypalObject<T>(HttpRequestMessage request)
        {
            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(data);
            }
            else
            {
                return default;
            }
        }

        private NameValueCollection GetNameValueCollectionHeader(IHeaderDictionary header)
        {
            var nameValueCollection = new NameValueCollection();
            foreach (var kvp in header)
            {
                nameValueCollection.Add(kvp.Key.ToString(), kvp.Value.ToString());
            }

            return nameValueCollection;
        }
    }
}
