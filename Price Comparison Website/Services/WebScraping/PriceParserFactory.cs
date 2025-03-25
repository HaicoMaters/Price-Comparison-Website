using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Price_Comparison_Website.Services.WebScraping.Interfaces;
using Price_Comparison_Website.Services.WebScraping.Parsers.Interfaces;

namespace Price_Comparison_Website.Services.WebScraping
{
    public class PriceParserFactory : IPriceParserFactory
    {
        /*
        Add this later to extensions for singleton logic and handling the factory correctly:
                        services.AddTransient<IPriceParser, AmazonParser>();
                        services.AddTransient<IPriceParser, EbayParser>();

                        services.AddSingleton<PriceParserFactory>(sp =>
                        {
                            var parsers = sp.GetServices<IPriceParser>();
                            return new PriceParserFactory(parsers);
                        });
        */

        public IEnumerable<string> GetAllSupportedParsers()
        {
            throw new NotImplementedException();
        }

        public IPriceParser? GetParserForDomain(string domain)
        {
            throw new NotImplementedException();
        }

        public bool HasParserForDomain(string domain)
        {
            throw new NotImplementedException();
        }
    }
}