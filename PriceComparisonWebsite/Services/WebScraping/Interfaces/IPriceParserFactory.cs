using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceComparisonWebsite.Services.WebScraping.Parsers.Interfaces;

namespace PriceComparisonWebsite.Services.WebScraping.Interfaces
{
    public interface IPriceParserFactory
    {
        bool HasParserForDomain(string domain);
        IPriceParser? GetParserForDomain(string domain);
        IEnumerable<string> GetAllSupportedParsers();
    }
}