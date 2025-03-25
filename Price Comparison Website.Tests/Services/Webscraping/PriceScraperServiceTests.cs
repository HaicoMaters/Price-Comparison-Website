using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Price_Comparison_Website.Services.HttpClients;
using Price_Comparison_Website.Services.Utilities;
using Price_Comparison_Website.Services.Utilities.Interfaces;
using Price_Comparison_Website.Services.WebScraping;
using Price_Comparison_Website.Services.WebScraping.Interfaces;

namespace Price_Comparison_Website.Tests.Services.Webscraping
{
    public class PriceScraperServiceTests
    {
        
        private readonly Mock<IRobotsTxtChecker> _robotsTxtCheckerMock;
        private readonly Mock<IVendorService> _vendorServiceMock;
        private readonly Mock<IPriceListingService> _priceListingServiceMock;
        private readonly Mock<IPriceParserFactory> _priceParserFactoryMock;
        private readonly Mock<ILogger<PriceScraperService>> _loggerMock;
        private readonly Mock<IRateLimiter> _rateLimiterMock;
        private readonly Mock<IScraperHttpClient> _scraperHttpClientMock;
        private readonly IPriceScraperService _priceScraperService;

        public PriceScraperServiceTests()
        {
            _rateLimiterMock = new Mock<IRateLimiter>();
            _vendorServiceMock = new Mock<IVendorService>();
            _robotsTxtCheckerMock = new Mock<IRobotsTxtChecker>();
            _priceListingServiceMock = new Mock<IPriceListingService>();
            _loggerMock = new Mock<ILogger<PriceScraperService>>();
            _scraperHttpClientMock = new Mock<IScraperHttpClient>();
            _priceParserFactoryMock = new Mock<IPriceParserFactory>();

            _priceScraperService = new PriceScraperService(
                _robotsTxtCheckerMock.Object,
                _vendorServiceMock.Object,
                _priceListingServiceMock.Object,
                _loggerMock.Object,
                _rateLimiterMock.Object,
                _scraperHttpClientMock.Object,
                _priceParserFactoryMock.Object
            );
        }

        // ---------------------------------------------------------------- GetVendorIdsThatSupportScraping ------------------------------------------------

        // ---------------------------------------------------------------- UpdateAllListings --------------------------------------------------------------

        // ---------------------------------------------------------------- FilterUsingRobotsTxt (proabaly not neededing testing) -------------------------------------

        // ---------------------------------------------------------------- UpdateListing (not implemented yet don't even know if want to keep function) ------------------------------------------------
    }
}