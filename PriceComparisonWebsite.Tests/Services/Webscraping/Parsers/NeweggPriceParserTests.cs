using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Moq;
using PriceComparisonWebsite.Services.Utilities;
using PriceComparisonWebsite.Services.Utilities.Interfaces;
using PriceComparisonWebsite.Services.WebScraping.Parsers;
using PriceComparisonWebsite.Services.WebScraping.Parsers.Interfaces;

namespace PriceComparisonWebsite.Tests.Services.Webscraping.Parsers
{
    public class NeweggPriceParserTests
    {
        private readonly Mock<IContentCompressor> _mockCompressor;
        private readonly IPriceParser _neweggPriceParser;
        private readonly IFileSystemWrapper _fileSystem;
        private readonly string _folderName;
        private string DiscountedPageContent;
        private string NonDiscountedPageContent;

        public NeweggPriceParserTests()
        {
            _folderName = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, 
                "Services", "Webscraping", "Parsers", "TestHttpResponses");
            _fileSystem = new FileSystemWrapper();
            _mockCompressor = new Mock<IContentCompressor>();
            _neweggPriceParser = new NeweggPriceParser(_mockCompressor.Object);
        }

        [Fact]
        public async Task SetupPageContent()
        {
            var DiscountedFilePath = Path.Combine(_folderName, "NeweggDiscountedPageContent.txt");
            var NonDiscountedFilePath = Path.Combine(_folderName, "NeweggNonDiscountedPageContent.txt");

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
            var CanParse = _neweggPriceParser.CanParse(uri);

            // Assert
            Assert.True(CanParse);
        }

        [Fact]
        public void CanParse_WhenNonAmazonUri_ShouldReturnFalse()
        {
            // Arrange
            Uri uri = new Uri("https://www.ebay.co.uk");

            // Act
            var CanParse = _neweggPriceParser.CanParse(uri);

            // Assert
            Assert.False(CanParse);
        }

        // ---------------------------------- Supported Domain ------------------------------------
        [Fact]
        public void SupportedDomain_ShouldBeSetToCorrectDomain()
        {
            // Act and Assert
            Assert.Equal("www.newegg.com", _neweggPriceParser.SupportedDomain);
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
            var prices = await _neweggPriceParser.ParsePriceAsync(response);

            // Assert
            Assert.Equal(327.59m, prices.Price);
            Assert.Equal(319.19m, prices.DiscountedPrice);
        }

        [Fact]
        public async Task ParsePriceAsync_WhenProductIsNotDiscounted_ShouldReturnBothPriceAndDiscountedPriceAsTheValueOfPrice()
        {
            // Arrange
            await SetupPageContent();
            _mockCompressor.Setup(x => x.DecompressAsync(It.IsAny<byte[]>()))
                .ReturnsAsync(NonDiscountedPageContent);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(new byte[] { 1, 2, 3 }) // dummy content
            };

            // Act
            var prices = await _neweggPriceParser.ParsePriceAsync(response);

            // Assert
            Assert.Equal(827.99m, prices.Price);
            Assert.Equal(827.99m, prices.DiscountedPrice);
        }

        [Fact]
        public async Task ParsePriceAsync_WhenNonValidPageLayout_ShouldReturnBothPriceAndDiscountedPriceAsZero()
        {
            // Arrange
            _mockCompressor.Setup(x => x.DecompressAsync(It.IsAny<byte[]>()))
                .ReturnsAsync("NotValidContent");

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(new byte[] { 1, 2, 3 }) // dummy content
            };

            // Act
            var prices = await _neweggPriceParser.ParsePriceAsync(response);

            // Assert
            Assert.Equal(0m, prices.Price);
            Assert.Equal(0m, prices.DiscountedPrice);
        }
    }
}
