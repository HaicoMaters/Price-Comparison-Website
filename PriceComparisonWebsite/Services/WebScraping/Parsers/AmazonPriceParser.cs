using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using PriceComparisonWebsite.Services.Utilities;
using PriceComparisonWebsite.Services.Utilities.Interfaces;
using PriceComparisonWebsite.Services.WebScraping.Parsers.Interfaces;

namespace PriceComparisonWebsite.Services.WebScraping.Parsers
{
    /// <inheritdoc />
    public class AmazonPriceParser : IPriceParser
    {
        private readonly IContentCompressor _compressor;

        public AmazonPriceParser(IContentCompressor compressor)
        {
            _compressor = compressor;
        }

        /// <inheritdoc />
        public bool CanParse(Uri uri) => uri.Host.Contains(SupportedDomain);

        /// <inheritdoc />
        public string SupportedDomain => "www.amazon.co.uk";

        /// <inheritdoc />
        public async Task<(decimal Price, decimal DiscountedPrice)> ParsePriceAsync(HttpResponseMessage httpResponse)
        {
            var content = await httpResponse.Content.ReadAsByteArrayAsync();
            var htmlContent = await _compressor.DecompressAsync(content);

            var document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            // currently based off of certain page structures: 
            // https://www.amazon.co.uk/UNO-W2087-Card-Game-European/dp/B005I5M2F8/259-3693489-2210466 (this was discounted at the time)
            // https://www.amazon.co.uk/Coca-Cola-Zero-7-92-Litre/dp/B007C7KGKI/ (this was not discounted at the time)

            decimal price = 0;
            decimal discountedPrice = 0; 

            var originalPriceNode = document.DocumentNode.SelectSingleNode("//*[@id='corePriceDisplay_desktop_feature_div']/div[2]/span/span[1]/span[1]"); // node exists if it is discounted (at least for some pages)
            var priceNode = document.DocumentNode.SelectSingleNode("//*[@id='corePriceDisplay_desktop_feature_div']/div[1]/span[1]");

            if (originalPriceNode != null)
            {
                price = PriceExtractor.ExtractPriceAmazonFormat(originalPriceNode.InnerText);
                discountedPrice = PriceExtractor.ExtractPriceAmazonFormat(priceNode.InnerText);
            }
            else if (priceNode != null)
            {
                price = PriceExtractor.ExtractPriceAmazonFormat(priceNode.InnerText);
                discountedPrice = price;
            }

            // If discounted price exists, return both discounted price and the regular price
            // If no discount is found, the regular price is returned as both
            return (price, discountedPrice);
        }
    }
}