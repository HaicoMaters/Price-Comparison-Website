using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Threading.Tasks;
using Price_Comparison_Website.Models;
using Price_Comparison_Website.Services.HttpClients;
using Price_Comparison_Website.Services.Implementations;
using Price_Comparison_Website.Services.Interfaces;
using Price_Comparison_Website.Services.Utilities;
using Price_Comparison_Website.Services.Utilities.Interfaces;
using Price_Comparison_Website.Services.WebScraping.Interfaces;

namespace Price_Comparison_Website.Services.WebScraping
{
    public class PriceScraperService : IPriceScraperService // UPDATE THE INTERFACE BEFORE WORKING ON THIS 
    {

        private readonly IRobotsTxtChecker _robotsTxtChecker;
        private readonly IVendorService _vendorService;
        private readonly IPriceListingService _priceListingService;
        private readonly IPriceParserFactory _priceParserFactory;
        private readonly ILogger<PriceScraperService> _logger;
        private readonly IScraperRateLimiter _rateLimiter;
        private readonly IScraperHttpClient _scraperHttpClient;


        public PriceScraperService(
            IRobotsTxtChecker robotsTxtChecker,
            IVendorService vendorService,
            IPriceListingService priceListingService,
            ILogger<PriceScraperService> logger,
            IScraperRateLimiter rateLimiter,
            IScraperHttpClient scraperHttpClient,
            IPriceParserFactory priceParserFactory
        )
        {
            _robotsTxtChecker = robotsTxtChecker;
            _logger = logger;
            _vendorService = vendorService;
            _priceListingService = priceListingService;
            _rateLimiter = rateLimiter;
            _scraperHttpClient = scraperHttpClient;
            _priceParserFactory = priceParserFactory;
        }

        // maybe keep somewhere when the last update  was for the automatic updater
        // or just do at the same time everyday regardless of if admin requested it
        //


        /* Try to do things in order
            Get All interfaces done so know functions that need to build off each other
            This Class - Main logic and how things are handled
            PriceParserFactory - Filtering logic to filter into valid parsers for each uri
            RateLimiter - Handle Flow Control of requests
            ScraperHttpClient - Make Requests based on flow control of rate limiter
            RetryHandler - For Fault tolerance
            Parsers - Add parsers for each vendor as available keep note of all currently supported vendors should have one example parser probably amazon for testing earlier on than expected
            Scraper Api Controller (// DONT FORGET TO DO NOTIFICATION API CONTROLLER)


            For writing up in readme later robotstxtchecker.cs checks that a uri is valid in terms of the sites rules for robots.txt scraping rule and helps filter which sites allow for what
            so no requests against policy are made
        */

        // Should probably have somewhere where listings match their venodr e.g. if tesco is picked as vendor with a link that is amazon.co.uk ; it should automatically be changed to amazon

        public async Task UpdateAllListings()
        {
            var vendorIds = await GetVendorIdsThatSupportScraping();

            Dictionary<Uri, int> uris = new Dictionary<Uri, int>(); // Uri attached with price listing id

            // Get all price listings by vendor id and add those with available parsers
            foreach (int vendId in vendorIds)
            {
                var listings = await _priceListingService.GetPriceListingsByVendorId(vendId, new QueryOptions<PriceListing>());
                foreach (var listing in listings)
                {
                    if (Uri.TryCreate(listing.PurchaseUrl, UriKind.Absolute, out var uri))
                    {
                        string domain = uri.Host;

                        if (_priceParserFactory.HasParserForDomain(domain))
                        {
                            uris[uri] = listing.PriceListingId;
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Invalid URL format for listing: {listing.PriceListingId}");
                    }
                }
            }

            // Filter out each URI based on robots.txt
            try
            {
                await FilterUsingRobotsTxt(uris);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to filter using robots.txt. Failed to update Listings.");
                return;
            }

            var tasks = new List<Task>();

            foreach (var kvp in uris)
            {
                var uri = kvp.Key;
                var listingId = kvp.Value;
                string domain = uri.Host;

                async Task ScrapeAndUpdate()
                {
                    try
                    {
                        // Send request using HTTP client
                        var httpResponse = await _scraperHttpClient.SendRequestAsync(uri, HttpMethod.Get);

                        // Get the correct parser
                        var parser = _priceParserFactory.GetParserForDomain(domain);

                        if (parser != null)
                        {
                            // Parse the content to extract prices
                            var (price, discountedPrice) = await parser.ParsePriceAsync(httpResponse);

                            // Update the listing with new prices
                            var listing = await _priceListingService.GetPriceListingById(listingId, new QueryOptions<PriceListing>());

                            listing.Price = price;
                            listing.DiscountedPrice = discountedPrice;

                            await _priceListingService.UpdatePriceListing(listing);

                            _logger.LogInformation($"Updated listing {uri} with Price: {price} and DiscountedPrice: {discountedPrice}");

                            await _priceListingService.UpdateCheapestPrice(listing.ProductId, discountedPrice);
                        }
                        else
                        {
                            _logger.LogWarning($"No parser found for {domain}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to scrape or update listing for {uri}");
                    }
                }

                // Enqueue request through the rate limiter (one per domain at a time)
                await _rateLimiter.EnqueueRequest(ScrapeAndUpdate, domain);
            }

            await _rateLimiter.StopProcessing();
            _logger.LogInformation("All listings have been updated.");
        }



        public async Task<List<int>> GetVendorIdsThatSupportScraping()
        {
            var vendors = await _vendorService.GetAllVendorsAsync(new QueryOptions<Vendor>
            {
                Where = v => v.SupportsAutomaticUpdates
            });

            List<int> vendorIds = new List<int>();

            foreach (var vendor in vendors)
            {
                // Create Uri and extract the domain
                if (Uri.TryCreate(vendor.VendorUrl, UriKind.Absolute, out var uri))
                {
                    string domain = uri.Host;  // Extract domain from Uri

                    if (_priceParserFactory.HasParserForDomain(domain))
                    {
                        vendorIds.Add(vendor.VendorId);
                    }
                    else
                    {
                        // Vendor does not actually support it, change the flag
                        vendor.SupportsAutomaticUpdates = false;
                        await _vendorService.UpdateVendorAsync(vendor);
                    }
                }
                else
                {
                    // Log or handle invalid vendor URL
                    _logger.LogWarning($"Invalid URL format for vendor: {vendor.VendorUrl}");
                }
            }

            return vendorIds;
        }


        public async Task FilterUsingRobotsTxt(Dictionary<Uri, int> uris)
        {
            var keysToRemove = new List<Uri>();

            foreach (var kvp in uris)
            {
                if (!await _robotsTxtChecker.CheckRobotsTxt(kvp.Key))
                {
                    keysToRemove.Add(kvp.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                uris.Remove(key);
            }
        }

        public Task<bool> UpdateListing(int id) // This Can be added when needed
        {
            throw new NotImplementedException();
        }
    }
}