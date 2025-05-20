using System.Text;
using System.Text.Json;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

namespace PriceComparisonWebsite.Services.HttpClients
{
    /// <inheritdoc />
    public class ApiHttpClient : IApiHttpClient
    {
        private readonly HttpClient _httpClient;

        public ApiHttpClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            ConfigureClient(configuration);
        }

        private void ConfigureClient(IConfiguration configuration)
        {
            _httpClient.BaseAddress = new Uri(configuration["LocalhostUrl"]);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("X-Internal-Client", "true");
            _httpClient.DefaultRequestHeaders.Add("X-Internal-Auth", configuration["InternalApi:Key"]);
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> SendAsync(HttpMethod method, string endpoint, object content = null)
        {
            var request = new HttpRequestMessage(method, endpoint);
            
            if (content != null)
            {
                var json = JsonSerializer.Serialize(content);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(request);
            return response;
        }
    }
}
