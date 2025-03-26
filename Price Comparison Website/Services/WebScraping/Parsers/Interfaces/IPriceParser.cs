using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Price_Comparison_Website.Services.WebScraping.Parsers.Interfaces
{
    public interface IPriceParser
    {
        string SupportedDomain { get; }
        bool CanParse (Uri uri);
        Task<(decimal Price, decimal DiscountedPrice)> ParsePriceAsync(HttpResponseMessage httpResponse);
    }
}