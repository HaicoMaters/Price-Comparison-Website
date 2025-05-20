using System.Text.Json.Nodes;

namespace PriceComparisonWebsite.Services.HttpClients
{
    /// <summary>
    /// Client for making internal API requests with authentication
    /// </summary>
    public interface IApiHttpClient
    {
        /// <summary>
        /// Sends an authenticated request to an internal API endpoint
        /// </summary>
        /// <param name="method">The HTTP method to use</param>
        /// <param name="endpoint">The API endpoint to send the request to</param>
        /// <param name="content">Optional content to send with the request</param>
        /// <returns>The HTTP response message</returns>
        Task<HttpResponseMessage> SendAsync(HttpMethod method, string endpoint, object content = null);
    }
}
