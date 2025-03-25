using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Price_Comparison_Website.Services.WebScraping.Parsers.Interfaces;

namespace Price_Comparison_Website.Services.WebScraping.Interfaces
{
    public interface IPriceParserFactory
    {
        bool HasParserForDomain(string domain);
        IPriceParser? GetParserForDomain(string domain);
        IEnumerable<string> GetAllSupportedParsers();
    }
}