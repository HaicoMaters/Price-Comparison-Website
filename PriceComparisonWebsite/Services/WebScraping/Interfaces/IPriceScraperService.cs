using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceComparisonWebsite.Services.WebScraping.Interfaces
{
    /// <summary>
    /// Service for managing web scraping operations for price updates
    /// </summary>
    public interface IPriceScraperService
    {
        /// <summary>
        /// Updates prices for all eligible listings through web scraping
        /// </summary>
        Task UpdateAllListings();

        /// <summary>
        /// Updates a single price listing through web scraping
        /// </summary>
        /// <param name="id">The ID of the listing to update</param>
        /// <returns>True if update was successful, false otherwise</returns>
        Task<bool> UpdateListing(int id);

        /// <summary>
        /// Filters URIs based on robots.txt rules
        /// </summary>
        /// <param name="uris">Dictionary of URIs and their associated listing IDs</param>
        Task FilterUsingRobotsTxt(Dictionary<Uri, int> uris);

        /// <summary>
        /// Gets IDs of vendors that support automatic price updates
        /// </summary>
        /// <returns>List of vendor IDs that support scraping</returns>
        Task<List<int>> GetVendorIdsThatSupportScraping();
    }
}