using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceComparisonWebsite.Services.WebScraping.Parsers.Interfaces
{
    /// <summary>
    /// Interface for parsing product prices from different vendor websites
    /// </summary>
    public interface IPriceParser
    {
        /// <summary>
        /// Gets the domain name that this parser supports
        /// </summary>
        string SupportedDomain { get; }

        /// <summary>
        /// Checks if this parser can handle the given URI
        /// </summary>
        /// <param name="uri">The URI to check</param>
        bool CanParse(Uri uri);

        /// <summary>
        /// Extracts the price information from an HTTP response
        /// </summary>
        /// <param name="httpResponse">The HTTP response containing the product page HTML</param>
        /// <returns>Tuple containing the regular price and discounted price (if available)</returns>
        Task<(decimal Price, decimal DiscountedPrice)> ParsePriceAsync(HttpResponseMessage httpResponse);
    }
}