using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Price_Comparison_Website.Services.Utilities;
using Price_Comparison_Website.Services.Utilities.Interfaces;
using Price_Comparison_Website.Services.WebScraping.Parsers;
using Price_Comparison_Website.Services.WebScraping.Parsers.Interfaces;

namespace Price_Comparison_Website.Tests.Services.Webscraping.Parsers
{
    public class AmazonParserTests // Could be worth having more page options that could be parsed e.g. where layouts are different (i think i saw this for a book) and tests if that support is added
    {
        IFileSystemWrapper _fileSystem;
        readonly string folderName;
        string DiscountedPageContent; // From the page provided discounted price is £11.99 and normal price is £15.99 (was the response content from https://www.amazon.co.uk/UNO-Families-Collectible-Cards-Instructions/dp/B08WKF5HZR on 30/03/2025)
        string NonDiscountedPageContent; // From page provided price aws £9.00 with no discounted price (was the response content from https://www.amazon.co.uk/Coca-Cola-Zero-7-92-Litre/dp/B007C7KGKI on 30/03/2025)
        IPriceParser amazonPriceParser;

        public AmazonParserTests()
        {
            folderName = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "Services", "Webscraping", "Parsers", "TestHttpResponses");
            _fileSystem = new FileSystemWrapper();
            amazonPriceParser = new AmazonPriceParser();
        }

        public async Task SetupPageContent()
        {
            var DiscountedFilePath = Path.Combine(folderName, "AmazonDiscountedPageContent.txt");
            var NonDiscountedFilePath = Path.Combine(folderName, "AmazonNonDiscountedPageContent.txt");

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
            Uri uri = new Uri("https://www.amazon.co.uk");

            // Act
            var CanParse = amazonPriceParser.CanParse(uri);

            // Assert
            Assert.True(CanParse);
        }

        [Fact]
        public void CanParse_WhenNonAmazonUri_ShouldReturnFalse()
        {
            // Arrange
            Uri uri = new Uri("https://www.ebay.co.uk");

            // Act
            var CanParse = amazonPriceParser.CanParse(uri);

            // Assert
            Assert.False(CanParse);
        }

        // ---------------------------------- Supported Domain ------------------------------------
        [Fact]
        public void SupportedDomain_ShouldBeSetToCorrectDomain()
        {
            // Act and Assert
            Assert.Equal("amazon.co.uk", amazonPriceParser.SupportedDomain);
        }

        // ------------------------------------------ ParsePriceAsync --------------------------------------
        [Fact]
        public async Task ParsePriceAsynce_WhenProductIsDiscounted_ShouldReturnBothPriceAndDiscountedPrice()
        {
            // Arrange
            await SetupPageContent();

            decimal expectedDiscountedPrice = 11.99m;
            decimal expectedPrice = 15.99m;

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(DiscountedPageContent)
            };

            // Act
            var prices = await amazonPriceParser.ParsePriceAsync(response);

            // Assert
            Assert.Equal(expectedPrice, prices.Price);
            Assert.Equal(expectedDiscountedPrice, prices.DiscountedPrice);
        }

        [Fact]
        public async Task ParsePriceAsynce_WhenProductIsNotDiscounted_ShouldReturnBothPriceAndDiscountedPriceAsTheValueOfPrice()
        {
            // Arrange
            await SetupPageContent();

            decimal expectedDiscountedPrice = 9.00m;
            decimal expectedPrice = 9.00m;

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(NonDiscountedPageContent)
            };

            // Act
            var prices = await amazonPriceParser.ParsePriceAsync(response);

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
            var prices = await amazonPriceParser.ParsePriceAsync(response);

            // Assert
            Assert.Equal(expectedPrice, prices.Price);
            Assert.Equal(expectedDiscountedPrice, prices.DiscountedPrice);
        }
    }
}