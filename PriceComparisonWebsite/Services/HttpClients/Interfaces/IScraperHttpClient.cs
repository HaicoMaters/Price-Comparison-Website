using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceComparisonWebsite.Services.HttpClients
{
    /// <summary>
    /// Client for making HTTP requests that simulate a web browser for scraping purposes
    /// </summary>
    public interface IScraperHttpClient
    {
        /// <summary>
        /// Sends an HTTP request with browser-like headers
        /// </summary>
        /// <param name="uri">The URI to send the request to</param>
        /// <param name="method">The HTTP method to use</param>
        /// <returns>The HTTP response message</returns>
        Task<HttpResponseMessage> SendRequestAsync(Uri uri, HttpMethod method);
    }
}