using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Price_Comparison_Website.Services.Utilities.Interfaces;

namespace Price_Comparison_Website.Services.Utilities
{
    public static class PriceExtractor
    {
        public static decimal ExtractPrice(string priceText)
        {
            // Regex to match price patterns such as £4.31, 3.56, etc.
            var regex = new Regex(@"£?(\d+\.\d{2})");

            // Match the regex to the priceText
            var match = regex.Match(priceText);

            // If a match is found, return the decimal value
            if (match.Success)
            {
                return decimal.Parse(match.Groups[1].Value);
            }

            // Return 0 if no price is found (or handle as needed)
            return 0m;
        }
    }
}