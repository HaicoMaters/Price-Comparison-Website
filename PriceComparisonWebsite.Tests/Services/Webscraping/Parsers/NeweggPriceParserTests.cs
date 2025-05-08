using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using PriceComparisonWebsite.Services.Utilities;
using PriceComparisonWebsite.Services.Utilities.Interfaces;
using PriceComparisonWebsite.Services.WebScraping.Parsers;
using PriceComparisonWebsite.Services.WebScraping.Parsers.Interfaces;

namespace PriceComparisonWebsite.Tests.Services.Webscraping.Parsers
{
    public class NeweggPriceParserTests
    {
        IFileSystemWrapper _fileSystem;
        readonly string folderName;
        string DiscountedPageContent; // From the page provided discounted price is £319.19 and normal price is £327.59 (was the response content from https://www.newegg.com/global/uk-en/amd-ryzen-7-9700x-ryzen-7-9000-series-granite-ridge-socket-am5-processor/p/N82E16819113843 on 08/05/2025)
        string NonDiscountedPageContent; // From page provided price was £827.99 with no discounted price (was the response content from https://www.newegg.com/global/uk-en/aspire-14-ai/p/N82E16834360378 on 08/05/2025)
        IPriceParser neweggPriceParser;

        public NeweggPriceParserTests()
        {
            folderName = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "Services", "Webscraping", "Parsers", "TestHttpResponses");
            _fileSystem = new FileSystemWrapper();
            neweggPriceParser = new NeweggPriceParser();
        }

        [Fact]
        public async Task SetupPageContent()
        {
            var DiscountedFilePath = Path.Combine(folderName, "NeweggDiscountedPageContent.txt");
            var NonDiscountedFilePath = Path.Combine(folderName, "NeweggNonDiscountedPageContent.txt");

            Assert.True(_fileSystem.FileExists(DiscountedFilePath), $"File not found: {DiscountedFilePath}");
            Assert.True(_fileSystem.FileExists(NonDiscountedFilePath), $"File not found: {NonDiscountedFilePath}");

            DiscountedPageContent = await _fileSystem.ReadAllTextAsync(DiscountedFilePath);
            NonDiscountedPageContent = await _fileSystem.ReadAllTextAsync(NonDiscountedFilePath);
        }

        // ---------------------------------- Can Parse -----------------------------------------------------------
        [Fact]
        public void CanParse_WhenAmazonUri_ShouldReturnTrue()
        {
            // Arrange
            Uri uri = new Uri("https://www.newegg.com");

            // Act
            var CanParse = neweggPriceParser.CanParse(uri);

            // Assert
            Assert.True(CanParse);
        }

        [Fact]
        public void CanParse_WhenNonAmazonUri_ShouldReturnFalse()
        {
            // Arrange
            Uri uri = new Uri("https://www.ebay.co.uk");

            // Act
            var CanParse = neweggPriceParser.CanParse(uri);

            // Assert
            Assert.False(CanParse);
        }

        // ---------------------------------- Supported Domain ------------------------------------
        [Fact]
        public void SupportedDomain_ShouldBeSetToCorrectDomain()
        {
            // Act and Assert
            Assert.Equal("www.newegg.com", neweggPriceParser.SupportedDomain);
        }

        // ------------------------------------------ ParsePriceAsync --------------------------------------
        [Fact]
        public async Task ParsePriceAsynce_WhenProductIsDiscounted_ShouldReturnBothPriceAndDiscountedPrice()
        {
            // Arrange
            await SetupPageContent();

            decimal expectedDiscountedPrice = 319.19m;
            decimal expectedPrice = 327.59m;

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(DiscountedPageContent)
            };

            // Act
            var prices = await neweggPriceParser.ParsePriceAsync(response);

            // Assert
            Assert.Equal(expectedPrice, prices.Price);
            Assert.Equal(expectedDiscountedPrice, prices.DiscountedPrice);
        }

        [Fact]
        public async Task ParsePriceAsynce_WhenProductIsNotDiscounted_ShouldReturnBothPriceAndDiscountedPriceAsTheValueOfPrice()
        {
            // Arrange
            await SetupPageContent();

            decimal expectedDiscountedPrice = 827.99m;
            decimal expectedPrice = 827.99m;

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(NonDiscountedPageContent)
            };

            // Act
            var prices = await neweggPriceParser.ParsePriceAsync(response);

            // Assert
            Assert.Equal(expectedPrice, prices.Price);
            Assert.Equal(expectedDiscountedPrice, prices.DiscountedPrice);
        }

        [Fact]
        public async Task ParsePriceAsynce_WhenNonValidPageLayout_ShouldReturnBothPriceAndDiscountedPriceAsZero()
        {
            // Arrange
            decimal expectedDiscountedPrice = 0;
            decimal expectedPrice = 0;

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("NotValidContent")
            };

            // Act
            var prices = await neweggPriceParser.ParsePriceAsync(response);

            // Assert
            Assert.Equal(expectedPrice, prices.Price);
            Assert.Equal(expectedDiscountedPrice, prices.DiscountedPrice);
        }
    }
}
