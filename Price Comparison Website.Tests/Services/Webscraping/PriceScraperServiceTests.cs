using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Price_Comparison_Website.Services.HttpClients;
using Price_Comparison_Website.Services.Utilities;
using Price_Comparison_Website.Services.Utilities.Interfaces;
using Price_Comparison_Website.Services.WebScraping;
using Price_Comparison_Website.Services.WebScraping.Interfaces;
using Price_Comparison_Website.Services.WebScraping.Parsers.Interfaces;

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

        [Fact]
        public async Task GetVendorIdsThatSupportScraping_WhenValidUrisAndAllHasParser_ShouldReturnAllVendorIds()
        {
            // Arrange
            var vendors = new List<Vendor>{
                new Vendor {VendorId = 1, VendorUrl = "https://example.com"},
                new Vendor {VendorId = 6, VendorUrl = "https://example2.com"},
                new Vendor {VendorId = 14, VendorUrl = "https://example3.com"}
            };

            _vendorServiceMock.Setup(v => v.GetAllVendorsAsync(It.IsAny<QueryOptions<Vendor>>()))
                .ReturnsAsync(vendors);

            _priceParserFactoryMock.Setup(p => p.HasParserForDomain(It.IsAny<string>()))
                .Returns(true);

            // Act
            var vendorIds = await _priceScraperService.GetVendorIdsThatSupportScraping();

            // Assert
            int vendorCount = vendors.Count;
            Assert.Equal(vendorCount, vendorIds.Count);

            for (int i = 0; i < vendorCount; i++)
            {
                Assert.Equal(vendors[i].VendorId, vendorIds[i]);
            }

            _vendorServiceMock.Verify(v => v.UpdateVendorAsync(It.IsAny<Vendor>()), Times.Never);
        }

        [Fact]
        public async Task GetVendorIdsThatSupportScraping_WhenValidUrisAndNotAllHasParser_ShouldReturnAllVendorIdsThatHaveParsersAndUpdateSupportsAutomaticUpdatesFlagOnOnesThatDoNot()
        {
            // Arrange
            var vendors = new List<Vendor>{
                new Vendor {VendorId = 1, VendorUrl = "https://example.com"},
                new Vendor {VendorId = 6, VendorUrl = "https://example2.com", SupportsAutomaticUpdates = true},
                new Vendor {VendorId = 14, VendorUrl = "https://example3.com"}
            };

            _vendorServiceMock.Setup(v => v.GetAllVendorsAsync(It.IsAny<QueryOptions<Vendor>>()))
                .ReturnsAsync(vendors);

            _priceParserFactoryMock.Setup(p => p.HasParserForDomain("example.com"))
                .Returns(true);

            _priceParserFactoryMock.Setup(p => p.HasParserForDomain("example2.com"))
                .Returns(false);

            _priceParserFactoryMock.Setup(p => p.HasParserForDomain("example3.com"))
                .Returns(true);

            Vendor capturedVendor = null;
            _vendorServiceMock.Setup(vs => vs.UpdateVendorAsync(It.IsAny<Vendor>()))
                .Callback<Vendor>(v => capturedVendor = v);

            // Act
            var vendorIds = await _priceScraperService.GetVendorIdsThatSupportScraping();

            // Assert
            Assert.Equal(2, vendorIds.Count);
            Assert.Equal(1, vendorIds[0]);
            Assert.Equal(14, vendorIds[1]);
            Assert.Equal(6, capturedVendor.VendorId);
            Assert.False(capturedVendor.SupportsAutomaticUpdates);

            _vendorServiceMock.Verify(v => v.UpdateVendorAsync(It.IsAny<Vendor>()), Times.Once);
        }

        [Fact]
        public async Task GetVendorIdsThatSupportScraping_WhenSomeInvalidUrisAndAllHasParser_ShouldLogInvalidUrisWarningAndAddAllValidUris()
        {
            // Arrange
            var vendors = new List<Vendor>{
                new Vendor {VendorId = 1, VendorUrl = "https://example.com"},
                new Vendor {VendorId = 6, VendorUrl = "badUrl"},
                new Vendor {VendorId = 14, VendorUrl = "https://example3.com"}
            };

            _vendorServiceMock.Setup(v => v.GetAllVendorsAsync(It.IsAny<QueryOptions<Vendor>>()))
                .ReturnsAsync(vendors);

            _priceParserFactoryMock.Setup(p => p.HasParserForDomain(It.IsAny<string>()))
                .Returns(true);

            // Act
            var vendorIds = await _priceScraperService.GetVendorIdsThatSupportScraping();

            // Assert
            Assert.Equal(2, vendorIds.Count);
            Assert.Equal(1, vendorIds[0]);
            Assert.Equal(14, vendorIds[1]);

            _loggerMock.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Invalid URL format for vendor: {vendors[1].VendorUrl}")),
                It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once
            );
        }

        // ---------------------------------------------------------------- FilterUsingRobotsTxt ---------------------------------------------

        [Fact]
        public async Task FilterUsingRobotsTxt_WhenAllAreAllowed_ShouldNotRemoveAnyUris()
        {
            // Arrange
            var uris = new Dictionary<Uri, int>
            {
                { new Uri("https://example.com/page1"), 1 },
                { new Uri("https://example.com/page2"), 2 }
            };

            _robotsTxtCheckerMock.Setup(c => c.CheckRobotsTxt(It.IsAny<Uri>()))
                .ReturnsAsync(true);

            // Act
            await _priceScraperService.FilterUsingRobotsTxt(uris);

            // Assert
            Assert.Equal(2, uris.Count);
        }

        [Fact]
        public async Task FilterUsingRobotsTxt_WhenDisallowedUris_ShouldRemoveDisallowedUris()
        {
            // Arrange
            var uris = new Dictionary<Uri, int>
        {
            { new Uri("https://example.com/allowed"), 1 },
            { new Uri("https://example.com/disallowed"), 2 }
        };

            _robotsTxtCheckerMock.Setup(c => c.CheckRobotsTxt(new Uri("https://example.com/allowed")))
                .ReturnsAsync(true);

            _robotsTxtCheckerMock.Setup(c => c.CheckRobotsTxt(new Uri("https://example.com/disallowed")))
                .ReturnsAsync(false);


            // Act
            await _priceScraperService.FilterUsingRobotsTxt(uris);

            // Assert
            Assert.Single(uris);
            Assert.True(uris.ContainsKey(new Uri("https://example.com/allowed")));
        }


        // ---------------------------------------------------------------- UpdateAllListings --------------------------------------------------------------
        [Fact]
        public async Task UpdateAllListings_WhenAllValidUrisAndAllHasParser_ShouldSendAndExecuteAllTasksSucessfully() // Very complicated lots of steps just copy and change the specific methods for each check
        {
            // Arrange 
            var vendors = new List<Vendor>{
                new Vendor {VendorId = 1, VendorUrl = "https://example.com"},
                new Vendor {VendorId = 6, VendorUrl = "https://example2.com"},
            };

            var listingsVend1 = new List<PriceListing>
            {
                new PriceListing{PriceListingId = 1, VendorId = 1, Price = 2.00m, DiscountedPrice = 1.50m, PurchaseUrl = "https://example.com/1", ProductId = 1}, // Each has different product id for explicit verification
                new PriceListing{PriceListingId = 2, VendorId = 1, Price = 3.00m, DiscountedPrice = 3.00m, PurchaseUrl = "https://example.com/2", ProductId = 2}
            };

            var listingsVend6 = new List<PriceListing>
            {
                new PriceListing{PriceListingId = 3, VendorId = 6, Price = 2.00m, DiscountedPrice = 1.50m, PurchaseUrl = "https://example2.com/1", ProductId = 3},
                new PriceListing{PriceListingId = 4, VendorId = 6, Price = 2.00m, DiscountedPrice = 2.00m, PurchaseUrl = "https://example2.com/2", ProductId = 4}
            };

            // Pass the other two functions fine
            _vendorServiceMock.Setup(v => v.GetAllVendorsAsync(It.IsAny<QueryOptions<Vendor>>()))
                .ReturnsAsync(vendors);

            _robotsTxtCheckerMock.Setup(c => c.CheckRobotsTxt(It.IsAny<Uri>()))
                .ReturnsAsync(true);

            _priceParserFactoryMock.Setup(p => p.HasParserForDomain(It.IsAny<string>()))
                .Returns(true);

            // Main method setups 

            _priceListingServiceMock.Setup(s => s.GetPriceListingsByVendorId(1, It.IsAny<QueryOptions<PriceListing>>()))
                .ReturnsAsync(listingsVend1);

            _priceListingServiceMock.Setup(s => s.GetPriceListingsByVendorId(6, It.IsAny<QueryOptions<PriceListing>>()))
                .ReturnsAsync(listingsVend6);


            // Mock GetPriceListingById for each listing
            foreach (var listing in listingsVend1.Concat(listingsVend6))
            {
                _priceListingServiceMock.Setup(p => p.GetPriceListingById(listing.PriceListingId, It.IsAny<QueryOptions<PriceListing>>()))
                    .ReturnsAsync(listing);
            }

            // Mock Price Parser    
            var mockParser = new Mock<IPriceParser>();

            mockParser.Setup(p => p.ParsePriceAsync(It.IsAny<Uri>()))
                .ReturnsAsync((5.00m, 4.50m));

            _priceParserFactoryMock.Setup(p => p.GetParserForDomain(It.IsAny<string>()))
                .Returns(mockParser.Object);

            // Mock HTTP client responses
            _scraperHttpClientMock.Setup(c => c.SendRequestAsync(It.IsAny<Uri>(), HttpMethod.Get))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("<html>Valid Content</html>")
                });

            // Mock RateLimiter behavior (just execute the action immediately)
            _rateLimiterMock.Setup(r => r.EnqueueRequest(It.IsAny<Func<Task>>(), It.IsAny<string>()))
                .Returns<Func<Task>, string>((action, domain) => action());

            // Act
            await _priceScraperService.UpdateAllListings();

            // Assert
            _vendorServiceMock.Verify(v => v.UpdateVendorAsync(It.IsAny<Vendor>()), Times.Never);

            _priceListingServiceMock.Verify(p => p.GetPriceListingsByVendorId(1, It.IsAny<QueryOptions<PriceListing>>()), Times.Once);
            _priceListingServiceMock.Verify(p => p.GetPriceListingsByVendorId(6, It.IsAny<QueryOptions<PriceListing>>()), Times.Once);

            _robotsTxtCheckerMock.Verify(r => r.CheckRobotsTxt(It.IsAny<Uri>()), Times.Exactly(4));

            // These should show the ScrapeAndUpdate() task succeeding for each listing
            _scraperHttpClientMock.Verify(c => c.SendRequestAsync(It.IsAny<Uri>(), HttpMethod.Get), Times.Exactly(4));
            _priceParserFactoryMock.Verify(p => p.GetParserForDomain(It.IsAny<string>()), Times.Exactly(4));
            mockParser.Verify(p => p.ParsePriceAsync(It.IsAny<Uri>()), Times.Exactly(4));

            foreach (var listing in listingsVend1.Concat(listingsVend6))
            {
                _priceListingServiceMock.Verify(p => p.UpdatePriceListing(It.Is<PriceListing>(l =>
                    l.Price == 5.00m &&
                    l.DiscountedPrice == 4.50m &&
                    l.PriceListingId == listing.PriceListingId)), Times.Once);

                _priceListingServiceMock.Verify(p => p.UpdateCheapestPrice(listing.ProductId, 4.50m), Times.Once);
            }

            _loggerMock.Verify(l => l.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task UpdateAllListings_WhenAllUrisFilteredByRobotsTxt_ShouldNotExecuteAnyTasks()
        {
            // Arrange 
            var vendors = new List<Vendor>{
                new Vendor {VendorId = 1, VendorUrl = "https://example.com"},
                new Vendor {VendorId = 6, VendorUrl = "https://example2.com"},
            };

            var listingsVend1 = new List<PriceListing>
            {
                new PriceListing{PriceListingId = 1, VendorId = 1, Price = 2.00m, DiscountedPrice = 1.50m, PurchaseUrl = "https://example.com/1", ProductId = 1},
                new PriceListing{PriceListingId = 2, VendorId = 1, Price = 3.00m, DiscountedPrice = 3.00m, PurchaseUrl = "https://example.com/2", ProductId = 2}
            };

            var listingsVend6 = new List<PriceListing>
            {
                new PriceListing{PriceListingId = 3, VendorId = 6, Price = 2.00m, DiscountedPrice = 1.50m, PurchaseUrl = "https://example2.com/1", ProductId = 3},
                new PriceListing{PriceListingId = 4, VendorId = 6, Price = 2.00m, DiscountedPrice = 2.00m, PurchaseUrl = "https://example2.com/2", ProductId = 4}
            };

            // Pass the other two functions fine
            _vendorServiceMock.Setup(v => v.GetAllVendorsAsync(It.IsAny<QueryOptions<Vendor>>()))
                .ReturnsAsync(vendors);

            _robotsTxtCheckerMock.Setup(c => c.CheckRobotsTxt(It.IsAny<Uri>()))
                .ReturnsAsync(false);

            _priceParserFactoryMock.Setup(p => p.HasParserForDomain(It.IsAny<string>()))
                .Returns(true);

            // Main method setups 

            _priceListingServiceMock.Setup(s => s.GetPriceListingsByVendorId(1, It.IsAny<QueryOptions<PriceListing>>()))
                .ReturnsAsync(listingsVend1);

            _priceListingServiceMock.Setup(s => s.GetPriceListingsByVendorId(6, It.IsAny<QueryOptions<PriceListing>>()))
                .ReturnsAsync(listingsVend6);


            // Mock GetPriceListingById for each listing
            foreach (var listing in listingsVend1.Concat(listingsVend6))
            {
                _priceListingServiceMock.Setup(p => p.GetPriceListingById(listing.PriceListingId, It.IsAny<QueryOptions<PriceListing>>()))
                    .ReturnsAsync(listing);
            }

            // Mock Price Parser    
            var mockParser = new Mock<IPriceParser>();

            mockParser.Setup(p => p.ParsePriceAsync(It.IsAny<Uri>()))
                .ReturnsAsync((5.00m, 4.50m));

            _priceParserFactoryMock.Setup(p => p.GetParserForDomain(It.IsAny<string>()))
                .Returns(mockParser.Object);

            // Mock HTTP client responses
            _scraperHttpClientMock.Setup(c => c.SendRequestAsync(It.IsAny<Uri>(), HttpMethod.Get))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("<html>Valid Content</html>")
                });

            // Mock RateLimiter behavior (just execute the action immediately)
            _rateLimiterMock.Setup(r => r.EnqueueRequest(It.IsAny<Func<Task>>(), It.IsAny<string>()))
                .Returns<Func<Task>, string>((action, domain) => action());

            // Act
            await _priceScraperService.UpdateAllListings();

            // Assert
            _vendorServiceMock.Verify(v => v.UpdateVendorAsync(It.IsAny<Vendor>()), Times.Never);

            _priceListingServiceMock.Verify(p => p.GetPriceListingsByVendorId(1, It.IsAny<QueryOptions<PriceListing>>()), Times.Once);
            _priceListingServiceMock.Verify(p => p.GetPriceListingsByVendorId(6, It.IsAny<QueryOptions<PriceListing>>()), Times.Once);

            _robotsTxtCheckerMock.Verify(r => r.CheckRobotsTxt(It.IsAny<Uri>()), Times.Exactly(4));

            // These should show the No tasks should be created 
            _scraperHttpClientMock.Verify(c => c.SendRequestAsync(It.IsAny<Uri>(), HttpMethod.Get), Times.Never);
            _priceParserFactoryMock.Verify(p => p.GetParserForDomain(It.IsAny<string>()), Times.Never);
            mockParser.Verify(p => p.ParsePriceAsync(It.IsAny<Uri>()), Times.Never);


            _priceListingServiceMock.Verify(p => p.UpdatePriceListing(It.IsAny<PriceListing>()), Times.Never);

            _priceListingServiceMock.Verify(p => p.UpdateCheapestPrice(It.IsAny<int>(), It.IsAny<decimal>()), Times.Never);

            _loggerMock.Verify(l => l.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task UpdateAllListings_WhenRateLimiterIsUsed_ShouldThrottleRequestsCorrectlyPerDomain()
        {
            // Arrange 
            var vendors = new List<Vendor>{
                new Vendor {VendorId = 1, VendorUrl = "https://example.com"},
                new Vendor {VendorId = 6, VendorUrl = "https://example2.com"},
            };

            var listingsVend1 = new List<PriceListing>
            {
                new PriceListing{PriceListingId = 1, VendorId = 1, Price = 2.00m, DiscountedPrice = 1.50m, PurchaseUrl = "https://example.com/1", ProductId = 1}, // Each has different product id for explicit verification
                new PriceListing{PriceListingId = 2, VendorId = 1, Price = 3.00m, DiscountedPrice = 3.00m, PurchaseUrl = "https://example.com/2", ProductId = 2}
            };

            var listingsVend6 = new List<PriceListing>
            {
                new PriceListing{PriceListingId = 3, VendorId = 6, Price = 2.00m, DiscountedPrice = 1.50m, PurchaseUrl = "https://example2.com/1", ProductId = 3},
                new PriceListing{PriceListingId = 4, VendorId = 6, Price = 2.00m, DiscountedPrice = 2.00m, PurchaseUrl = "https://example2.com/2", ProductId = 4}
            };

            // Pass the other two functions fine
            _vendorServiceMock.Setup(v => v.GetAllVendorsAsync(It.IsAny<QueryOptions<Vendor>>()))
                .ReturnsAsync(vendors);

            _robotsTxtCheckerMock.Setup(c => c.CheckRobotsTxt(It.IsAny<Uri>()))
                .ReturnsAsync(true);

            _priceParserFactoryMock.Setup(p => p.HasParserForDomain(It.IsAny<string>()))
                .Returns(true);

            // Main method setups 

            _priceListingServiceMock.Setup(s => s.GetPriceListingsByVendorId(1, It.IsAny<QueryOptions<PriceListing>>()))
                .ReturnsAsync(listingsVend1);

            _priceListingServiceMock.Setup(s => s.GetPriceListingsByVendorId(6, It.IsAny<QueryOptions<PriceListing>>()))
                .ReturnsAsync(listingsVend6);


            // Mock GetPriceListingById for each listing
            foreach (var listing in listingsVend1.Concat(listingsVend6))
            {
                _priceListingServiceMock.Setup(p => p.GetPriceListingById(listing.PriceListingId, It.IsAny<QueryOptions<PriceListing>>()))
                    .ReturnsAsync(listing);
            }

            // Mock Price Parser    
            var mockParser = new Mock<IPriceParser>();

            mockParser.Setup(p => p.ParsePriceAsync(It.IsAny<Uri>()))
                .ReturnsAsync((5.00m, 4.50m));

            _priceParserFactoryMock.Setup(p => p.GetParserForDomain(It.IsAny<string>()))
                .Returns(mockParser.Object);

            // Mock HTTP client responses
            _scraperHttpClientMock.Setup(c => c.SendRequestAsync(It.IsAny<Uri>(), HttpMethod.Get))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("<html>Valid Content</html>")
                });

            // Simulate throttled execution based on domain with a delay using rate limiter mock
            _rateLimiterMock.Setup(r => r.EnqueueRequest(It.IsAny<Func<Task>>(), It.IsAny<string>()))
                .Returns<Func<Task>, string>(async (action, domain) =>
                {
                    await Task.Delay(100); // Simulate slight cooldown
                    await action();
                });


            // Act
            await _priceScraperService.UpdateAllListings();

            // Assert
            _vendorServiceMock.Verify(v => v.UpdateVendorAsync(It.IsAny<Vendor>()), Times.Never);

            _priceListingServiceMock.Verify(p => p.GetPriceListingsByVendorId(1, It.IsAny<QueryOptions<PriceListing>>()), Times.Once);
            _priceListingServiceMock.Verify(p => p.GetPriceListingsByVendorId(6, It.IsAny<QueryOptions<PriceListing>>()), Times.Once);

            _robotsTxtCheckerMock.Verify(r => r.CheckRobotsTxt(It.IsAny<Uri>()), Times.Exactly(4));

            // These should show the ScrapeAndUpdate() task succeeding for each listing
            _scraperHttpClientMock.Verify(c => c.SendRequestAsync(It.IsAny<Uri>(), HttpMethod.Get), Times.Exactly(4));
            _priceParserFactoryMock.Verify(p => p.GetParserForDomain(It.IsAny<string>()), Times.Exactly(4));
            mockParser.Verify(p => p.ParsePriceAsync(It.IsAny<Uri>()), Times.Exactly(4));

            foreach (var listing in listingsVend1.Concat(listingsVend6))
            {
                _priceListingServiceMock.Verify(p => p.UpdatePriceListing(It.Is<PriceListing>(l =>
                    l.Price == 5.00m &&
                    l.DiscountedPrice == 4.50m &&
                    l.PriceListingId == listing.PriceListingId)), Times.Once);

                _priceListingServiceMock.Verify(p => p.UpdateCheapestPrice(listing.ProductId, 4.50m), Times.Once);
            }

            _loggerMock.Verify(l => l.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.AtLeastOnce);

            // Verify the rate limiting behavior
            _rateLimiterMock.Verify(r => r.EnqueueRequest(It.IsAny<Func<Task>>(), "example.com"), Times.Exactly(2));
            _rateLimiterMock.Verify(r => r.EnqueueRequest(It.IsAny<Func<Task>>(), "example2.com"), Times.Exactly(2));
        }

        [Fact]
        public async Task UpdateAllListings_WhenParserForDomainIsNull_ShouldLogWarningAndSkip()
        {
            // Arrange 
            var vendors = new List<Vendor>{
                new Vendor {VendorId = 1, VendorUrl = "https://example.com"},
                new Vendor {VendorId = 6, VendorUrl = "https://example2.com"},
            };

            var listingsVend1 = new List<PriceListing>
            {
                new PriceListing{PriceListingId = 1, VendorId = 1, Price = 2.00m, DiscountedPrice = 1.50m, PurchaseUrl = "https://example.com/1", ProductId = 1}, // Each has different product id for explicit verification
                new PriceListing{PriceListingId = 2, VendorId = 1, Price = 3.00m, DiscountedPrice = 3.00m, PurchaseUrl = "https://example.com/2", ProductId = 2}
            };

            var listingsVend6 = new List<PriceListing>
            {
                new PriceListing{PriceListingId = 3, VendorId = 6, Price = 2.00m, DiscountedPrice = 1.50m, PurchaseUrl = "https://example2.com/1", ProductId = 3},
                new PriceListing{PriceListingId = 4, VendorId = 6, Price = 2.00m, DiscountedPrice = 2.00m, PurchaseUrl = "https://example2.com/2", ProductId = 4}
            };

            // Pass the other two functions fine
            _vendorServiceMock.Setup(v => v.GetAllVendorsAsync(It.IsAny<QueryOptions<Vendor>>()))
                .ReturnsAsync(vendors);

            _robotsTxtCheckerMock.Setup(c => c.CheckRobotsTxt(It.IsAny<Uri>()))
                .ReturnsAsync(true);

            _priceParserFactoryMock.Setup(p => p.HasParserForDomain(It.IsAny<string>()))
                .Returns(true);

            // Main method setups 

            _priceListingServiceMock.Setup(s => s.GetPriceListingsByVendorId(1, It.IsAny<QueryOptions<PriceListing>>()))
                .ReturnsAsync(listingsVend1);

            _priceListingServiceMock.Setup(s => s.GetPriceListingsByVendorId(6, It.IsAny<QueryOptions<PriceListing>>()))
                .ReturnsAsync(listingsVend6);


            // Mock GetPriceListingById for each listing
            foreach (var listing in listingsVend1.Concat(listingsVend6))
            {
                _priceListingServiceMock.Setup(p => p.GetPriceListingById(listing.PriceListingId, It.IsAny<QueryOptions<PriceListing>>()))
                    .ReturnsAsync(listing);
            }

            // Mock Price Parser    
            var mockParser = new Mock<IPriceParser>();

            mockParser.Setup(p => p.ParsePriceAsync(It.IsAny<Uri>()))
                .ReturnsAsync((5.00m, 4.50m));

            _priceParserFactoryMock.Setup(p => p.GetParserForDomain("example.com")) // THIS IS THE MAIN THING DIFFERENT THAT IS BEING TESTED
                .Returns(mockParser.Object);                                         // < ----------------------------------------------------------------------------------------------------------------------------

            _priceParserFactoryMock.Setup(p => p.GetParserForDomain("example2.com")) // THIS LINE TOO
                .Returns((IPriceParser)null);

            // Mock HTTP client responses
            _scraperHttpClientMock.Setup(c => c.SendRequestAsync(It.IsAny<Uri>(), HttpMethod.Get))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("<html>Valid Content</html>")
                });

            // Mock RateLimiter behavior (just execute the action immediately)
            _rateLimiterMock.Setup(r => r.EnqueueRequest(It.IsAny<Func<Task>>(), It.IsAny<string>()))
                .Returns<Func<Task>, string>((action, domain) => action());

            // Act
            await _priceScraperService.UpdateAllListings();

            // Assert
            _vendorServiceMock.Verify(v => v.UpdateVendorAsync(It.IsAny<Vendor>()), Times.Never);

            _priceListingServiceMock.Verify(p => p.GetPriceListingsByVendorId(1, It.IsAny<QueryOptions<PriceListing>>()), Times.Once);
            _priceListingServiceMock.Verify(p => p.GetPriceListingsByVendorId(6, It.IsAny<QueryOptions<PriceListing>>()), Times.Once);

            _robotsTxtCheckerMock.Verify(r => r.CheckRobotsTxt(It.IsAny<Uri>()), Times.Exactly(4));

            // These should show the ScrapeAndUpdate() task succeeding for each listing
            _scraperHttpClientMock.Verify(c => c.SendRequestAsync(It.IsAny<Uri>(), HttpMethod.Get), Times.Exactly(4));
            _priceParserFactoryMock.Verify(p => p.GetParserForDomain(It.IsAny<string>()), Times.Exactly(4));
            mockParser.Verify(p => p.ParsePriceAsync(It.IsAny<Uri>()), Times.Exactly(2));

            foreach (var listing in listingsVend1)
            {
                _priceListingServiceMock.Verify(p => p.UpdatePriceListing(It.Is<PriceListing>(l =>
                    l.Price == 5.00m &&
                    l.DiscountedPrice == 4.50m &&
                    l.PriceListingId == listing.PriceListingId)), Times.Once);

                _priceListingServiceMock.Verify(p => p.UpdateCheapestPrice(listing.ProductId, 4.50m), Times.Once);
            }

            _loggerMock.Verify(l => l.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.AtLeastOnce);

            _loggerMock.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"No parser found for ")),
                It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Exactly(2)
            );
        }

        [Fact]
        public async Task UpdateAllListings_WhenRobotsTxtThrowsException_ShouldLogErrorAndSkipFiltering()
        {
            // Arrange 
            var vendors = new List<Vendor>{
                new Vendor {VendorId = 1, VendorUrl = "https://example.com"},
                new Vendor {VendorId = 6, VendorUrl = "https://example2.com"},
            };

            var listingsVend1 = new List<PriceListing>
            {
                new PriceListing{PriceListingId = 1, VendorId = 1, Price = 2.00m, DiscountedPrice = 1.50m, PurchaseUrl = "https://example.com/1", ProductId = 1}, // Each has different product id for explicit verification
                new PriceListing{PriceListingId = 2, VendorId = 1, Price = 3.00m, DiscountedPrice = 3.00m, PurchaseUrl = "https://example.com/2", ProductId = 2}
            };

            var listingsVend6 = new List<PriceListing>
            {
                new PriceListing{PriceListingId = 3, VendorId = 6, Price = 2.00m, DiscountedPrice = 1.50m, PurchaseUrl = "https://example2.com/1", ProductId = 3},
                new PriceListing{PriceListingId = 4, VendorId = 6, Price = 2.00m, DiscountedPrice = 2.00m, PurchaseUrl = "https://example2.com/2", ProductId = 4}
            };

            // Pass the other two functions fine
            _vendorServiceMock.Setup(v => v.GetAllVendorsAsync(It.IsAny<QueryOptions<Vendor>>()))
                .ReturnsAsync(vendors);

            _robotsTxtCheckerMock.Setup(c => c.CheckRobotsTxt(It.IsAny<Uri>()))
                .ThrowsAsync(new Exception("Test Exception"));

            _priceParserFactoryMock.Setup(p => p.HasParserForDomain(It.IsAny<string>()))
                .Returns(true);

            // Main method setups 

            _priceListingServiceMock.Setup(s => s.GetPriceListingsByVendorId(1, It.IsAny<QueryOptions<PriceListing>>()))
                .ReturnsAsync(listingsVend1);

            _priceListingServiceMock.Setup(s => s.GetPriceListingsByVendorId(6, It.IsAny<QueryOptions<PriceListing>>()))
                .ReturnsAsync(listingsVend6);


            // Mock GetPriceListingById for each listing
            foreach (var listing in listingsVend1.Concat(listingsVend6))
            {
                _priceListingServiceMock.Setup(p => p.GetPriceListingById(listing.PriceListingId, It.IsAny<QueryOptions<PriceListing>>()))
                    .ReturnsAsync(listing);
            }


            // Act
            await _priceScraperService.UpdateAllListings();

            // Assert
            _vendorServiceMock.Verify(v => v.UpdateVendorAsync(It.IsAny<Vendor>()), Times.Never);

            _priceListingServiceMock.Verify(p => p.GetPriceListingsByVendorId(1, It.IsAny<QueryOptions<PriceListing>>()), Times.Once);
            _priceListingServiceMock.Verify(p => p.GetPriceListingsByVendorId(6, It.IsAny<QueryOptions<PriceListing>>()), Times.Once);

            _robotsTxtCheckerMock.Verify(r => r.CheckRobotsTxt(It.IsAny<Uri>()), Times.Once);

            _loggerMock.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(),
                 It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to filter using robots.txt. Failed to update Listings.")),
                 It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once
             );
        }

        [Fact]
        public async Task UpdateAllListings_WhenHttpClientThrowsException_ShouldLogErrorAndSkipListing()
        {
            // Arrange 
            var vendors = new List<Vendor>{
                new Vendor {VendorId = 1, VendorUrl = "https://example.com"},
                new Vendor {VendorId = 6, VendorUrl = "https://example2.com"},
            };

            var listingsVend1 = new List<PriceListing>
            {
                new PriceListing{PriceListingId = 1, VendorId = 1, Price = 2.00m, DiscountedPrice = 1.50m, PurchaseUrl = "https://example.com/1", ProductId = 1}, // Each has different product id for explicit verification
                new PriceListing{PriceListingId = 2, VendorId = 1, Price = 3.00m, DiscountedPrice = 3.00m, PurchaseUrl = "https://example.com/2", ProductId = 2}
            };

            var listingsVend6 = new List<PriceListing>
            {
                new PriceListing{PriceListingId = 3, VendorId = 6, Price = 2.00m, DiscountedPrice = 1.50m, PurchaseUrl = "https://example2.com/1", ProductId = 3},
                new PriceListing{PriceListingId = 4, VendorId = 6, Price = 2.00m, DiscountedPrice = 2.00m, PurchaseUrl = "https://example2.com/2", ProductId = 4}
            };

            // Pass the other two functions fine
            _vendorServiceMock.Setup(v => v.GetAllVendorsAsync(It.IsAny<QueryOptions<Vendor>>()))
                .ReturnsAsync(vendors);

            _robotsTxtCheckerMock.Setup(c => c.CheckRobotsTxt(It.IsAny<Uri>()))
                .ReturnsAsync(true);

            _priceParserFactoryMock.Setup(p => p.HasParserForDomain(It.IsAny<string>()))
                .Returns(true);

            // Main method setups 

            _priceListingServiceMock.Setup(s => s.GetPriceListingsByVendorId(1, It.IsAny<QueryOptions<PriceListing>>()))
                .ReturnsAsync(listingsVend1);

            _priceListingServiceMock.Setup(s => s.GetPriceListingsByVendorId(6, It.IsAny<QueryOptions<PriceListing>>()))
                .ReturnsAsync(listingsVend6);


            // Mock GetPriceListingById for each listing
            foreach (var listing in listingsVend1.Concat(listingsVend6))
            {
                _priceListingServiceMock.Setup(p => p.GetPriceListingById(listing.PriceListingId, It.IsAny<QueryOptions<PriceListing>>()))
                    .ReturnsAsync(listing);
            }

            // Mock HTTP client responses
            _scraperHttpClientMock.Setup(c => c.SendRequestAsync(It.IsAny<Uri>(), HttpMethod.Get))
                .ThrowsAsync(new Exception("Test Exception"));

            // Mock RateLimiter behavior (just execute the action immediately)
            _rateLimiterMock.Setup(r => r.EnqueueRequest(It.IsAny<Func<Task>>(), It.IsAny<string>()))
                .Returns<Func<Task>, string>((action, domain) => action());

            // Act
            await _priceScraperService.UpdateAllListings();

            // Assert
            _vendorServiceMock.Verify(v => v.UpdateVendorAsync(It.IsAny<Vendor>()), Times.Never);

            _priceListingServiceMock.Verify(p => p.GetPriceListingsByVendorId(1, It.IsAny<QueryOptions<PriceListing>>()), Times.Once);
            _priceListingServiceMock.Verify(p => p.GetPriceListingsByVendorId(6, It.IsAny<QueryOptions<PriceListing>>()), Times.Once);

            _robotsTxtCheckerMock.Verify(r => r.CheckRobotsTxt(It.IsAny<Uri>()), Times.Exactly(4));

            // These should show the ScrapeAndUpdate() task failing at send request async
            _scraperHttpClientMock.Verify(c => c.SendRequestAsync(It.IsAny<Uri>(), HttpMethod.Get), Times.Exactly(4));
            _priceParserFactoryMock.Verify(p => p.GetParserForDomain(It.IsAny<string>()), Times.Exactly(0));


            _loggerMock.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to scrape or update listing for")),
                It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Exactly(4)
            );
        }

        [Fact]
        public async Task UpdateAllListings_WhenUpdatePriceListingFails_ShouldLogErrorAndContinue()
        {
            // Arrange 
            var vendors = new List<Vendor>{
                new Vendor {VendorId = 1, VendorUrl = "https://example.com"},
                new Vendor {VendorId = 6, VendorUrl = "https://example2.com"},
            };

            var listingsVend1 = new List<PriceListing>
            {
                new PriceListing{PriceListingId = 1, VendorId = 1, Price = 2.00m, DiscountedPrice = 1.50m, PurchaseUrl = "https://example.com/1", ProductId = 1}, // Each has different product id for explicit verification
                new PriceListing{PriceListingId = 2, VendorId = 1, Price = 3.00m, DiscountedPrice = 3.00m, PurchaseUrl = "https://example.com/2", ProductId = 2}
            };

            var listingsVend6 = new List<PriceListing>
            {
                new PriceListing{PriceListingId = 3, VendorId = 6, Price = 2.00m, DiscountedPrice = 1.50m, PurchaseUrl = "https://example2.com/1", ProductId = 3},
                new PriceListing{PriceListingId = 4, VendorId = 6, Price = 2.00m, DiscountedPrice = 2.00m, PurchaseUrl = "https://example2.com/2", ProductId = 4}
            };

            // Pass the other two functions fine
            _vendorServiceMock.Setup(v => v.GetAllVendorsAsync(It.IsAny<QueryOptions<Vendor>>()))
                .ReturnsAsync(vendors);

            _robotsTxtCheckerMock.Setup(c => c.CheckRobotsTxt(It.IsAny<Uri>()))
                .ReturnsAsync(true);

            _priceParserFactoryMock.Setup(p => p.HasParserForDomain(It.IsAny<string>()))
                .Returns(true);

            // Main method setups 

            _priceListingServiceMock.Setup(s => s.GetPriceListingsByVendorId(1, It.IsAny<QueryOptions<PriceListing>>()))
                .ReturnsAsync(listingsVend1);

            _priceListingServiceMock.Setup(s => s.GetPriceListingsByVendorId(6, It.IsAny<QueryOptions<PriceListing>>()))
                .ReturnsAsync(listingsVend6);


            // Mock GetPriceListingById for each listing
            foreach (var listing in listingsVend1.Concat(listingsVend6))
            {
                _priceListingServiceMock.Setup(p => p.GetPriceListingById(listing.PriceListingId, It.IsAny<QueryOptions<PriceListing>>()))
                    .ReturnsAsync(listing);
            }


            // Mock Price Parser    
            var mockParser = new Mock<IPriceParser>();

            mockParser.Setup(p => p.ParsePriceAsync(It.IsAny<Uri>()))
                .ReturnsAsync((5.00m, 4.50m));

            _priceParserFactoryMock.Setup(p => p.GetParserForDomain(It.IsAny<string>()))
                .Returns(mockParser.Object);

            // Mock HTTP client responses
            _scraperHttpClientMock.Setup(c => c.SendRequestAsync(It.IsAny<Uri>(), HttpMethod.Get))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("<html>Valid Content</html>")
                });

            // Mock RateLimiter behavior (just execute the action immediately)
            _rateLimiterMock.Setup(r => r.EnqueueRequest(It.IsAny<Func<Task>>(), It.IsAny<string>()))
                .Returns<Func<Task>, string>((action, domain) => action());


            _priceListingServiceMock.Setup(p => p.UpdatePriceListing(It.IsAny<PriceListing>()))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            await _priceScraperService.UpdateAllListings();

            // Assert
            _vendorServiceMock.Verify(v => v.UpdateVendorAsync(It.IsAny<Vendor>()), Times.Never);

            _priceListingServiceMock.Verify(p => p.GetPriceListingsByVendorId(1, It.IsAny<QueryOptions<PriceListing>>()), Times.Once);
            _priceListingServiceMock.Verify(p => p.GetPriceListingsByVendorId(6, It.IsAny<QueryOptions<PriceListing>>()), Times.Once);

            _robotsTxtCheckerMock.Verify(r => r.CheckRobotsTxt(It.IsAny<Uri>()), Times.Exactly(4));

            // These should show the ScrapeAndUpdate() task failing at update price listing
            _scraperHttpClientMock.Verify(c => c.SendRequestAsync(It.IsAny<Uri>(), HttpMethod.Get), Times.Exactly(4));
            _priceParserFactoryMock.Verify(p => p.GetParserForDomain(It.IsAny<string>()), Times.Exactly(4));
            mockParser.Verify(p => p.ParsePriceAsync(It.IsAny<Uri>()), Times.Exactly(4));

            // Check that pricelisting updates are what are stoping it
            _priceListingServiceMock.Verify(p => p.UpdatePriceListing(It.IsAny<PriceListing>()), Times.Exactly(4));

            _loggerMock.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to scrape or update listing for")),
                It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Exactly(4)
            );

            _loggerMock.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Updated listing ")),
                It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Never
            );
        }

        [Fact]
        public async Task UpdateAllListings_WhenNoVendorsSupportScraping_ShouldNotExecuteAnyTasks()
        {
            // Arrange 
            var vendors = new List<Vendor>{
                new Vendor {VendorId = 1, VendorUrl = "https://example.com"},
                new Vendor {VendorId = 6, VendorUrl = "https://example2.com"},
            };

            var listingsVend1 = new List<PriceListing>
            {
                new PriceListing{PriceListingId = 1, VendorId = 1, Price = 2.00m, DiscountedPrice = 1.50m, PurchaseUrl = "https://example.com/1", ProductId = 1}, // Each has different product id for explicit verification
                new PriceListing{PriceListingId = 2, VendorId = 1, Price = 3.00m, DiscountedPrice = 3.00m, PurchaseUrl = "https://example.com/2", ProductId = 2}
            };

            var listingsVend6 = new List<PriceListing>
            {
                new PriceListing{PriceListingId = 3, VendorId = 6, Price = 2.00m, DiscountedPrice = 1.50m, PurchaseUrl = "https://example2.com/1", ProductId = 3},
                new PriceListing{PriceListingId = 4, VendorId = 6, Price = 2.00m, DiscountedPrice = 2.00m, PurchaseUrl = "https://example2.com/2", ProductId = 4}
            };

            // Pass the other two functions fine
            _vendorServiceMock.Setup(v => v.GetAllVendorsAsync(It.IsAny<QueryOptions<Vendor>>()))
                .ReturnsAsync(vendors);

            _robotsTxtCheckerMock.Setup(c => c.CheckRobotsTxt(It.IsAny<Uri>()))
                .ReturnsAsync(true);

            _priceParserFactoryMock.Setup(p => p.HasParserForDomain(It.IsAny<string>()))
                .Returns(false);

            // Main method setups 

            _priceListingServiceMock.Setup(s => s.GetPriceListingsByVendorId(1, It.IsAny<QueryOptions<PriceListing>>()))
                .ReturnsAsync(listingsVend1);

            _priceListingServiceMock.Setup(s => s.GetPriceListingsByVendorId(6, It.IsAny<QueryOptions<PriceListing>>()))
                .ReturnsAsync(listingsVend6);


            // Mock GetPriceListingById for each listing
            foreach (var listing in listingsVend1.Concat(listingsVend6))
            {
                _priceListingServiceMock.Setup(p => p.GetPriceListingById(listing.PriceListingId, It.IsAny<QueryOptions<PriceListing>>()))
                    .ReturnsAsync(listing);
            }

            // Mock Price Parser    
            var mockParser = new Mock<IPriceParser>();

            mockParser.Setup(p => p.ParsePriceAsync(It.IsAny<Uri>()))
                .ReturnsAsync((5.00m, 4.50m));

            _priceParserFactoryMock.Setup(p => p.GetParserForDomain(It.IsAny<string>()))
                .Returns(mockParser.Object);

            // Mock HTTP client responses
            _scraperHttpClientMock.Setup(c => c.SendRequestAsync(It.IsAny<Uri>(), HttpMethod.Get))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("<html>Valid Content</html>")
                });

            // Mock RateLimiter behavior (just execute the action immediately)
            _rateLimiterMock.Setup(r => r.EnqueueRequest(It.IsAny<Func<Task>>(), It.IsAny<string>()))
                .Returns<Func<Task>, string>((action, domain) => action());

            // Act
            await _priceScraperService.UpdateAllListings();

            // Assert
            _vendorServiceMock.Verify(v => v.UpdateVendorAsync(It.IsAny<Vendor>()), Times.Exactly(2));

            _priceListingServiceMock.Verify(p => p.GetPriceListingsByVendorId(1, It.IsAny<QueryOptions<PriceListing>>()), Times.Never);
            _priceListingServiceMock.Verify(p => p.GetPriceListingsByVendorId(6, It.IsAny<QueryOptions<PriceListing>>()), Times.Never);

            _robotsTxtCheckerMock.Verify(r => r.CheckRobotsTxt(It.IsAny<Uri>()), Times.Never);

            // These should show the ScrapeAndUpdate() task succeeding for no listings
            _scraperHttpClientMock.Verify(c => c.SendRequestAsync(It.IsAny<Uri>(), HttpMethod.Get), Times.Never);
            _priceParserFactoryMock.Verify(p => p.GetParserForDomain(It.IsAny<string>()), Times.Never);
            mockParser.Verify(p => p.ParsePriceAsync(It.IsAny<Uri>()), Times.Never);

            _loggerMock.Verify(l => l.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task UpdateAllListings_WhenNoValidUrisAreFound_ShouldNotExecuteAnyTasks()
        {
            // Arrange 
            var vendors = new List<Vendor>{
                new Vendor {VendorId = 1, VendorUrl = "https://example.com"},
                new Vendor {VendorId = 6, VendorUrl = "https://example2.com"},
            };

            var listingsVend1 = new List<PriceListing>
            {
                new PriceListing{PriceListingId = 1, VendorId = 1, Price = 2.00m, DiscountedPrice = 1.50m, PurchaseUrl = "hsfas", ProductId = 1}, // Each has different product id for explicit verification
                new PriceListing{PriceListingId = 2, VendorId = 1, Price = 3.00m, DiscountedPrice = 3.00m, PurchaseUrl = "basdirw", ProductId = 2}
            };

            var listingsVend6 = new List<PriceListing>
            {
                new PriceListing{PriceListingId = 3, VendorId = 6, Price = 2.00m, DiscountedPrice = 1.50m, PurchaseUrl = "fsfdfsf", ProductId = 3},
                new PriceListing{PriceListingId = 4, VendorId = 6, Price = 2.00m, DiscountedPrice = 2.00m, PurchaseUrl = "sdsadsa", ProductId = 4}
            };

            // Pass the other two functions fine
            _vendorServiceMock.Setup(v => v.GetAllVendorsAsync(It.IsAny<QueryOptions<Vendor>>()))
                .ReturnsAsync(vendors);

            _robotsTxtCheckerMock.Setup(c => c.CheckRobotsTxt(It.IsAny<Uri>()))
                .ReturnsAsync(true);

            _priceParserFactoryMock.Setup(p => p.HasParserForDomain(It.IsAny<string>()))
                .Returns(true);

            // Main method setups 

            _priceListingServiceMock.Setup(s => s.GetPriceListingsByVendorId(1, It.IsAny<QueryOptions<PriceListing>>()))
                .ReturnsAsync(listingsVend1);

            _priceListingServiceMock.Setup(s => s.GetPriceListingsByVendorId(6, It.IsAny<QueryOptions<PriceListing>>()))
                .ReturnsAsync(listingsVend6);


            // Mock GetPriceListingById for each listing
            foreach (var listing in listingsVend1.Concat(listingsVend6))
            {
                _priceListingServiceMock.Setup(p => p.GetPriceListingById(listing.PriceListingId, It.IsAny<QueryOptions<PriceListing>>()))
                    .ReturnsAsync(listing);
            }

            // Mock Price Parser    
            var mockParser = new Mock<IPriceParser>();

            mockParser.Setup(p => p.ParsePriceAsync(It.IsAny<Uri>()))
                .ReturnsAsync((5.00m, 4.50m));

            _priceParserFactoryMock.Setup(p => p.GetParserForDomain(It.IsAny<string>()))
                .Returns(mockParser.Object);

            // Mock HTTP client responses
            _scraperHttpClientMock.Setup(c => c.SendRequestAsync(It.IsAny<Uri>(), HttpMethod.Get))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("<html>Valid Content</html>")
                });

            // Mock RateLimiter behavior (just execute the action immediately)
            _rateLimiterMock.Setup(r => r.EnqueueRequest(It.IsAny<Func<Task>>(), It.IsAny<string>()))
                .Returns<Func<Task>, string>((action, domain) => action());

            // Act
            await _priceScraperService.UpdateAllListings();

            // Assert
            _vendorServiceMock.Verify(v => v.UpdateVendorAsync(It.IsAny<Vendor>()), Times.Never);

            _priceListingServiceMock.Verify(p => p.GetPriceListingsByVendorId(1, It.IsAny<QueryOptions<PriceListing>>()), Times.Once);
            _priceListingServiceMock.Verify(p => p.GetPriceListingsByVendorId(6, It.IsAny<QueryOptions<PriceListing>>()), Times.Once);

            _robotsTxtCheckerMock.Verify(r => r.CheckRobotsTxt(It.IsAny<Uri>()), Times.Never);

            // These should show the ScrapeAndUpdate() task succeeding for no listings
            _scraperHttpClientMock.Verify(c => c.SendRequestAsync(It.IsAny<Uri>(), HttpMethod.Get), Times.Never);
            _priceParserFactoryMock.Verify(p => p.GetParserForDomain(It.IsAny<string>()), Times.Never);
            mockParser.Verify(p => p.ParsePriceAsync(It.IsAny<Uri>()), Times.Never);

            _loggerMock.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Invalid URL format for listing")),
                It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Exactly(4)
            );
        }

        [Fact]
        public async Task UpdateAllListings_WhenSomeUrisInvalid_ShouldSkipInvalidOnes()
        {
                        // Arrange 
            var vendors = new List<Vendor>{
                new Vendor {VendorId = 1, VendorUrl = "https://example.com"},
                new Vendor {VendorId = 6, VendorUrl = "https://example2.com"},
            };

            var listingsVend1 = new List<PriceListing>
            {
                new PriceListing{PriceListingId = 1, VendorId = 1, Price = 2.00m, DiscountedPrice = 1.50m, PurchaseUrl = "https://example.com/1", ProductId = 1}, // Each has different product id for explicit verification
                new PriceListing{PriceListingId = 2, VendorId = 1, Price = 3.00m, DiscountedPrice = 3.00m, PurchaseUrl = "basdirw", ProductId = 2}
            };

            var listingsVend6 = new List<PriceListing>
            {
                new PriceListing{PriceListingId = 3, VendorId = 6, Price = 2.00m, DiscountedPrice = 1.50m, PurchaseUrl = "fsfdfsf", ProductId = 3},
                new PriceListing{PriceListingId = 4, VendorId = 6, Price = 2.00m, DiscountedPrice = 2.00m, PurchaseUrl = "https://example2.com/2", ProductId = 4}
            };

            // Pass the other two functions fine
            _vendorServiceMock.Setup(v => v.GetAllVendorsAsync(It.IsAny<QueryOptions<Vendor>>()))
                .ReturnsAsync(vendors);

            _robotsTxtCheckerMock.Setup(c => c.CheckRobotsTxt(It.IsAny<Uri>()))
                .ReturnsAsync(true);

            _priceParserFactoryMock.Setup(p => p.HasParserForDomain(It.IsAny<string>()))
                .Returns(true);

            // Main method setups 

            _priceListingServiceMock.Setup(s => s.GetPriceListingsByVendorId(1, It.IsAny<QueryOptions<PriceListing>>()))
                .ReturnsAsync(listingsVend1);

            _priceListingServiceMock.Setup(s => s.GetPriceListingsByVendorId(6, It.IsAny<QueryOptions<PriceListing>>()))
                .ReturnsAsync(listingsVend6);


            // Mock GetPriceListingById for each listing
            foreach (var listing in listingsVend1.Concat(listingsVend6))
            {
                _priceListingServiceMock.Setup(p => p.GetPriceListingById(listing.PriceListingId, It.IsAny<QueryOptions<PriceListing>>()))
                    .ReturnsAsync(listing);
            }

            // Mock Price Parser    
            var mockParser = new Mock<IPriceParser>();

            mockParser.Setup(p => p.ParsePriceAsync(It.IsAny<Uri>()))
                .ReturnsAsync((5.00m, 4.50m));

            _priceParserFactoryMock.Setup(p => p.GetParserForDomain(It.IsAny<string>()))
                .Returns(mockParser.Object);

            // Mock HTTP client responses
            _scraperHttpClientMock.Setup(c => c.SendRequestAsync(It.IsAny<Uri>(), HttpMethod.Get))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("<html>Valid Content</html>")
                });

            // Mock RateLimiter behavior (just execute the action immediately)
            _rateLimiterMock.Setup(r => r.EnqueueRequest(It.IsAny<Func<Task>>(), It.IsAny<string>()))
                .Returns<Func<Task>, string>((action, domain) => action());

            // Act
            await _priceScraperService.UpdateAllListings();
            
            // Assert
            _vendorServiceMock.Verify(v => v.UpdateVendorAsync(It.IsAny<Vendor>()), Times.Never);

            _priceListingServiceMock.Verify(p => p.GetPriceListingsByVendorId(1, It.IsAny<QueryOptions<PriceListing>>()), Times.Once);
            _priceListingServiceMock.Verify(p => p.GetPriceListingsByVendorId(6, It.IsAny<QueryOptions<PriceListing>>()), Times.Once);

            _robotsTxtCheckerMock.Verify(r => r.CheckRobotsTxt(It.IsAny<Uri>()), Times.Exactly(2));

            // These should show the ScrapeAndUpdate() task succeeding for 2 listings
            _scraperHttpClientMock.Verify(c => c.SendRequestAsync(It.IsAny<Uri>(), HttpMethod.Get), Times.Exactly(2));
            _priceParserFactoryMock.Verify(p => p.GetParserForDomain(It.IsAny<string>()), Times.Exactly(2));
            mockParser.Verify(p => p.ParsePriceAsync(It.IsAny<Uri>()), Times.Exactly(2));

            _loggerMock.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Invalid URL format for listing")),
                It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Exactly(2)
            );
        }


        // ---------------------------------------------------------------- UpdateListing (not implemented yet don't even know if want to keep function) ------------------------------------------------
    }
}