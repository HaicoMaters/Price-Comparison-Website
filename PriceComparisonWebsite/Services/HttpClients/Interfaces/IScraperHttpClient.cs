using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceComparisonWebsite.Services.HttpClients
{
    public interface IScraperHttpClient
    {
        Task<HttpResponseMessage> SendRequestAsync( Uri uri, HttpMethod method);
    }
}