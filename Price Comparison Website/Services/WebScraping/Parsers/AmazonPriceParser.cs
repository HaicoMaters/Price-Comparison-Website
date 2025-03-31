using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Price_Comparison_Website.Services.Utilities;
using Price_Comparison_Website.Services.WebScraping.Parsers.Interfaces;

namespace Price_Comparison_Website.Services.WebScraping.Parsers
{
    public class AmazonPriceParser : IPriceParser
    {
        public bool CanParse(Uri uri) => uri.Host.Contains(SupportedDomain);

        public string SupportedDomain => "www.amazon.co.uk";

        public async Task<(decimal Price, decimal DiscountedPrice)> ParsePriceAsync(HttpResponseMessage httpResponse) // This can always be updated to support more page layouts (i noticed some page layouts are not the same)
        {
            var htmlContent = await httpResponse.Content.ReadAsStringAsync();

            var document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            // currently based off of certain page structures: 
            // https://www.amazon.co.uk/UNO-Families-Collectible-Cards-Instructions/dp/B08WKF5HZR/ (this was discounted at the time)
            // https://www.amazon.co.uk/Coca-Cola-Zero-7-92-Litre/dp/B007C7KGKI/ (this was not discounted at the time)

            decimal price = 0;
            decimal discountedPrice = 0; 

            var originalPriceNode = document.DocumentNode.SelectSingleNode("//*[@id='corePriceDisplay_desktop_feature_div']/div[2]/span/span[1]/span[1]"); // node exists if it is discounted (at least for some pages)
            var priceNode = document.DocumentNode.SelectSingleNode("//*[@id='corePriceDisplay_desktop_feature_div']/div[1]/span[1]");

            if (originalPriceNode != null)
            {
                price = PriceExtractor.ExtractPrice(originalPriceNode.InnerText);
                discountedPrice = PriceExtractor.ExtractPrice(priceNode.InnerText);
            }
            else if (priceNode != null)
            {
                price = PriceExtractor.ExtractPrice(priceNode.InnerText);
                discountedPrice = price;
            }

            // If discounted price exists, return both discounted price and the regular price
            // If no discount is found, the regular price is returned as both
            return (price, discountedPrice);
        }
    }
}