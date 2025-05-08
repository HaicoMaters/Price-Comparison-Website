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
            
            // Add headers to make the request look like a regular browser
            requestMessage.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/html"));
            requestMessage.Headers.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));
            requestMessage.Headers.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("deflate"));
            requestMessage.Headers.AcceptLanguage.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("en-GB"));
            requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36");
            
            HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            
            return response;
        }
    }
}