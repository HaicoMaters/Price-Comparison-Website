using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PriceComparisonWebsite.Services.Utilities.Interfaces;

namespace PriceComparisonWebsite.Services.Utilities
{
    public static class PriceExtractor
    {
        public static decimal ExtractPriceAmazonFormat(string priceText)
        {
            var regex = new Regex(@"[£$€]?\s*(\d+\.\d{2})");

            var match = regex.Match(priceText);

            if (match.Success)
            {
                return decimal.Parse(match.Groups[1].Value);
            }

            // Return 0 if no price is found
            return 0m;
        }
    }
}