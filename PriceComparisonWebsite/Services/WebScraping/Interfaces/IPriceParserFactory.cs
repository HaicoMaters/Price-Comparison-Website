using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceComparisonWebsite.Services.WebScraping.Parsers.Interfaces;

namespace PriceComparisonWebsite.Services.WebScraping.Interfaces
{
    /// <summary>
    /// Factory for creating and managing price parsers for different vendors
    /// </summary>
    public interface IPriceParserFactory
    {
        /// <summary>
        /// Gets a price parser for the specified domain
        /// </summary>
        /// <param name="domain">The domain to get a parser for</param>
        IPriceParser? GetParserForDomain(string domain);

        /// <summary>
        /// Checks if a parser exists for the specified domain
        /// </summary>
        /// <param name="domain">The domain to check</param>
        bool HasParserForDomain(string domain);

        /// <summary>
        /// Gets all domains that have supported parsers
        /// </summary>
        IEnumerable<string> GetAllSupportedParsers();
    }
}