using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Price_Comparison_Website.Services.WebScraping.Parsers.Interfaces;

namespace Price_Comparison_Website.Services.WebScraping.Parsers
{
    public class AmazonPriceParser : IPriceParser
    {
        public bool CanParse(Uri uri) => uri.Host.Contains(SupportedDomain); // For each unique add the relevant vendor here

        public string SupportedDomain => "amazon.co.uk";

        public Task<(decimal Price, decimal DiscountedPrice)> ParsePriceAsync(HttpResponseMessage httpResponse)
        {
            throw new NotImplementedException();
        }
    }
}