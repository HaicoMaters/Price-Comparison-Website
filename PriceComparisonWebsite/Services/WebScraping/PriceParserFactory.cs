using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PriceComparisonWebsite.Services.WebScraping.Interfaces;
using PriceComparisonWebsite.Services.WebScraping.Parsers.Interfaces;

namespace PriceComparisonWebsite.Services.WebScraping
{
    /// <inheritdoc />
    public class PriceParserFactory : IPriceParserFactory
    {
        private readonly Dictionary<string, IPriceParser> _parsers; // domain and parser

        public PriceParserFactory(IEnumerable<IPriceParser> parsers)
        {
            _parsers = parsers.ToDictionary(p => p.SupportedDomain, p => p, StringComparer.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public IEnumerable<string> GetAllSupportedParsers() => _parsers.Keys;

        /// <inheritdoc />
        public IPriceParser? GetParserForDomain(string domain)
        {
            _parsers.TryGetValue(domain, out var parser);
            Console.WriteLine($"Parser for domain '{domain}': {parser?.GetType().Name ?? "None"}"); // Debugging line
            return parser;
        }

        /// <inheritdoc />
        public bool HasParserForDomain(string domain) => _parsers.ContainsKey(domain);
    }
}