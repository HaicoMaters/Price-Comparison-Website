using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceComparisonWebsite.Services.WebScraping.Interfaces;
using PriceComparisonWebsite.Services.WebScraping.Parsers.Interfaces;

namespace PriceComparisonWebsite.Services.WebScraping
{
    public class PriceParserFactory : IPriceParserFactory
    {
        private readonly Dictionary<string, IPriceParser> _parsers; // domain and parser

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