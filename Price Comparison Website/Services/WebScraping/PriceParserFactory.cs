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

        private readonly Dictionary<string, IPriceParser> _parsers;

        public PriceParserFactory(IEnumerable<IPriceParser> parsers)
        {
            _parsers = parsers.ToDictionary(p => p.SupportedDomain, p => p, StringComparer.OrdinalIgnoreCase);
        }

        public IEnumerable<string> GetAllSupportedParsers() => _parsers.Keys;

        public IPriceParser? GetParserForDomain(string domain)
        {
            _parsers.TryGetValue(domain, out var parser);
            return parser;
        }

        public bool HasParserForDomain(string domain) => _parsers.ContainsKey(domain);
    }
}