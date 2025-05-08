using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using PriceComparisonWebsite.Services.Utilities;
using PriceComparisonWebsite.Services.Utilities.Interfaces;
using PriceComparisonWebsite.Services.WebScraping.Parsers;
using PriceComparisonWebsite.Services.WebScraping.Parsers.Interfaces;

namespace PriceComparisonWebsite.Tests.Services.Webscraping.Parsers
{
    public class AmazonParserTests // Could be worth having more page options that could be parsed e.g. where layouts are different (i think i saw this for a book) and tests if that support is added
    {
        private readonly Mock<IContentCompressor> _mockCompressor;
        private readonly IPriceParser _amazonPriceParser;
        IFileSystemWrapper _fileSystem;
        readonly string folderName;
        string DiscountedPageContent; // From the page provided discounted price is £11.99 and normal price is £15.99 (was the response content from https://www.amazon.co.uk/UNO-Families-Collectible-Cards-Instructions/dp/B08WKF5HZR on 30/03/2025)
        string NonDiscountedPageContent; // From page provided price was £9.00 with no discounted price (was the response content from https://www.amazon.co.uk/Coca-Cola-Zero-7-92-Litre/dp/B007C7KGKI on 30/03/2025)

        public AmazonParserTests()
        {
            folderName = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "Services", "Webscraping", "Parsers", "TestHttpResponses");
            _fileSystem = new FileSystemWrapper();
            _mockCompressor = new Mock<IContentCompressor>();
            _amazonPriceParser = new AmazonPriceParser(_mockCompressor.Object);
        }

        [Fact]
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
            var CanParse = _amazonPriceParser.CanParse(uri);

            // Assert
            Assert.True(CanParse);
        }

        [Fact]
        public void CanParse_WhenNonAmazonUri_ShouldReturnFalse()
        {
            // Arrange
            Uri uri = new Uri("https://www.ebay.co.uk");

            // Act
            var CanParse = _amazonPriceParser.CanParse(uri);

            // Assert
            Assert.False(CanParse);
        }

        // ---------------------------------- Supported Domain ------------------------------------
        [Fact]
        public void SupportedDomain_ShouldBeSetToCorrectDomain()
        {
            // Act and Assert
            Assert.Equal("www.amazon.co.uk", _amazonPriceParser.SupportedDomain);
        }

        // ------------------------------------------ ParsePriceAsync --------------------------------------
        [Fact]
        public async Task ParsePriceAsync_WhenProductIsDiscounted_ShouldReturnBothPriceAndDiscountedPrice()
        {
            // Arrange
            await SetupPageContent();
            _mockCompressor.Setup(x => x.DecompressAsync(It.IsAny<byte[]>()))
                .ReturnsAsync(DiscountedPageContent);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(new byte[] { 1, 2, 3 }) // dummy content
            };

            // Act
            var prices = await _amazonPriceParser.ParsePriceAsync(response);

            // Assert
            Assert.Equal(15.99m, prices.Price);
            Assert.Equal(11.99m, prices.DiscountedPrice);
        }

        [Fact]
        public async Task ParsePriceAsync_WhenProductIsNotDiscounted_ShouldReturnBothPriceAndDiscountedPriceAsTheValueOfPrice()
        {
            // Arrange
            await SetupPageContent();
            _mockCompressor.Setup(x => x.DecompressAsync(It.IsAny<byte[]>()))
                .ReturnsAsync(NonDiscountedPageContent);

            decimal expectedDiscountedPrice = 9.00m;
            decimal expectedPrice = 9.00m;

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(new byte[] { 1, 2, 3 }) // dummy content
            };

            // Act
            var prices = await _amazonPriceParser.ParsePriceAsync(response);

            // Assert
            Assert.Equal(expectedPrice, prices.Price);
            Assert.Equal(expectedDiscountedPrice, prices.DiscountedPrice);
        }

        [Fact]
        public async Task ParsePriceAsync_WhenNonValidPageLayout_ShouldReturnBothPriceAndDiscountedPriceAsZero()
        {
            // Arrange
            _mockCompressor.Setup(x => x.DecompressAsync(It.IsAny<byte[]>()))
                .ReturnsAsync("NotValidContent");

            decimal expectedDiscountedPrice = 0;
            decimal expectedPrice = 0;

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(new byte[] { 1, 2, 3 }) // dummy content
            };

            // Act
            var prices = await _amazonPriceParser.ParsePriceAsync(response);

            // Assert
            Assert.Equal(expectedPrice, prices.Price);
            Assert.Equal(expectedDiscountedPrice, prices.DiscountedPrice);
        }
    }
}