using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Price_Comparison_Website.Services.WebScraping;
using Price_Comparison_Website.Services.WebScraping.Parsers.Interfaces;

namespace Price_Comparison_Website.Tests.Services.Webscraping
{
    public class PriceParserFactoryTests
    {
        private readonly Mock<IPriceParser> _amazonParserMock;
        private readonly Mock<IPriceParser> _ebayParserMock;
        private readonly PriceParserFactory _factory;

        public PriceParserFactoryTests()
        {
            _amazonParserMock = new Mock<IPriceParser>();
            _amazonParserMock.Setup(p => p.SupportedDomain).Returns("amazon.co.uk");

            _ebayParserMock = new Mock<IPriceParser>();
            _ebayParserMock.Setup(p => p.SupportedDomain).Returns("ebay.com");

            var parsers = new List<IPriceParser> { _amazonParserMock.Object, _ebayParserMock.Object };

            _factory = new PriceParserFactory(parsers);
        }

        // --------------------------------------------------------------------------- GetAllSuportedParsers -----------------------------------------------------

        [Fact]
        public void GetAllSuportedParsers_WhenParsersExist_ShouldReturnAllSupportedParsers()
        {
            // Act
            var supportedParsers = _factory.GetAllSupportedParsers().ToList();

            // Assert
            Assert.Equal(2, supportedParsers.Count);
            Assert.Contains("amazon.co.uk", supportedParsers);
            Assert.Contains("ebay.com", supportedParsers);
        }

        // ---------------------------------------------------------- GetParserForDomain-------------------------------------------------------------
        [Fact]
        public void GetParserForDomain_WhenDomainExists_ShouldReturnCorrectParser()
        {
            // Act
            var amazonParser = _factory.GetParserForDomain("amazon.co.uk");
            var ebayParser = _factory.GetParserForDomain("ebay.com");

            // Assert
            Assert.NotNull(amazonParser);
            Assert.NotNull(ebayParser);

            Assert.Equal("amazon.co.uk", amazonParser.SupportedDomain);
            Assert.Equal("ebay.com", ebayParser.SupportedDomain);
        }

        [Fact]
        public void GetParserForDomain_ShouldReturnNull_WhenDomainDoesNotExist()
        {
            // Act
            var parser = _factory.GetParserForDomain("invalid.com");

            // Assert
            Assert.Null(parser);
        }

        // --------------------------------------------------------------------------- HasParserForDomain ---------------------------------------------------------

        [Fact]
        public void HasParserForDomain_ShouldReturnTrue_WhenDomainExists()
        {
            // Act & Assert
            Assert.True(_factory.HasParserForDomain("amazon.co.uk"));
            Assert.True(_factory.HasParserForDomain("ebay.com"));
        }

        [Fact]
        public void HasParserForDomain_WhenDomainDoesNotExist_ShouldReturnFalse()
        {
            // Act & Assert
            Assert.False(_factory.HasParserForDomain("invalid.com"));
        }

    }
}