using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Price_Comparison_Website.Models;
using Price_Comparison_Website.Services.Utilities;
using Price_Comparison_Website.Services.WebScraping.Interfaces;

namespace Price_Comparison_Website.Services.WebScraping
{
    public class PriceScraperService : IPriceScraperService // UPDATE THE INTERFACE BEFORE WORKING ON THIS 
    {

        private readonly RobotsTxtChecker _robotsTxtChecker;

        /* Try to do things in order
            Get All interfaces done so know functions that need to build off each other
            This Class - Main logic and how things are handled
            PriceParserFactory - Filtering logic to filter into valid parsers for each uri
            RateLimiter - Handle Flow Control of requests
            ScraperHttpClient - Make Requests based on flow control of rate limiter
            RetryHandler - For Fault tolerance
            Parsers - Add parsers for each vendor as available keep note of all currently supported vendors should have one example parser probably amazon for testing earlier on than expected
            Scraper Api Controller (// DONT FORGET TO DO NOTIFICATION API CONTROLLER)
        */

        public async Task DoAutomaticUpdates()
        {
            var vendorIds = await GetVendorIdsThatSupportScraping();

            // get all pricelistings by vendor id

            // get each link as a uri

            // Filter out each uri based on if it fits in robots.txt
            var uris = new List<Uri>();
            await FilterUsingRobotsTxt(uris);

            // Filter by price parser factory having it in case of vendor and uri not being the same

            // have scraping http client do all the retrieval
            // rate limited by ratelimiter.cs keep a that has each uri and cooldown for each website
            // retries handled by retryhandler.cs
            // using Price parser factory to parse the page to find price and the discounted price
        }


        public async Task<List<int>> GetVendorIdsThatSupportScraping()
        {

            List<Vendor> vendorList = new List<Vendor>();

            // Get all vendors using vendor service
            // Check that price parser factory includes a valid parser for the vendor
            // Add a method to priceparserfactory that gets a list of all parsers (we can do by name most likely, or some other way of binding it)
            // if it doesn't vendor.SuportsAutomaticUpdates = false and push that to database
            // Otherwise add to vendor list

            List<int> vendorIds = new List<int>();

            // Add all vendor ids to vendor id list and return it

            return vendorIds;
        }

        public async Task FilterUsingRobotsTxt(List<Uri> uris)
        {
            foreach (var uri in uris)
            {
                if (!await _robotsTxtChecker.CheckRobotsTxt(uri))
                {
                    uris.Remove(uri);
                }
            }
        }

        public async Task FilterUsingPriceParserFactory(List<Uri> uris)
        {
            // use price parser factory to remove unsupported uris that have been implemented
        }

    }
}