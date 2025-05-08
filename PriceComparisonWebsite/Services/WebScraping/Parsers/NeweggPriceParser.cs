using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using PriceComparisonWebsite.Services.WebScraping.Parsers.Interfaces;
using PriceComparisonWebsite.Services.Utilities;
using PriceComparisonWebsite.Services.Utilities.Interfaces;

namespace PriceComparisonWebsite.Services.WebScraping.Parsers
{
    public class NeweggPriceParser : IPriceParser // A site that allows scraping so thats why this was chosen
    {
        private readonly IContentCompressor _compressor;

        public NeweggPriceParser(IContentCompressor compressor)
        {
            _compressor = compressor;
        }

        public string SupportedDomain => "www.newegg.com";

        public bool CanParse(Uri uri) => uri.Host.Contains(SupportedDomain);

        public async Task<(decimal Price, decimal DiscountedPrice)> ParsePriceAsync(HttpResponseMessage httpResponse)
        {
            var content = await httpResponse.Content.ReadAsByteArrayAsync();
            var htmlContent = await _compressor.DecompressAsync(content);

            var document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            // currently based off of certain page structures: 
            // https://www.newegg.com/global/uk-en/amd-ryzen-7-9700x-ryzen-7-9000-series-granite-ridge-socket-am5-processor/p/N82E16819113843 (this was discounted at the time) is also seen what it looked like in the parser tests
            // https://www.newegg.com/global/uk-en/aspire-14-ai/p/N82E16834360378 (this was not discounted at the time)

            decimal price = 0;
            decimal discountedPrice = 0; 

            var originalPriceNode = document.DocumentNode.SelectSingleNode("//*[@id='newProductPageContent']/div/div/div/div[1]/div[1]/div[1]/div/ul/div[1]/div[3]/li/span"); // node exists if it is discounted (at least for some pages)

            var priceNodeWhole = document.DocumentNode.SelectSingleNode("//*[@id='newProductPageContent']/div/div/div/div[1]/div[1]/div[1]/div/ul/div[1]/div[2]/strong"); // The current price node is split into two parts: whole and decimal
            var priceNodeDecimal = document.DocumentNode.SelectSingleNode("//*[@id='newProductPageContent']/div/div/div/div[1]/div[1]/div[1]/div/ul/div[1]/div[2]/sup");

            if (originalPriceNode != null)
            {
                price = PriceExtractor.ExtractPriceAmazonFormat(originalPriceNode.InnerText);
                bool wholeParsed = decimal.TryParse(priceNodeWhole.InnerText, out decimal wholePart); 
                bool decimalParsed = decimal.TryParse(priceNodeDecimal.InnerText, out decimal decimalPart);
                if (wholeParsed && decimalParsed)
                {
                    discountedPrice = wholePart + decimalPart;
                }
            }
            else if (priceNodeWhole != null)
            {
                bool wholeParsed = decimal.TryParse(priceNodeWhole.InnerText, out decimal wholePart); 
                bool decimalParsed = decimal.TryParse(priceNodeDecimal.InnerText, out decimal decimalPart);
                if (wholeParsed && decimalParsed)
                {
                    price = wholePart + decimalPart;
                    discountedPrice = price; 
                }
            }

            // If discounted price exists, return both discounted price and the regular price
            // If no discount is found, the regular price is returned as both
            return (price, discountedPrice);
        }
    }
}