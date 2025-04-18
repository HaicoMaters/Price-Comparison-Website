using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;


namespace PriceComparisonWebsite.Services.HttpClients
{
    public class ScraperHttpClient : IScraperHttpClient
    {
        private readonly HttpClient _httpClient;

        public ScraperHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> SendRequestAsync(Uri uri, HttpMethod method)
        {
            var requestMessage = new HttpRequestMessage(method, uri);
            HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);

            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}