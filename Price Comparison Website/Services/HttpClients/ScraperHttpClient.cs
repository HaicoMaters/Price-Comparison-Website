using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Price_Comparison_Website.Services.HttpClients
{
    public class ScraperHttpClient : IScraperHttpClient
    {
        public Task<HttpResponseMessage> SendRequestAsync(Uri uri, HttpMethod method)
        {
            throw new NotImplementedException();
        }

        public void SetupClient()
        {
            throw new NotImplementedException();
        }
    }
}