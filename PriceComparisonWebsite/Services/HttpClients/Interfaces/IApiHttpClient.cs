using System.Text.Json.Nodes;

namespace PriceComparisonWebsite.Services.HttpClients
{
    public interface IApiHttpClient
    {
        Task<HttpResponseMessage> SendAsync(HttpMethod method, string endpoint, object content = null);
    }
}
