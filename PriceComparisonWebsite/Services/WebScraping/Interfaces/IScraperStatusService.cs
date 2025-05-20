using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceComparisonWebsite.Services.WebScraping.Interfaces
{
    /// <summary>
    /// Service for tracking the status of scraping operations
    /// </summary>
    public interface IScraperStatusService
    {
        /// <summary>
        /// Gets the timestamp of the last successful scraping update
        /// </summary>
        Task<DateTime> GetLastUpdateTime();

        /// <summary>
        /// Updates the timestamp of the last successful scraping operation
        /// </summary>
        Task UpdateLastUpdateTime();
    }
}